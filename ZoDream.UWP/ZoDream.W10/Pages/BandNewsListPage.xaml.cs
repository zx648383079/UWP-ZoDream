//---------------------------------------------------------------------------
//
// <copyright file="BandNewsListPage.xaml.cs" company="Microsoft">
//    Copyright (C) 2015 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <createdOn>8/24/2016 11:30:49 AM</createdOn>
//
//---------------------------------------------------------------------------

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using AppStudio.DataProviders.Rss;
using ZoDream.Sections;
using ZoDream.ViewModels;
using AppStudio.Uwp;

namespace ZoDream.Pages
{
    public sealed partial class BandNewsListPage : Page
    {
	    public ListViewModel ViewModel { get; set; }
        public BandNewsListPage()
        {
			ViewModel = ViewModelFactory.NewList(new BandNewsSection());

            this.InitializeComponent();
			commandBar.DataContext = ViewModel;
			NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
			ShellPage.Current.ShellControl.SelectItem("0e5e5d68-01fb-4a70-9d31-f55d4323975b");
			ShellPage.Current.ShellControl.SetCommandBar(commandBar);
			if (e.NavigationMode == NavigationMode.New)
            {			
				await this.ViewModel.LoadDataAsync();
                this.ScrollToTop();
			}			
            base.OnNavigatedTo(e);
        }

    }
}
