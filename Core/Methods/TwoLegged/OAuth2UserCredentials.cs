using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OAuth2Manager.Common;

namespace OAuth2Manager.Core.Methods.TwoLegged
{
    public class OAuth2UserCredentials : OAuth2Base
    {
        private string clientSecret = null;
        public OAuth2UserCredentials(
                    string clientId, string clientSecret,
                    string redirectUrl, string scope, string authorizationUrl)
            : base(clientId, redirectUrl, scope, authorizationUrl)
        {

            this.clientSecret = clientSecret;
        }

        public virtual async Task<bool> InvokeUserAuthorization()
        {
            //initialize the underlying OAuthorizer
            InitAuthorizer(clientSecret);

            base.OAuthState = OAuthState.INITIALIZED;
            var parameters = new List<KeyValuePair<string, string>>(capacity: 6)
                {
                    new KeyValuePair<string,string>(OAuthConstants.CLIENT_SECRET, this.clientSecret)
                };

            var accessTokenUrl = Authorizer.BuildAuthorizeUrl(AuthorizationUrl, OAuthConstants.GRANT_TYPE_CLIENT_CREDENTIALS, parameters);
            Uri authUri = new Uri(accessTokenUrl);

            OAuthState = OAuthState.ACCESS_TOKEN_WAIT;
            try
            {
                var result = await Authorizer.GetAccessTokenAsync(accessTokenUrl);

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
            catch (Exception)
            {
                OAuthState = OAuthState.FAILED;
                throw;
            }
        }
    }
}
