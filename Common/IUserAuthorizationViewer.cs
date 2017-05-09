using System;

namespace OAuth2Manager.Common
{
    public interface IUserAuthorizationViewer
    {
        Uri AuthorizeUrl { set; }
        IUserConsentHandler AuthController { set; }
    }
}
