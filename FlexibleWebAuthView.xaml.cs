using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OAuth2Manager.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OAuth2Manager
{
    public sealed partial class FlexibleWebAuthView : UserControl, IUserAuthorizationViewer
    {

        public EventHandler OAuthCancelled { get; set; }
        public EventHandler OAuthSuccess { get; set; }
        public EventHandler NavigationFailed { get; set; }


        public FlexibleWebAuthView()
        {
            this.InitializeComponent();
            FlexibleWebAuthViewControl.Focus(FocusState.Pointer);
            this.InitEvents();
        }

        private void InternalWebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            NavigationFailed?.Invoke(e.Uri, EventArgs.Empty);
        }

        private void InitEvents()
        {
            this.Loaded += FlexibleWebAuthView_Loaded;
            InternalWebView.NavigationFailed += InternalWebView_NavigationFailed;
            InternalWebView.NavigationStarting += InternalWebView_NavigationStarting;
        }

        private void InternalWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (AuthController.IsCallBack(args.Uri))
                OAuthSuccess?.Invoke(args.Uri, EventArgs.Empty);
        }

        private void FlexibleWebAuthView_Loaded(object sender, RoutedEventArgs e)
        {
            InternalWebView.Width = 800.0f;
        }

        public void Navigate(Uri uri)
        {
            InternalWebView.Navigate(uri);
        }

        public Uri GetCurrentUri()
        {
            return InternalWebView.Source;
        }

        public Uri AuthorizeUrl { get; set; }
        public IUserConsentHandler AuthController { get; set; }
    }
}
