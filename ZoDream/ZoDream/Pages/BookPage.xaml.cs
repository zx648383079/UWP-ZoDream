using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class BookPage : Page,ISubPage
    {
        public string NavTitile => Book.PageTitle;

        private BookApi bookApi = new BookApi();

        private Book book;

        private ObservableCollection<BookChapter> chapters = new ObservableCollection<BookChapter>(); 

        public BookPage()
        {
            this.InitializeComponent();
            ListView.ItemsSource = chapters;
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
            chapters.Clear();
            book = await bookApi.GetBook(id);
            if (book == null)
            {
                return;
            }
            NameTb.Text = book.Name;
            DescTb.Text = book.Description;
            var data = await bookApi.GetChapterListAsync(book.Id);
            if (data == null)
            {
                return;
            }
            foreach (var item in data)
            {
                chapters.Add(item);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListView.SelectedItem is BookChapter chapter)
            {
                BookChapter.PageTitle = chapter.Title;
                Frame.Navigate(typeof(BookChapterPage), chapter.Id);
            };
        }
    }
}
