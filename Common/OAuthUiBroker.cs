using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using OAuth2Manager.Core.Methods.ThreeLegged;
using OAuth2Manager.Core.Methods.TwoLegged;
using OAuth2Manager.Extensions;

namespace OAuth2Manager.Common
{
    public class OAuthUiBroker
    {
        public static EventHandler<AuthenticatedEventArgs> OnAuthenticated;

        private static List<UIElement> controls;
        public static void ShowOAuthView(OAuth2WebServerFlow oAuth)
        {
            StoreElements();
            var webAuthView = new FlexibleWebAuthView { Height = Window.Current.Bounds.Height };
            oAuth.InvokeUserAuthorization(webAuthView);
            var elements = GetUIElementCollectionInstance();
            elements.Clear();
            elements.Add(webAuthView);
            webAuthView.Navigate(webAuthView.AuthorizeUrl);
            webAuthView.OAuthSuccess += async (sender, args) =>
            {
                await oAuth.ProcessUserAuthorizationAsync(sender as Uri);
                RestoreView();
                OnAuthenticated?.Invoke(oAuth, new AuthenticatedEventArgs(oAuth.AccessToken.Code, oAuth.AccessToken.RefreshToken, oAuth.AccessToken.Expires));
                await new MessageDialog("Access Token Received: " + oAuth.AccessToken.Code).ShowAsync();
            };
        }

        public static async void ShowOAuthView(OAuth2PinBasedFlow oAuth)
        {
            StoreElements();
            var contentDialog = new ContentDialog();
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            var urlTextBlock = new TextBlock
            {
                Text = "Open URL in Browser: " + oAuth.GetUserTokenUrl(),
                VerticalAlignment = VerticalAlignment.Center
            };
            var pinTextBox = new TextBox { Width = 150.0f };
            var okButton = new Button { Content = "OK" };
            okButton.Click += async (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(pinTextBox.Text))
                {
                    await new MessageDialog("Please Enter Pin").ShowAsync();
                }
                if (await oAuth.ProcessUserAuthorizationAsync(pinTextBox.Text))
                {
                    contentDialog.Hide();
                }
                else
                {
                    await new MessageDialog("Invalid Pin").ShowAsync();
                }
            };

            stackPanel.Children.Add(urlTextBlock);
            stackPanel.Children.Add(pinTextBox);
            stackPanel.Children.Add(okButton);

            contentDialog.Content = stackPanel;
            await contentDialog.ShowAsync();
        }

        public static void ShowOAuthView(OAuth2BrowserBasedFlow oAuth)
        {
            StoreElements();
            var webAuthView = new FlexibleWebAuthView { Height = Window.Current.Bounds.Height };
            oAuth.InvokeUserAuthorization(webAuthView);
            var elements = GetUIElementCollectionInstance();
            elements.Clear();
            elements.Add(webAuthView);
            webAuthView.Navigate(webAuthView.AuthorizeUrl);
            webAuthView.OAuthSuccess += async (sender, args) =>
            {
                await oAuth.ProcessUserAuthorizationAsync(sender as Uri);
                RestoreView();
                await new MessageDialog("Access Token Received: " + oAuth.AccessToken.Code).ShowAsync();
            };
        }


        private static UIElementCollection GetUIElementCollectionInstance()
        {
            return (((Window.Current.Content as Frame)?.Content as Page)?.Content as Panel)?.Children;
        }

        private static void StoreElements()
        {
            controls = new List<UIElement>();
            if (GetUIElementCollectionInstance() != null)
            {
                foreach (var item in GetUIElementCollectionInstance())
                {
                    controls.Add(item);
                }
            }
        }

        public static void RestoreView()
        {
            var elementCollection = GetUIElementCollectionInstance();
            elementCollection.Clear();
            elementCollection.AddRange(controls);
        }
    }
}
