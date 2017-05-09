using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using OAuth2Manager.Common;
using OAuth2Manager.Common.Tokens;

namespace OAuth2Manager.Core
{
    public class OAuth2Base : IAuthProvider
    {
        internal OAuth2Authorizer Authorizer;
        protected AuthToken AuthToken;

        public AccessToken AccessToken { get; internal set; }

        protected string ClientId;
        protected string RedirectUrl;
        protected string AuthorizationUrl;
        protected string Scope;

        public OAuthState OAuthState { get; protected set; }

        public void RestoreAccessToken(string accessToken, string expiresIn = null, string refreshToken = null)
        {
            var parameters = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(expiresIn))
                parameters.Add(new KeyValuePair<string, string>(OAuthConstants.EXPIRES_IN, expiresIn));

            if (!string.IsNullOrWhiteSpace(refreshToken))
                parameters.Add(new KeyValuePair<string, string>(OAuthConstants.REFRESH_TOKEN, refreshToken));

            AccessToken = new AccessToken(accessToken, parameters.ToLookup(kvp => kvp.Key, kvp => kvp.Value));
        }

        protected OAuth2Base(string clientId, string redirectUrl, string scope, string authorizationUrl)
        {
            ClientId = clientId;
            RedirectUrl = redirectUrl;
            Scope = scope;
            AuthorizationUrl = authorizationUrl;
            OAuthState = OAuthState.INITIALIZED;
        }

        protected void InitAuthorizer()
        {
           Authorizer = new OAuth2Authorizer(ClientId, RedirectUrl, Scope);
        }

        protected void InitAuthorizer(string clientSecret)
        {
            Authorizer = new OAuth2Authorizer(ClientId, clientSecret, RedirectUrl, Scope);
        }


        public virtual bool AppendCredentials(HttpRequestMessage request)
        {
            if (OAuthState == OAuthState.FAILED)
            {
                throw new InvalidOperationException("The OAuth process has failed to authorize this request.");
            }
            if (OAuthState != OAuthState.SUCCEEDED)
            {
                throw new InvalidOperationException("The OAuth process must finish before this request can be made.");
            }

            var headerVal = AccessToken.Code;
            request.Headers.TryAddWithoutValidation(OAuthConstants.OAUTH_HEADER, string.Format(OAuthConstants.OAUTH_HEADER_VALUE_FORMAT, headerVal));
            return true;
        }
    }
}
