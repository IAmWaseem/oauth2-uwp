using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OAuth2Manager.Common;
using OAuth2Manager.Common.Tokens;
using OAuth2Manager.Extensions;

namespace OAuth2Manager.Core
{
    public class OAuth2Authorizer
    {
        readonly string _clientId;
        readonly string _clientSecret;
        readonly string _redirectUrl;
        readonly string _scope;

        private OAuth2Authorizer(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scope = string.Empty;
            _redirectUrl = OAuthConstants.LOCALHOST;
        }

        public OAuth2Authorizer(string consumerKey, string consumerSecret, string redirectUrl, string scope)
            : this(consumerKey, consumerSecret)
        {
            _redirectUrl = redirectUrl;
            _scope = scope;
        }

        public OAuth2Authorizer(string consumerKey, string redirectUrl, string scope)
        {
            _clientId = consumerKey;
            _clientSecret = null;
            _redirectUrl = redirectUrl;
            _scope = scope;
        }

        public TokenResponse<AccessToken> GetAccessTokenFromResponse(Uri url)
        {
            var tokenResponse = GetTokenResponseFromFragment
                (url, OAuthConstants.ACCESS_TOKEN, (code, data) => new AccessToken(code, data));
            return tokenResponse;
        }

        public TokenResponse<AuthToken> GetAuthTokenFromResponse(Uri url)
        {
            var tokenResponse = GetTokenResponseFromQuery
                (url, OAuthConstants.RESPONSE_TYPE_CODE, (code, data) => new AuthToken(code, data));
            return tokenResponse;
        }

        private TokenResponse<T> GetTokenResponseFromFragment<T>(
            Uri url, string tokenName, Func<string, ILookup<string, string>, T> tokenFactory)
            where T : Token
        {
            var tokenBase = url.Fragment;
            if (tokenBase.StartsWith("#"))
                tokenBase = tokenBase.Substring(1);

            return ExtractTokenAndExtraData(tokenName, tokenFactory, tokenBase);
        }

        private TokenResponse<T> GetTokenResponseFromQuery<T>(
            Uri url, string tokenName, Func<string, ILookup<string, string>, T> tokenFactory)
            where T : Token
        {
            var tokenBase = url.Query;
            if (tokenBase.StartsWith("?"))
                tokenBase = tokenBase.Substring(1);

            return ExtractTokenAndExtraData(tokenName, tokenFactory, tokenBase);
        }

        private static TokenResponse<T> ExtractTokenAndExtraDataJson<T>(string tokenName,
            Func<string, ILookup<string, string>, T> tokenFactory, string tokenBase) where T : Token
        {

            //Convert to dictionary
            var results = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(tokenBase);

            //Find access token
            string accessToken = null;
            var extraData = new Dictionary<string, string>();
            foreach (var item in results)
            {
                if (item.Key.Equals(tokenName))
                {
                    accessToken = item.Value as string;
                    continue;
                }
                if (item.Value is string)
                {

                    extraData.Add(item.Key, item.Value as string);
                }
                else if (item.Value is int)
                {
                    var val = item.Value as int?;
                    extraData.Add(item.Key, val.ToString());
                }
            }

            var data = extraData.ToLookup(kv => kv.Key, kv => kv.Value);
            var token = tokenFactory(accessToken, data);
            return new TokenResponse<T>(token);
        }

        private static TokenResponse<T> ExtractTokenAndExtraData<T>(string tokenName,
            Func<string, ILookup<string, string>, T> tokenFactory, string tokenBase) where T : Token
        {
            var splitted = tokenBase.Split('&').Select(s => s.Split('=')).ToLookup(xs => xs[0], xs => xs[1]);
            var code = splitted[tokenName].First().UrlDecode();
            var extraData = splitted.Where(kvp => kvp.Key != tokenName)
                .SelectMany(g => g, (g, value) => new { g.Key, Value = value })
                .ToLookup(kvp => kvp.Key, kvp => kvp.Value);
            var token = tokenFactory(code, extraData);

            return new TokenResponse<T>(token);
        }

