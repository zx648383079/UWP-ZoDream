//---------------------------------------------------------------------------
//
// <copyright file="AlbumsListPage.xaml.cs" company="Microsoft">
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
    public sealed partial class AlbumsListPage : Page
    {
	    public ListViewModel ViewModel { get; set; }
        public AlbumsListPage()
        {
			ViewModel = ViewModelFactory.NewList(new AlbumsSection());

            this.InitializeComponent();
			commandBar.DataContext = ViewModel;
			NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
			ShellPage.Current.ShellControl.SelectItem("d55a8638-f991-4fb7-8c16-9d780f28b927");
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
