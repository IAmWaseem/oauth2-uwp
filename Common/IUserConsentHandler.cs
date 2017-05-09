using System;
using System.Threading.Tasks;

namespace OAuth2Manager.Common
{
    public interface IUserConsentHandler
    {
        bool IsCallBack(Uri currentUrl);
        Task<bool> ProcessUserAuthorizationAsync(Uri currentUrl);
    }
}
