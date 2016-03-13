using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ZoDream.Reader.Model;
using ZoDream.Reader.View;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace ZoDream.Reader
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool isExit = false;

        private List<MenuItem> _menuList = new List<MenuItem>()
        {
            new MenuItem("首页", Symbol.Home, typeof(BooksPage)),
            new MenuItem("搜书", Symbol.Find, typeof(SearchPage)),
            new MenuItem("浏览器", Symbol.Globe, typeof(WebPage)),
            new MenuItem("设置", Symbol.Setting, typeof(SettingPage))
        };

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MenuListBox.ItemsSource = _menuList;
            MenuListBox.SelectedIndex = 0;
            if (Window.Current.Bounds.Width < 640)
            {

            }
        }

        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScenarioFrame.Navigate((MenuListBox.SelectedItem as MenuItem).PageType);
        }
    }
}
