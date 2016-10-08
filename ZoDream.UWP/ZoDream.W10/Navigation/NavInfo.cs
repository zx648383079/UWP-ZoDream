using AppStudio.DataProviders.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ZoDream.Navigation
{
    public class NavInfo
    {
        public string TargetPage { get; set; }

        public Uri TargetUri { get; set; }

        public NavType NavigationType { get; set; }
        public bool IsDetail { get; set; }

        public static NavInfo FromPage<T>(bool isDetail = false) where T : Page
        {
            return new NavInfo
            {
                NavigationType = NavType.Page,
                TargetPage = typeof(T).Name,
                IsDetail = isDetail
            };
        }

        public static NavInfo FromMenu(MenuSchema menuItem)
        {
            if (menuItem == null)
            {
                return null;
            }

            var NavInfo = new NavInfo
            {
                NavigationType = SafeParse(menuItem.MenuType)
            };

            if (NavInfo.NavigationType == NavType.Page)
            {
                NavInfo.TargetPage = menuItem.Target;
            }
            else
            {
                NavInfo.TargetUri = new Uri(menuItem.Target, UriKind.Absolute);
            }

            return NavInfo;
        }

        private static NavType SafeParse(string value)
        {
            var type = NavType.Page;
            Enum.TryParse(value, out type);

            return type;
        }
    }

    public enum NavType
    {
        Page,
        DeepLink
    }
}