using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
#pragma warning disable 618

namespace OAuth2Manager.Common.Tokens
{
    [DataContract]
    public class AuthToken : Token
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AuthToken()
        { }

        public AuthToken(string code, ILookup<string, string> extraData)
            : base(code, extraData)
        { }
    }
}
