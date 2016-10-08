using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;
using ZoDream.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Threading.Tasks;

namespace ZoDream
{
    public sealed partial class MainPage
    {
        public MainViewModel Vm => (MainViewModel)DataContext;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            ShowStatusBar();
            var m = SystemNavigationManager.GetForCurrentView();
            m.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            m.BackRequested += SystemNavigationManagerBackRequested;
            Loaded += (s, e) =>
            {
                Vm.RunClock();
            };
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
            else if (e.Handled == false)
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
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Vm.StopClock();
            base.OnNavigatingFrom(e);
        }
    }
}
