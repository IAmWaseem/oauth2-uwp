using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OAuth2Manager.Utils
{
    public class XamlWindowUtils
    {
        public static async Task TryShowNewWindowAsync(Page content, bool switchToView)
        {
            var newView = CoreApplication.CreateNewView();
            var newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Window.Current.Content = content;

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            ApplicationViewSwitcher.DisableShowingMainViewOnActivation();
            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            if (switchToView && viewShown)
            {
                await ApplicationViewSwitcher.SwitchAsync(newViewId);
            }
        }
    }
}
