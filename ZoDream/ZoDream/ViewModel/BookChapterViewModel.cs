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
using Microsoft.Data.Sqlite;

namespace ZoDream.ViewModel
{
    public class BookChapterViewModel:BaseViewModel
    {
        private NotificationMessageAction<Book> _showBook;

        public BookChapterViewModel(INavigationService navigationService) : base(navigationService)
        {
            Messenger.Default.Register<NotificationMessageAction<Book>>(this, "book", m =>
            {
                _showBook = m;
                Book = m.Sender as Book;
            });
            using (var reader = SqlHelper.ExecuteReader("SELECT Id, BookId, Name FROM Book WHERE BookId = @id ORDER BY Position ASC, Id ASC", new SqliteParameter("@id", Book.Id))) { 
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        ChapterList.Add(new BookChapter(reader));
                    }
                }
            }
        }

        /// <summary>
        /// The <see cref="Book" /> property's name.
        /// </summary>
        public const string BookPropertyName = "Book";

        private Book _book = null;

        /// <summary>
        /// Sets and gets the Book property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Book Book
        {
            get
            {
                return _book;
            }
            set
            {
                Set(BookPropertyName, ref _book, value);
            }
        }

        /// <summary>
            /// The <see cref="ChapterList" /> property's name.
            /// </summary>
        public const string ChapterListPropertyName = "ChapterList";

        private ObservableCollection<BookChapter> _chapterList = new ObservableCollection<BookChapter>();

        /// <summary>
        /// Sets and gets the ChapterList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<BookChapter> ChapterList
        {
            get
            {
                return _chapterList;
            }
            set
            {
                Set(ChapterListPropertyName, ref _chapterList, value);
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
            if (index < 0 || index >= ChapterList.Count) return;
            Book.ChapterId = ChapterList[index].Id;
            NavigationService.NavigateTo(typeof(BookReadPage).FullName, Book);
            //Messenger.Default.Send(new NotificationMessageAction<BookChapter>(ChapterList[index], null, item =>
            //{
                
            //}), "read");
            //Messenger.Default.Send(new NotificationMessageAction<Book>(Book, null, item =>
            //{

            //}), "book");
        }


    }
}
