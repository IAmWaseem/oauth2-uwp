# OAuthManager-UWP

Usage:

    var oAuth = new OAuth2WebServerFlow(ClientId, ClientSecret, CallbackUrl, Scope, 
                                        AuthorizationUrl, AccessTokenUrl);
                                        
    OAuthUiBroker.ShowOAuthView(oAuth);
    
    OAuthUiBroker.OnAuthenticated += async (sender, args) => {
      await new MessageDialog("Access Token: " + args.AccessToken + 
                              "\nRefreshToken: " + args.RefreshToken + 
                              "\nExpires In: " + args.ExpiresIn).ShowAsync();
    }
