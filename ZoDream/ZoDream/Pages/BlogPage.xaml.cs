﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZoDream.Models;
using ZoDream.Models.Api;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ZoDream.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BlogPage : Page, ISubPage
    {
        private Blog blog;

        private BlogApi blogApi = new BlogApi();

        public BlogPage()
        {
            this.InitializeComponent();
        }

        public string NavTitile => Blog.PageTitle;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                Refresh(Convert.ToUInt32(e.Parameter));
            }
        }

        private async void Refresh(uint id)
        {
            blog = await blogApi.GetBlogAsync(id);
            if (blog == null)
            {
                return;
            }
            var data = await blogApi.GetContentAsync(id);
            BlogContent.NavigateToString(data.Content);
            TitleTb.Text = blog.Title;
            AuthorTb.Label = blog.User.Name;
            TermTb.Label = blog.Term.Name;
            LangTb.Label = blog.ProgrammingLanguage;
            TimeTb.Label = blog.CreatedAt;
            CommentTb.Label = blog.CommentCount.ToString();
            RemTb.Label = blog.Recommend.ToString();
            ViewTb.Label = blog.ClickCount.ToString();
        }
    }
}
