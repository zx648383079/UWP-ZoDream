using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ZoDream.Reader.Model;

namespace ZoDream.Reader.View
{
    public sealed partial class BooksPage: Page
    {
        private ObservableCollection<BookItem> _booksList = new ObservableCollection<BookItem>()
        {
            new BookItem("我真是大明星"),
            new BookItem("超品相师"),
            new BookItem("医统江山"),
            new BookItem("大主宰", ImageKind.CHUANGSHI),
            new BookItem("莽荒纪"),
            new BookItem("斗破苍穹", ImageKind.CHUANGSHI),
            new BookItem("将夜", ImageKind.CHUANGSHI),
            new BookItem("傲世九重天", ImageKind.OTHER),
        };
        /// <summary>
        /// 纪录书架状态 简单版 详细版
        /// </summary>
        private bool _isBlockItem = true;

        public BooksPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            BooksGridView.ItemsSource = _booksList;
        }

        private void ChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            _changMode(BooksGridView.SelectionMode == ListViewSelectionMode.Multiple ?
                ListViewSelectionMode.None : ListViewSelectionMode.Multiple);
        }

        private void _changMode(ListViewSelectionMode mode = ListViewSelectionMode.None)
        {
            if (BooksGridView.SelectionMode == mode)
            {
                return;
            }
            BooksGridView.SelectionMode = mode;
            if (ListViewSelectionMode.Multiple == mode)
            {
                AddBtn.Visibility = ModeBtn.Visibility = Visibility.Collapsed;
                DeleteBtn.Visibility = Visibility.Visible;
            } else
            {
                AddBtn.Visibility = ModeBtn.Visibility = Visibility.Visible;
                DeleteBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void ModeBtn_Click(object sender, RoutedEventArgs e)
        {
            _changMode();
            if (_isBlockItem)
            {
                BooksGridView.Style = Resources["RowGridView"] as Style;
                ((ModeBtn.Icon) as SymbolIcon).Symbol = Symbol.ViewAll;
                ModeBtn.Label = "图标";

            } else
            {
                BooksGridView.Style = Resources["BlockGridView"] as Style;
                ((ModeBtn.Icon) as SymbolIcon).Symbol = Symbol.List;
                ModeBtn.Label = "列表";
            }
            _isBlockItem = !_isBlockItem;
            
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGridView.SelectedIndex < 0)
            {
                return;
            }
            List<BookItem> selectedItems = new List<BookItem>();
            foreach (BookItem item in BooksGridView.SelectedItems)
            {
                selectedItems.Add(item);
            }
            foreach (BookItem item in selectedItems)
            {
                _booksList.Remove(item);
            }
        }

        private void ChooseAllBtn_Click(object sender, RoutedEventArgs e)
        {
            _changMode(ListViewSelectionMode.Multiple);
            BooksGridView.SelectAll();
        }

        private void BooksGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksGridView.SelectionMode == ListViewSelectionMode.Multiple)
            {
                return;
            }
            (Window.Current.Content as Frame).Navigate(typeof(ReadPage), BooksGridView.SelectedItem);
        }
    }
}
