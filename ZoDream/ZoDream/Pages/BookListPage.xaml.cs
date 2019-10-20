using System;
using System.Collections.Generic;
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
using ZoDream.Models;
using ZoDream.Models.Api;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ZoDream.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookListPage : Page, ISubPage
    {
        private IncrementalLoadingCollection<Book> Books;

        private uint page = 0;

        private BookApi bookApi = new BookApi();

        public string NavTitile => "小说";

        public BookListPage()
        {
            this.InitializeComponent();
            Books = new IncrementalLoadingCollection<Book>(count => {
                page++;
                return bookApi.GetListAsync(page);
            });
            ListView.ItemsSource = Books;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Refresh();
        }

        private async void Refresh()
        {
            Books.Clear();
            page = 0;
            await Books.LoadMoreItemsAsync(20);
        }

        private async Task Fetch(uint page)
        {
            LoadingRing.IsActive = true;

            var data = await bookApi.GetListAsync(page);
            if (data.Item1 != null)
            {
                foreach (var blog in data.Item1)
                {
                    Books.Add(blog);
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
            if (ListView.SelectedItem is Book book)
            {
                Book.PageTitle = book.Name;
                Frame.Navigate(typeof(BookPage), book.Id);
            };
        }
        
    }
}
