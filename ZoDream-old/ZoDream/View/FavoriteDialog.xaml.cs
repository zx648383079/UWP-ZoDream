using System;
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
using ZoDream.Model;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace ZoDream.View
{
    public sealed partial class FavoriteDialog : ContentDialog
    {
        public FavoriteDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(TitleTb.Text) || string.IsNullOrWhiteSpace(UrlTb.Text))
            {
                args.Cancel = true;
                return;
            }
            _url.Name = TitleTb.Text;
            _url.Url = UrlTb.Text;
            //this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //this.Hide();
        }

        private FavoriteUrl _url = new FavoriteUrl("", "http://");

        public FavoriteUrl Url
        {
            get { return _url; }
            set {
                _url = value;
                TitleTb.Text = _url.Name;
                UrlTb.Text = _url.Url;
            }
        }

    }
}
