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
using ZoDream.Models;
using ZoDream.Models.Api;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ZoDream.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookChapterPage : Page, ISubPage
    {
        public string NavTitile => BookChapter.PageTitle;

        private BookApi bookApi = new BookApi();

        private BookChapter chapter;

        public BookChapterPage()
        {
            this.InitializeComponent();
        }

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
            chapter = await bookApi.GetChapterAsync(id);
            if (chapter == null)
            {
                return;
            }
            
        }
    }
}
