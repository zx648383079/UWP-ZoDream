using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZoDream.Models;
using ZoDream.Pages;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ZoDream
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ShowStatusBar();
            refreshMenu();
        }

        private void refreshMenu()
        {
            foreach (var item in MenuItem.GetMainItems())
            {
                IconElement icon;
                if (item.Icon.ToLowerInvariant().EndsWith(".png"))
                {
                    icon = new BitmapIcon() { UriSource = new Uri(item.Icon, UriKind.RelativeOrAbsolute) };
                }
                else
                {
                    icon = new FontIcon()
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = item.Icon
                    };
                }
                NavView.MenuItems.Add(new NavigationViewItem() { Icon = icon, Content = item.Name, DataContext = item });
            }
        }

        private async void ShowStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusbar = StatusBar.GetForCurrentView();
                await statusbar.ShowAsync();
                statusbar.BackgroundColor = Colors.White;
                statusbar.BackgroundOpacity = 1;
                statusbar.ForegroundColor = Colors.Black;
            }
        }

        private bool isExit;

        private async void SystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;
            if (rootFrame.CurrentSourcePageType.Name != "MainPage")
            {
                if (rootFrame.CanGoBack && e.Handled == false)
                {
                    e.Handled = true;
                    rootFrame.GoBack();
                }
            }
            else if (e.Handled == false && ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {

                StatusBar statusBar = StatusBar.GetForCurrentView();
                await statusBar.ShowAsync();
                statusBar.ForegroundColor = Colors.White; // 前景色  
                statusBar.BackgroundOpacity = 0.9; // 透明度  
                statusBar.ProgressIndicator.Text = "再按一次返回键退出程序。"; // 文本  
                await statusBar.ProgressIndicator.ShowAsync();

                if (isExit)
                {
                    App.Current.Exit();
                }
                else
                {
                    isExit = true;
                    await Task.Run(async () =>
                    {
                        //Windows.Data.Xml.Dom. XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);  
                        //Windows.Data.Xml.Dom.XmlNodeList elements = toastXml.GetElementsByTagName("text");  
                        //elements[0].AppendChild(toastXml.CreateTextNode("再按一次返回键退出程序。"));  
                        //ToastNotification toast = new ToastNotification(toastXml);  
                        //ToastNotificationManager.CreateToastNotifier().Show(toast);      

                        await Task.Delay(1500);
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            await statusBar.ProgressIndicator.HideAsync();
                            await statusBar.HideAsync();
                        });
                        isExit = false;
                    });
                    e.Handled = true;
                }
            } else
            {
                App.Current.Exit();
            }
        }
        private void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                if (NavView.SelectedItem != NavView.SettingsItem)
                {
                    contentFrame.Navigate(typeof(SettingPage));
                }
            }
            else
            {
                var invokedItem = NavView.MenuItems.Cast<NavigationViewItem>().Single(i => i.Content == args.InvokedItem);

                var menuItem = invokedItem.DataContext as MenuItem;
                if (menuItem.IsNew)
                {
                    Frame.Navigate(menuItem.PageType);
                    return;
                }
                contentFrame.Navigate(menuItem.PageType);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                contentFrame.Navigate(typeof(HomePage));
            }
            base.OnNavigatedTo(e);
        }
    }
}
