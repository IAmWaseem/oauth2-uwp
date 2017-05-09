using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
#pragma warning disable 618

namespace OAuth2Manager.Common.Tokens
{
    [DataContract]
    public class AccessToken : Token
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AccessToken()
        { }

        public AccessToken(string accessToken, ILookup<string, string> extraData)
            : base(accessToken, extraData)
        { }

        public string Expires => ExtraData?[OAuthConstants.EXPIRES_IN].FirstOrDefault();

        public string RefreshToken => ExtraData?[OAuthConstants.REFRESH_TOKEN].FirstOrDefault();
    }
}
