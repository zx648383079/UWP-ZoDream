//---------------------------------------------------------------------------
//
// <copyright file="GalleryListPage.xaml.cs" company="Microsoft">
//    Copyright (C) 2015 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <createdOn>8/24/2016 11:30:49 AM</createdOn>
//
//---------------------------------------------------------------------------

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using AppStudio.DataProviders.Flickr;
using ZoDream.Sections;
using ZoDream.ViewModels;
using AppStudio.Uwp;

namespace ZoDream.Pages
{
    public sealed partial class GalleryListPage : Page
    {
	    public ListViewModel ViewModel { get; set; }
        public GalleryListPage()
        {
			ViewModel = ViewModelFactory.NewList(new GallerySection());

            this.InitializeComponent();
			commandBar.DataContext = ViewModel;
			NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
			ShellPage.Current.ShellControl.SelectItem("1b577483-b304-4ab4-8da1-3ab6a54f04d5");
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
