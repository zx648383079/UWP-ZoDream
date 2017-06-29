using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZoDream.Helper;
using ZoDreamToolkit.Common;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace ZoDream.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ReadPage : Page
    {
        public ReadPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _currentUrl = (Uri)e.Parameter;
            GetHtml();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Pager.GoBack();
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            Pager.GoForword();
        }

        private void Pager_OnNextPage(object sender, EventArgs e)
        {
            _currentUrl = _nextUrl;
            GetHtml();
        }

        private void Pager_OnPreviousPage(object sender, EventArgs e)
        {
            _currentUrl = _previousUrl;
            GetHtml();
        }

        private void Pager_OnIndexChanged(object sender, Layout.IndexEventArgs e)
        {
            PageProgress.Value = e.Index * 100 / e.Count;
        }

        private Uri _nextUrl;

        private Uri _currentUrl;

        private Uri _previousUrl;


        public async void GetHtml()
        {
            if (_currentUrl == null)
            {
                return;
            }
            var html = await HttpHelper.GetAsync(_currentUrl);
            _getUrl(html);
            TitleTb.Text = Regex.Match(html, @"\<title\>([^\<\>]+)\</title\>").Groups[1].Value;
            Pager.PageHtml = Regex.Match(html, @"\<div class=""con""\>[\s\S]+\<div class=""func""\>").Value;
            //await Http.Get(Regex.Match(html, @"\<img src=""(http://irs01\.com[^""]+)").Groups[1].Value, false);
        }

        private void _getUrl(string html)
        {
            string a = null, b = null, c = null, d = null;
            var matches = Regex.Matches(html, @"\<a[^\<\>]+href=""([^""]+)""[^\<\>]*\>(上|下)一(页|章)\</a\>");
            foreach (Match item in matches)
            {
                if (item.Groups[2].Value == "上")
                {
                    if (item.Groups[3].Value == "页")
                    {
                        a = item.Groups[1].Value;
                    } else
                    {
                        b = item.Groups[1].Value;
                    }
                } else
                {
                    if (item.Groups[3].Value == "页")
                    {
                        c = item.Groups[1].Value;
                    }
                    else
                    {
                        d = item.Groups[1].Value;
                    }
                }
            }
            if (!string.IsNullOrEmpty(a))
            {
                _previousUrl = new Uri(_currentUrl, a.Replace("&amp;", "&"));
            } else if (!string.IsNullOrEmpty(b))
            {
                _previousUrl = new Uri(_currentUrl, b.Replace("&amp;", "&"));
            } else
            {
                _previousUrl = null;
            }
            if (!string.IsNullOrEmpty(c))
            {
                _nextUrl = new Uri(_currentUrl, c.Replace("&amp;", "&"));
            }
            else if (!string.IsNullOrEmpty(d))
            {
                _nextUrl = new Uri(_currentUrl, d.Replace("&amp;", "&"));
            }
            else
            {
                _nextUrl = null;
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            GetHtml();
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SettingGrid.Visibility == Visibility.Collapsed)
            {
                SettingGrid.Visibility = Visibility.Visible;
                return;
            }
            SettingGrid.Visibility = Visibility.Visible;
        }
    }
}
