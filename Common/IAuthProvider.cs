using System.Net.Http;

namespace OAuth2Manager.Common
{
    public interface IAuthProvider
    {
        bool AppendCredentials(HttpRequestMessage request);
    }
}
