using System;
using System.Threading.Tasks;
using OAuth2Manager.Common;
using OAuth2Manager.Common.Tokens;

namespace OAuth2Manager.Core.Methods.ThreeLegged
{
    public class OAuth2PinBasedFlow : OAuth2ThreeLeggedBase
    {
        public OAuth2PinBasedFlow(string clientId, string clientSecret, string redirectUrl, string scope, string authorizationUrl, string accessTokenUrl) : 
            base(clientId, clientSecret, redirectUrl, scope, authorizationUrl, accessTokenUrl)
        {
        }

        public virtual Uri GetUserTokenUrl()
        {
            //initialize the underlying OAuthorizer
            InitAuthorizer(clientSecret);

            base.OAuthState = OAuthState.AUTH_TOKEN_WAIT;
            var authorizeUrlResponse = Authorizer.BuildAuthorizeUrl(AuthorizationUrl, OAuthConstants.RESPONSE_TYPE_CODE);
            return new Uri(authorizeUrlResponse);
        }

        public virtual async Task<bool> ProcessUserAuthorizationAsync(string verifier)
        {
            OAuthState = OAuthState.ACCESS_TOKEN_WAIT;
            AuthToken = new AuthToken(verifier, null);
            var result = await Authorizer.GetAccessTokenAsync(accessTokenUrl, AuthToken, OAuthConstants.GRANT_TYPE_AUTH_CODE);

            if (result != null)
            {
                OAuthState = OAuthState.SUCCEEDED;
                AccessToken = result.Token;
                return true;
            }
            else
            {
                OAuthState = OAuthState.FAILED;
                return false;
            }
        }
    }
}
