using GalaSoft.MvvmLight.Messaging;
using Microsoft.Data.Sqlite;
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
using Windows.UI.Xaml.Shapes;
using ZoDream.Helper;
using ZoDream.Model;
using ZoDream.Services;
using ZoDreamToolkit;

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
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _book = e.Parameter as Book;
            _getChpaterAsync();
        }

        private void _loadSettings()
        {

        }

        private async Task _getChpaterAsync()
        {
            await SqlHelper.Conn.OpenAsync();
            if (_book.LastChapter > 0)
            {
                _chapter = SqlHelper.First<BookChapter>(_book.LastChapter);
                if (_chapter != null)
                {
                    Pager.PageText = _chapter.Content;
                    TitleTb.Text = _chapter.Name;
                }
                else
                {
                    Toast.ShowError("本书是空的");
                }
                return;
            }
            _chapter = SqlHelper.First<BookChapter>("BookId = @id ORDER BY Position ASC, Id ASC", new SqliteParameter("@id", _book.Id));
            if (_chapter != null)
            {
                Pager.PageText = _chapter.Content;
                TitleTb.Text = _chapter.Name;
                _book.LastChapter = _chapter.Id;
                _book.Save();
            }
            else
            {
                Toast.ShowError("章节不存在");
            }
            SqlHelper.Conn.Close();
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
            SqlHelper.Conn.Open();
            _chapter = SqlHelper.First<BookChapter>("BookId = @id AND Id > @chapter ORDER BY Position ASC, Id ASC",
                new SqliteParameter("@id", _book.Id),
                new SqliteParameter("@chapter", _book.LastChapter));
            if (_chapter != null)
            {
                Pager.PageText = _chapter.Content;
                _book.LastChapter = _chapter.Id;
                TitleTb.Text = _chapter.Name;
                _book.Save();
            }
            else
            {
                Toast.ShowError("没有更多章节了");
            }
            SqlHelper.Conn.Close();
        }

        private void Pager_OnPreviousPage(object sender, EventArgs e)
        {
            SqlHelper.Conn.Open();
            _chapter = SqlHelper.First<BookChapter>("BookId = @id AND Id < @chapter ORDER BY Position DESC, Id DESC",
                new SqliteParameter("@id", _book.Id),
                new SqliteParameter("@chapter", _book.LastChapter));
            if (_chapter != null)
            {
                Pager.PageText = _chapter.Content;
                _book.LastChapter = _chapter.Id;
                TitleTb.Text = _chapter.Name;
                _book.Save();
            }
            else
            {
                Toast.ShowError("已经是最前了");
            }
            SqlHelper.Conn.Close();
        }

        private void Pager_OnIndexChanged(object sender, Layout.IndexEventArgs e)
        {
            PageProgress.Value = (e.Index + 1) * 100 / e.Count;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Pager.Refresh();
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SettingGrid.Visibility == Visibility.Collapsed)
            {
                SettingGrid.Visibility = Visibility.Visible;
                return;
            }
            SettingGrid.Visibility = Visibility.Collapsed;
        }

        private void ChapterBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame root = Window.Current.Content as Frame;
            //这里参数自动装箱
            root.Navigate(typeof(BookChapterPage));
            Messenger.Default.Send(new NotificationMessageAction<Book>(_book, null, item =>
            {

            }), "book");
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            colorPicker.Placement = AdvancedFlyoutPlacementMode.RightCenter;
            colorPicker.PlacementTarget = (sender as FrameworkElement);
            colorPicker.Owner = sender;
            colorPicker.Show();

        }

        private void ColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            if (colorPicker.Owner == null)
            {
                return;
            }
            var rect = colorPicker.Owner as Rectangle;
            rect.Fill = new SolidColorBrush(colorPicker.SelectedColor);
            if (rect.Name == "BackgroundRect")
            {
                Pager.Background = rect.Fill;
                Pager.SetBackground();
            } else
            {
                Pager.Foreground = rect.Fill;
                Pager.SetBlockProperty();
            }
            colorPicker.Owner = null;
        }

        private void ColorPicker_Closed(object sender, object e)
        {
            colorPicker.PlacementTarget = null;
        }

        private void FontSizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Pager.FontSize = (sender as Slider).Value;
            Pager.SetBlockProperty();
        }

        private void LineHeightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Pager.LineHeight = (sender as Slider).Value;
            Pager.SetBlockProperty();
        }

        private void DiffSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Pager.CharacterSpacing = Convert.ToInt32((sender as Slider).Value);
            Pager.SetBlockProperty();
        }

        private void FontBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontBox.SelectedValue.ToString();
        }
    }
}
