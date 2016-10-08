//---------------------------------------------------------------------------
//
// <copyright file="ConcertsListPage.xaml.cs" company="Microsoft">
//    Copyright (C) 2015 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <createdOn>8/24/2016 11:30:49 AM</createdOn>
//
//---------------------------------------------------------------------------

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using AppStudio.DataProviders.LocalStorage;
using ZoDream.Sections;
using ZoDream.ViewModels;
using AppStudio.Uwp;

namespace ZoDream.Pages
{
    public sealed partial class ConcertsListPage : Page
    {
		public GroupedListViewModel ViewModel { get; set; }
        public ConcertsListPage()
        {
			ViewModel = ViewModelFactory.NewListGrouped(new ConcertsSection());

            this.InitializeComponent();
			commandBar.DataContext = ViewModel;
			NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
			ShellPage.Current.ShellControl.SelectItem("c21d989c-340a-492e-bea9-1f801ba416a7");
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
