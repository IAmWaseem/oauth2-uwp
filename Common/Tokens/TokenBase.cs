using System.Linq;

namespace OAuth2Manager.Common.Tokens
{
    public abstract class TokenBase
    {
        public ILookup<string, string> ExtraData { get; protected set; }
    }
}
