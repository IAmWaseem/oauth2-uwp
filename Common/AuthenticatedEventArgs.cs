using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2Manager.Common
{
    public class AuthenticatedEventArgs : EventArgs
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }

        public AuthenticatedEventArgs(string accessToken, string refreshToken, string expiresIn)
        {
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
            this.ExpiresIn = expiresIn;
        }
    }
}
