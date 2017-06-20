using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Model;
using GalaSoft.MvvmLight.Views;

namespace ZoDream.ViewModel
{
    public class BookListViewModel:BaseViewModel
    {

        public BookListViewModel(INavigationService navigationService) : base(navigationService)
        {
            BookList.Add(new Book()
            {
                Name = "12312321"
            });
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
        

    }
}
