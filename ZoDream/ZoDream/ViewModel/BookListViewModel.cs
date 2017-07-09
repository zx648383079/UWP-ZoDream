using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Model;
using GalaSoft.MvvmLight.Views;
using ZoDream.Helper;
using ZoDream.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.View;
using ZoDreamToolkit.Common;
using Windows.Storage;

namespace ZoDream.ViewModel
{
    public class BookListViewModel: BaseViewModel
    {

        public BookListViewModel(INavigationService navigationService) : base(navigationService)
        {
            _refresh();
        }

        private void _refresh()
        {
            BookList.Clear();
            SqlHelper.Conn.Open();
            using (var reader = SqlHelper.Select<Book>("ORDER BY ReadTime DESC"))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        BookList.Add(new Book(reader));
                    }
                }
            }
            SqlHelper.Conn.Close();
        }

        /// <summary>
        /// The <see cref="BookList" /> property's name.
        /// </summary>
        public const string BookListPropertyName = "BookList";

        private ObservableCollection<Book> _bookList = new ObservableCollection<Book>();

        /// <summary>
        /// Sets and gets the BookList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Book> BookList
        {
            get
            {
                return _bookList;
            }
            set
            {
                Set(BookListPropertyName, ref _bookList, value);
            }
        }

        /// <summary>
        /// The <see cref="EditMode" /> property's name.
        /// </summary>
        public const string EditModePropertyName = "EditMode";

        private bool _editMode = false;

        /// <summary>
        /// Sets and gets the EditMode property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                Set(EditModePropertyName, ref _editMode, value);
            }
        }

        private RelayCommand _editModeCommand;

        /// <summary>
        /// Gets the EditModeCommand.
        /// </summary>
        public RelayCommand EditModeCommand
        {
            get
            {
                return _editModeCommand
                    ?? (_editModeCommand = new RelayCommand(ExecuteEditModeCommand));
            }
        }

        private void ExecuteEditModeCommand()
        {
            EditMode = !EditMode;
            foreach (var item in BookList)
            {
                item.EditMode = EditMode;
            }
        }

        private RelayCommand<int> _readCommand;

        /// <summary>
        /// Gets the ReadCommand.
        /// </summary>
        public RelayCommand<int> ReadCommand
        {
            get
            {
                return _readCommand
                    ?? (_readCommand = new RelayCommand<int>(ExecuteReadCommand));
            }
        }

        private void ExecuteReadCommand(int index)
        {
            if (index < 0 || index >= BookList.Count) return;
            var book = BookList[index];
            if (EditMode)
            {
                book.IsChecked = !book.IsChecked;
            } else
            {
                NavigationService.NavigateTo(typeof(BookReadPage).FullName, book);
            }
            //Messenger.Default.Send(new NotificationMessageAction<Book>(BookList[index], null, item =>
            //{

            //}), "book");
        }

        private RelayCommand _httpCommand;

        /// <summary>
        /// Gets the HttpCommand.
        /// </summary>
        public RelayCommand HttpCommand
        {
            get
            {
                return _httpCommand
                    ?? (_httpCommand = new RelayCommand(ExecuteHttpCommand));
            }
        }

        private void ExecuteHttpCommand()
        {
            NavigationService.NavigateTo(typeof(HttpPage).FullName);
            Messenger.Default.Send(new NotificationMessageAction<Book>(null, null, item =>
            {
                BookList.Add(item);
            }), "http");
        }

        private RelayCommand _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand(ExecuteAddCommand));
            }
        }

        private void ExecuteAddCommand()
        {
            _addBookAsync();
        }

        private RelayCommand _removeCommand;

        /// <summary>
        /// Gets the RemoveCommand.
        /// </summary>
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand
                    ?? (_removeCommand = new RelayCommand(ExecuteRemoveCommand));
            }
        }

        private void ExecuteRemoveCommand()
        {
            SqlHelper.Conn.Open();
            for (var i = BookList.Count - 1; i >= 0; i--)
            {
                if (BookList[i].IsChecked)
                {
                    BookList[i].Delete();
                    BookList.RemoveAt(i);
                }
            }
            SqlHelper.Conn.Close();
        }

        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));
            }
        }

        private void ExecuteSaveCommand()
        {
            _saveBooks();
        }

        private RelayCommand _refreshCommand;

        /// <summary>
        /// Gets the RefreshCommand.
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                    ?? (_refreshCommand = new RelayCommand(ExecuteRefreshCommand));
            }
        }

        private void ExecuteRefreshCommand()
        {
            _refresh();
        }

        private async Task _saveBooks()
        {
            var folder = await StorageHelper.OpenFolderAsync();
            await SqlHelper.Conn.OpenAsync();
            for (var i = BookList.Count - 1; i >= 0; i--)
            {
                var book = BookList[i];
                if (book.IsChecked)
                {
                    var file = await folder.CreateFileAsync(book.Name + ".txt", CreationCollisionOption.ReplaceExisting);
                    await BookHelper.SaveAsync(book, file);
                }
            }
            SqlHelper.Conn.Close();
        }

        private async Task _addBookAsync()
        {
            var books = await BookHelper.OpenAsync();
            if (books == null)
            {
                return;
            }
            books.ForEach(book =>
            {
                BookList.Add(book);
            });
            NavigationService.NavigateTo(typeof(BookReadPage).FullName, books[0]);
        }


    }
}
