//---------------------------------------------------------------------------
//
// <copyright file="HomePage.xaml.cs" company="Microsoft">
//    Copyright (C) 2015 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <createdOn>8/24/2016 11:30:49 AM</createdOn>
//
//---------------------------------------------------------------------------

using System.Windows.Input;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;

using AppStudio.Uwp;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Navigation;

using ZoDream.ViewModels;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI;
using System;
using Windows.UI.Core;
using System.Threading.Tasks;

namespace ZoDream.Pages
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            ViewModel = new MainViewModel(12);            
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
			commandBar.DataContext = ViewModel;
			searchBox.SearchCommand = SearchCommand;
			this.SizeChanged += OnSizeChanged;
            ShowStatusBar();

            var m = SystemNavigationManager.GetForCurrentView();
            m.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            m.BackRequested += M_BackRequested;
           
        }

        private bool isExit = false;

        private async void M_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;
            if (rootFrame.CurrentSourcePageType.Name != "HomePage")
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

        public MainViewModel ViewModel { get; set; }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.ViewModel.LoadDataAsync();
			//Page cache requires set commandBar in code
			ShellPage.Current.ShellControl.SetCommandBar(commandBar);
            ShellPage.Current.ShellControl.SelectItem("Home");
        }

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            searchBox.SearchWidth = e.NewSize.Width > 640 ? 230 : 190;
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

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand<string>(text =>
                {
                    searchBox.Reset();
                    ShellPage.Current.ShellControl.CloseLeftPane();                    
                    NavigationService.NavigateToPage("SearchPage", text, true);
                },
                SearchViewModel.CanSearch);
            }
        }
    }
}
