using Microsoft.Data.Sqlite;
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
using ZoDream.Helper;
using ZoDream.Model;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ZoDream.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookReadPage : Page
    {
        private Book _book;

        private BookChapter _chapter;

        public BookReadPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _book = e.Parameter as Book;
            _getChpater();
        }

        private void _getChpater()
        {
            if (_book.ChapterId > 0)
            {
                using (var reader = SqlHelper.ExecuteReader("SELECT * FROM BookChapter WHERE Id = @id LIMIT 1", new SqliteParameter("@id", _book.ChapterId)))
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        _chapter = new BookChapter(reader, true);
                        Pager.PageText = _chapter.Content;
                    }
                    else
                    {
                        _chapter = null;
                    }
                }
                return;
            }
            using (var reader = SqlHelper.ExecuteReader("SELECT * FROM BookChapter WHERE BookId = @id ORDER BY Position ASC, Id ASC LIMIT 1", new SqliteParameter("@id", _book.Id)))
            {
                reader.Read();
                if (reader.HasRows)
                {
                    _chapter = new BookChapter(reader, true);
                    Pager.PageText = _chapter.Content;
                }
                else
                {
                    _chapter = null;
                }
            }
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
            
        }

        private void Pager_OnPreviousPage(object sender, EventArgs e)
        {
            
        }

        private void Pager_OnIndexChanged(object sender, Layout.IndexEventArgs e)
        {
            PageProgress.Value = e.Index * 100 / e.Count;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            
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
