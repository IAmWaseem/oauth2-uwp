using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OAuth2Manager.Common;

namespace OAuth2Manager.Core.Methods.ThreeLegged
{
    public class OAuth2ThreeLeggedBase : OAuth2Base
    {
        protected string accessTokenUrl;
        protected string clientSecret;
        public OAuth2ThreeLeggedBase(string clientId, string clientSecret,
            string redirectUrl, string scope, string authorizationUrl, string accessTokenUrl)
            : base(clientId, redirectUrl, scope, authorizationUrl)
        {
            this.accessTokenUrl = accessTokenUrl;
            this.clientSecret = clientSecret;
        }

        public async Task<bool> RefreshAccessToken()
        {
            //initialize the underlying OAuthorizer
            if (Authorizer == null)
                InitAuthorizer(clientSecret);

            if (OAuthState != OAuthState.SUCCEEDED)
            {
                throw new InvalidOperationException("The request must be authorized before refresh.");
            }

            if (AccessToken == null)
            {
                throw new InvalidOperationException("The access token has not previously been acquired.");
            }

            if (string.IsNullOrWhiteSpace(AccessToken.RefreshToken))
            {
                throw new InvalidOperationException("Refresh token was not found.");
            }

            OAuthState = OAuthState.REFRESH_WAIT;

            var parameters = new List<KeyValuePair<string, string>>(capacity: 6)
                {
                    new KeyValuePair<string,string>(OAuthConstants.REFRESH_TOKEN, this.AccessToken.RefreshToken),
                    new KeyValuePair<string,string>(OAuthConstants.CLIENT_SECRET, this.clientSecret)
                };

            var accessTokenUrl = Authorizer.BuildAuthorizeUrl(AuthorizationUrl, OAuthConstants.GRANT_TYPE_REFRESH_TOKEN, parameters);
            Uri authUri = new Uri(accessTokenUrl);

            OAuthState = OAuthState.ACCESS_TOKEN_WAIT;
            try
            {
                var result = await Authorizer.GetAccessTokenAsync(accessTokenUrl);

                if (result != null)
                {
                    OAuthState = OAuthState.SUCCEEDED;
                    var token = result.Token;
                    base.RestoreAccessToken(token.Code, token.Expires, token.RefreshToken);

                    return true;
                }
                else
                {
                    OAuthState = OAuthState.FAILED;
                    return false;
                }
            }
            catch (Exception ex)
            {
                OAuthState = OAuthState.FAILED;
                throw;
            }
        }


    }
}
