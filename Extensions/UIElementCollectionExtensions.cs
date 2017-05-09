using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OAuth2Manager.Extensions
{
    public static class UIElementCollectionExtensions
    {
        public static void AddRange(this UIElementCollection collection, List<UIElement> elementsToAdd)
        {
            foreach (var item in elementsToAdd)
            {
                collection.Add(item);
            }
        }
    }
}
