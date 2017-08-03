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
using Windows.UI.Core;
using Windows.UI.Input;
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
using ZoDreamToolkit.Common;

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
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                ToolsBtn.Visibility = Visibility.Collapsed;
            }
            _loadSetting();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            var book = e.Parameter as Book;
            if (_book != null && _book.Id == book.Id && _chapter != null && book.LastChapter == _chapter.Id)
            {
                return;
            }
            _book = book;
            _getChpaterAsync();

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
                _book.ReadTime = DateTime.Now;
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
            _nextPage();
        }

        private void _nextPage()
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
                _book.ReadTime = DateTime.Now;
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
            _previousPage();
        }

        private void _previousPage()
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
                _book.ReadTime = DateTime.Now;
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
                AppDataHelper.SetValue("Background", Pager.Background);
            } else
            {
                Pager.Foreground = rect.Fill;
                Pager.SetBlockProperty();
                AppDataHelper.SetValue("Foreground", Pager.Foreground);
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
            AppDataHelper.SetValue("FontSize", Pager.FontSize);
        }

        private void LineHeightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Pager.LineHeight = (sender as Slider).Value;
            Pager.SetBlockProperty();
            AppDataHelper.SetValue("LineHeight", Pager.LineHeight);
        }

        private void DiffSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Pager.CharacterSpacing = Convert.ToInt32((sender as Slider).Value);
            Pager.SetBlockProperty();
            AppDataHelper.SetValue("CharacterSpacing", Pager.CharacterSpacing);
        }

        private void FontBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var font = (FontBox.SelectedItem as ComboBoxItem).Content.ToString();
            Pager.FontFamily = BookHelper.GetFont(font);
            Pager.SetBlockProperty();
            AppDataHelper.SetValue("FontFamily", font);
        }

        private void _loadSetting()
        {
            Pager.FontFamily = BookHelper.GetFont(AppDataHelper.GetValue("FontFamily", "方正启体简体"));
            Pager.CharacterSpacing = AppDataHelper.GetValue("CharacterSpacing", 300);
            Pager.LineHeight = AppDataHelper.GetValue<double>("LineHeight", 36);
            Pager.FontSize = AppDataHelper.GetValue<double>("FontSize", 30);
            Pager.Background = AppDataHelper.GetValue("Background", new SolidColorBrush(ColorHelper.GetColor("#FFE9FAFF")));
            Pager.Foreground = AppDataHelper.GetValue("Background", new SolidColorBrush(ColorHelper.GetColor("#FF555555")));
            Pager.SetProperty();
        }

        private void Pager_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.A:
                case Windows.System.VirtualKey.Left:
                    Pager.GoBack();
                    break;
                case Windows.System.VirtualKey.D:
                case Windows.System.VirtualKey.Right:
                    Pager.GoForword();
                    break;
                case Windows.System.VirtualKey.PageUp:
                    _previousPage();
                    break;
                case Windows.System.VirtualKey.PageDown:
                    _nextPage();
                    break;
                default:
                    break;
            }
        }

        private double _isLeft;

        private void Pager_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _isLeft = e.Delta.Translation.X;
        }

        private void Pager_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_isLeft < 0)
            {
                Pager.GoForword();
                return;
            }
            Pager.GoBack();
        }

        private void Pager_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var x = e.GetPosition(Pager).X;
            if (x < Pager.Width / 2)
            {
                Pager.GoBack();
                return;
            }
            Pager.GoForword();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ToolsBtn.Visibility == Visibility.Visible)
            {
                ToolsBtn.SetValue(Canvas.TopProperty, e.NewSize.Height - 200);
                ToolsBtn.SetValue(Canvas.LeftProperty, e.NewSize.Width - 100);
            }
        }

        private void ToolsBtn_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            PointerPoint point = e.GetCurrentPoint(ToolsBtn);

            if (point.Properties.IsLeftButtonPressed)
            {
                double x = (double)ToolsBtn.GetValue(Canvas.LeftProperty);
                double y = (double)ToolsBtn.GetValue(Canvas.TopProperty);
                x += point.Position.X - ToolsBtn.ActualWidth / 2.0;
                y += point.Position.Y - ToolsBtn.ActualHeight / 2.0;
                ToolsBtn.SetValue(Canvas.LeftProperty, x);
                ToolsBtn.SetValue(Canvas.TopProperty, y);
            }
        }

        private void ToolsBtn_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

        }
    }
}
