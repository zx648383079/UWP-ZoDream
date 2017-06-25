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

namespace ZoDream.ViewModel
{
    public class BookListViewModel: BaseViewModel
    {

        public BookListViewModel(INavigationService navigationService) : base(navigationService)
        {
            SqlHelper.Conn.Open();
            using (var reader = SqlHelper.Select<Book>("ORDER BY ReadTime DESC")) { 
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
            for (int i = BookList.Count - 1; i >= 0; i--)
            {
                if (BookList[i].IsChecked)
                {
                    BookList[i].Delete();
                    BookList.RemoveAt(i);
                }
            }
            SqlHelper.Conn.Close();
        }

        private async Task _addBookAsync()
        {
            var book = await BookHelper.OpenAsync();
            if (book != null)
            {
                BookList.Add(book);
                NavigationService.NavigateTo(typeof(BookReadPage).FullName, book);
            }
        }





    }
}
