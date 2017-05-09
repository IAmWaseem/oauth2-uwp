namespace OAuth2Manager.Core
{
    public enum OAuthState
    {
        INITIALIZED,
        UNINITIALIZED,
        REQUEST_TOKEN_WAIT,
        AUTH_TOKEN_WAIT,
        ACCESS_TOKEN_WAIT,
        SUCCEEDED,  
        FAILED,
        REFRESH_WAIT
    }
}
