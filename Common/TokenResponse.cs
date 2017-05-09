using OAuth2Manager.Common.Tokens;

namespace OAuth2Manager.Common
{
    public class TokenResponse<T> where T : TokenBase
    {
        public T Token { get; private set; }

        public TokenResponse(T token)
        {
            Precondition.NotNull(token, "token");

            this.Token = token;
        }
    }
}
