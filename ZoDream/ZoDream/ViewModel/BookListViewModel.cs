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
    public class BookListViewModel:BaseViewModel
    {

        public BookListViewModel(INavigationService navigationService) : base(navigationService)
        {
            using (var reader = SqlHelper.ExecuteReader("SELECT * FROM Book ORDER BY ReadTime DESC")) { 
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        BookList.Add(new Book(reader));
                    }
                }
            }
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
            NavigationService.NavigateTo(typeof(BookReadPage).FullName, BookList[index]);
            //Messenger.Default.Send(new NotificationMessageAction<Book>(BookList[index], null, item =>
            //{

            //}), "book");
        }

    }
}
