﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZoDream.Helpers;
using ZoDream.Models;
using ZoDream.Models.Api;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ZoDream.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page, ISubPage
    {
        private IncrementalLoadingCollection<Blog> Blogs;

        private uint page = 0;

        private BlogApi blogApi = new BlogApi();

        public string NavTitile => "博客";

        public HomePage()
        {
            this.InitializeComponent();
            Blogs = new IncrementalLoadingCollection<Blog>(count => {
                return Task.Run(async () =>
                {
                    page++;
                    return await blogApi.GetListAsync(page);
                });
            });
            ListView.ItemsSource = Blogs;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Refresh();
        }

        private async void Refresh()
        {
            Blogs.Clear();
            page = 0;
            await Blogs.LoadMoreItemsAsync(20);
        }

        private async Task Fetch(uint page)
        {
            LoadingRing.IsActive = true;

            var data = await blogApi.GetListAsync(page);
            if (data.Item1 != null)
            {
                foreach (var blog in data.Item1)
                {
                    Blogs.Add(blog);
                }
            }
            LoadingRing.IsActive = false;
        }

        private void RefreshContainer_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            Refresh();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListView.SelectedItem is Blog blog)
            {
                Blog.PageTitle = blog.Title;
                Frame.Navigate(typeof(BlogPage), blog.Id);
            };
        }
    }
}
