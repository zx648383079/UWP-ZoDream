using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using ZoDream.Core;
using ZoDream.Pages;

namespace ZoDream.Models
{
    public class MenuItem
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }
        /// <summary>
        /// 是否打开新的页面
        /// </summary>
        public bool IsNew { get; set; } = false;

        public static List<MenuItem> GetMainItems()
        {
            var items = new List<MenuItem>
            {
                new MenuItem() { Icon = "\uE80F", Name = "首页", PageType = typeof(HomePage) },
                new MenuItem() { Icon = "\uE82D", Name = "小说", PageType = typeof(BookListPage)},
                new MenuItem() { Icon = "\uE156", Name = "扫一扫", PageType = typeof(ScanPage)},
            };
            items.Add(Configs.NewInstance().IsGuest()
                ? new MenuItem() {Icon = "\uE2AF", Name = "登录", PageType = typeof(LoginPage)}
                : new MenuItem() {Icon = "\uE2AF", Name = "已登录", PageType = typeof(ProfilePage)});
            return items;
        }

        public delegate void RefreshMenuHandle();

        public static event RefreshMenuHandle RefreshMenuEvent;

        public static void TriggerRefreshMenuEvent()
        {
            RefreshMenuEvent();
        }
    }
}