        public string BuildAuthorizeUrl(string authUrl,
            string responseType, IEnumerable<KeyValuePair<string, string>> optionalParameters = null)
        {
            Precondition.NotNull(authUrl, "authUrl");

            var parameters = new List<KeyValuePair<string, string>>(capacity: 8)
            {
                new KeyValuePair<string,string>(OAuthConstants.RESPONSE_TYPE, responseType),
                new KeyValuePair<string,string>(OAuthConstants.CLIENT_ID, _clientId),
                new KeyValuePair<string,string>(OAuthConstants.REDIRECT_URI, _redirectUrl),
                new KeyValuePair<string,string>(OAuthConstants.SCOPE, _scope),
            };

            if (optionalParameters == null) optionalParameters = Enumerable.Empty<KeyValuePair<string, string>>();

            var stringParameter = optionalParameters
                .Where(x => x.Key.ToLower() != OAuthConstants.REALM)
                .Concat(parameters)
                .Select(p => new { Key = p.Key.UrlEncode(), Value = p.Value.UrlEncode() })
                .OrderBy(p => p.Key, StringComparer.Ordinal)
                .ThenBy(p => p.Value, StringComparer.Ordinal)
                .Select(p => p.Key + "=" + p.Value)
                .ToString("&");

            return string.Format("{0}?{1}", authUrl, stringParameter);
        }

        private async Task<TokenResponse<T>> GetTokenResponseAsync<T>(string url, OAuthMessageHandler handler,
            HttpContent postValue, Func<string, ILookup<string, string>, T> tokenFactory) where T : Token
        {
            var client = new HttpClient(handler);

            var response = await client.PostAsync(url,
                postValue ?? new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()));

            var tokenBase = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException(response.StatusCode + ":" + tokenBase); // error message
            }

            if (!tokenBase.Contains(OAuthConstants.ACCESS_TOKEN)) return null;
            return tokenBase.IsJson() ? ExtractTokenAndExtraDataJson(OAuthConstants.ACCESS_TOKEN, tokenFactory, tokenBase) : ExtractTokenAndExtraData(OAuthConstants.ACCESS_TOKEN, tokenFactory, tokenBase);
        }

        public async Task<TokenResponse<AccessToken>> GetAccessTokenAsync(string accessTokenUrl,
            IEnumerable<KeyValuePair<string, string>> parameters = null, HttpContent postValue = null)
        {
            Precondition.NotNull(accessTokenUrl, "accessTokenUrl");

            if (parameters == null) parameters = Enumerable.Empty<KeyValuePair<string, string>>();
            var handler = new OAuthMessageHandler(_clientId, _redirectUrl, parameters);

            return await GetTokenResponseAsync(accessTokenUrl, handler, postValue, (code, data) => new AccessToken(code, data));
        }

        public async Task<TokenResponse<AccessToken>> GetAccessTokenAsync(string accessTokenUrl, AuthToken authToken,
            string grantType, IEnumerable<KeyValuePair<string, string>> parameters = null, HttpContent postValue = null)
        {
            Precondition.NotNull(accessTokenUrl, "accessTokenUrl");
            Precondition.NotNull(authToken, "authToken");
            Precondition.NotNull(grantType, "grantType");
            Precondition.NotNull(_clientId, "clientId");
            Precondition.NotNull(_clientSecret, "clientSecret");

            var sendParameters = new List<KeyValuePair<string, string>>(8)
                {
                    new KeyValuePair<string,string>(OAuthConstants.CODE, authToken.Code),
                    new KeyValuePair<string,string>(OAuthConstants.CLIENT_SECRET, _clientSecret),
                    new KeyValuePair<string,string>(OAuthConstants.REDIRECT_URI, _redirectUrl),
                    new KeyValuePair<string, string>(OAuthConstants.GRANT_TYPE, grantType),
                    new KeyValuePair<string, string>(OAuthConstants.SCOPE, _scope)
                };

            if (parameters == null) parameters = Enumerable.Empty<KeyValuePair<string, string>>();
            var handler = new OAuthMessageHandler(_clientId, _redirectUrl, parameters.Concat(sendParameters));

            return await GetTokenResponseAsync(accessTokenUrl, handler, postValue, (code, data) => new AccessToken(code, data));
        }
    }
}
