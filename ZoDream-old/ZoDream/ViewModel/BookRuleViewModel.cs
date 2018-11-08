using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Command;
using ZoDream.Helper;
using ZoDream.Model;
using Microsoft.Data.Sqlite;
using ZoDream.View;

namespace ZoDream.ViewModel
{
    public class BookRuleViewModel : BaseViewModel
    {
        public BookRuleViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        /// <summary>
        /// The <see cref="Host" /> property's name.
        /// </summary>
        public const string HostPropertyName = "Host";

        private string _host = string.Empty;

        /// <summary>
        /// Sets and gets the Host property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                Set(HostPropertyName, ref _host, value);
            }
        }

        /// <summary>
        /// The <see cref="NameStart" /> property's name.
        /// </summary>
        public const string NameStartPropertyName = "NameStart";

        private string _nameStart = string.Empty;

        /// <summary>
        /// Sets and gets the NameStart property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string NameStart
        {
            get
            {
                return _nameStart;
            }
            set
            {
                Set(NameStartPropertyName, ref _nameStart, value);
            }
        }

        /// <summary>
        /// The <see cref="NameEnd" /> property's name.
        /// </summary>
        public const string NameEndPropertyName = "NameEnd";

        private string _nameEnd = string.Empty;

        /// <summary>
        /// Sets and gets the NameEnd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string NameEnd
        {
            get
            {
                return _nameEnd;
            }
            set
            {
                Set(NameEndPropertyName, ref _nameEnd, value);
            }
        }

        /// <summary>
        /// The <see cref="AuthorStart" /> property's name.
        /// </summary>
        public const string AuthorStartPropertyName = "AuthorStart";

        private string _authorStart = string.Empty;

        /// <summary>
        /// Sets and gets the AuthorStart property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string AuthorStart
        {
            get
            {
                return _authorStart;
            }
            set
            {
                Set(AuthorStartPropertyName, ref _authorStart, value);
            }
        }

        /// <summary>
        /// The <see cref="AuthorEnd" /> property's name.
        /// </summary>
        public const string AuthorEndPropertyName = "AuthorEnd";

        private string _authorEnd = string.Empty;

        /// <summary>
        /// Sets and gets the AuthorEnd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string AuthorEnd
        {
            get
            {
                return _authorEnd;
            }
            set
            {
                Set(AuthorEndPropertyName, ref _authorEnd, value);
            }
        }

        /// <summary>
        /// The <see cref="CoverStart" /> property's name.
        /// </summary>
        public const string CoverStartPropertyName = "CoverStart";

        private string _coverStart = string.Empty;

        /// <summary>
        /// Sets and gets the CoverStart property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CoverStart
        {
            get
            {
                return _coverStart;
            }
            set
            {
                Set(CoverStartPropertyName, ref _coverStart, value);
            }
        }

        /// <summary>
        /// The <see cref="CoverEnd" /> property's name.
        /// </summary>
        public const string CoverEndPropertyName = "CoverEnd";

        private string _coverEnd = string.Empty;

        /// <summary>
        /// Sets and gets the CoverEnd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CoverEnd
        {
            get
            {
                return _coverEnd;
            }
            set
            {
                Set(CoverEndPropertyName, ref _coverEnd, value);
            }
        }

        /// <summary>
        /// The <see cref="DescriptionStart" /> property's name.
        /// </summary>
        public const string DescriptionStartPropertyName = "DescriptionStart";

        private string _descriptionStart = string.Empty;

        /// <summary>
        /// Sets and gets the DescriptionStart property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DescriptionStart
        {
            get
            {
                return _descriptionStart;
            }
            set
            {
                Set(DescriptionStartPropertyName, ref _descriptionStart, value);
            }
        }

        /// <summary>
        /// The <see cref="DescriptionEnd" /> property's name.
        /// </summary>
        public const string DescriptionEndPropertyName = "DescriptionEnd";

        private string _descriptionEnd = string.Empty;

        /// <summary>
        /// Sets and gets the DescriptionEnd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DescriptionEnd
        {
            get
            {
                return _descriptionEnd;
            }
            set
            {
                Set(DescriptionEndPropertyName, ref _descriptionEnd, value);
            }
        }

        /// <summary>
        /// The <see cref="ListStart" /> property's name.
        /// </summary>
        public const string ListStartPropertyName = "ListStart";

        private string _listStart = string.Empty;

        /// <summary>
        /// Sets and gets the ListStart property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ListStart
        {
            get
            {
                return _listStart;
            }
            set
            {
                Set(ListStartPropertyName, ref _listStart, value);
            }
        }

        /// <summary>
        /// The <see cref="ListEnd" /> property's name.
        /// </summary>
        public const string ListEndPropertyName = "ListEnd";

        private string _listEnd = string.Empty;

        /// <summary>
        /// Sets and gets the ListEnd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ListEnd
        {
            get
            {
                return _listEnd;
            }
            set
            {
                Set(ListEndPropertyName, ref _listEnd, value);
            }
        }

        /// <summary>
        /// The <see cref="TitleStart" /> property's name.
        /// </summary>
        public const string TitleStartPropertyName = "TitleStart";

        private string _titleStart = string.Empty;

        /// <summary>
        /// Sets and gets the TitleStart property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string TitleStart
        {
            get
            {
                return _titleStart;
            }
            set
            {
                Set(TitleStartPropertyName, ref _titleStart, value);
            }
        }

        /// <summary>
        /// The <see cref="TitleEnd" /> property's name.
        /// </summary>
        public const string TitleEndPropertyName = "TitleEnd";

        private string _titleEnd = string.Empty;

        /// <summary>
        /// Sets and gets the TitleEnd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string TitleEnd
        {
            get
            {
                return _titleEnd;
            }
            set
            {
                Set(TitleEndPropertyName, ref _titleEnd, value);
            }
        }

        /// <summary>
        /// The <see cref="ContentStart" /> property's name.
        /// </summary>
        public const string ContentStartPropertyName = "ContentStart";

        private string _contentStart = string.Empty;

        /// <summary>
        /// Sets and gets the ContentStart property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ContentStart
        {
            get
            {
                return _contentStart;
            }
            set
            {
                Set(ContentStartPropertyName, ref _contentStart, value);
            }
        }

        /// <summary>
        /// The <see cref="ContentEnd" /> property's name.
        /// </summary>
        public const string ContentEndPropertyName = "ContentEnd";

        private string _contentEnd = string.Empty;

        /// <summary>
        /// Sets and gets the ContentEnd property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ContentEnd
        {
            get
            {
                return _contentEnd;
            }
            set
            {
                Set(ContentEndPropertyName, ref _contentEnd, value);
            }
        }

        private RelayCommand _openCommand;

        /// <summary>
        /// Gets the OpenCommand.
        /// </summary>
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand
                    ?? (_openCommand = new RelayCommand(ExecuteOpenCommand));
            }
        }

        private void ExecuteOpenCommand()
        {
            if (string.IsNullOrWhiteSpace(Host))
            {
                return;
            }
            SqlHelper.Conn.Open();
            var book = SqlHelper.First<BookRule>("Host = @host", new SqliteParameter("@host", Host));
            SqlHelper.Conn.Close();
            if (book == null)
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(book.NameStart))
            {
                NameStart = book.NameStart;
            }
            if (!string.IsNullOrWhiteSpace(book.NameEnd))
            {
                NameEnd = book.NameEnd;
            }
            if (!string.IsNullOrWhiteSpace(book.AuthorStart))
            {
                AuthorStart = book.AuthorStart;
            }
            if (!string.IsNullOrWhiteSpace(book.AuthorEnd))
            {
                AuthorEnd = book.AuthorEnd;
            }
            if (!string.IsNullOrWhiteSpace(book.CoverStart))
            {
                CoverStart = book.CoverStart;
            }
            if (!string.IsNullOrWhiteSpace(book.CoverEnd))
            {
                CoverEnd = book.CoverEnd;
            }
            if (!string.IsNullOrWhiteSpace(book.DescriptionStart))
            {
                DescriptionStart = book.DescriptionStart;
            }
            if (!string.IsNullOrWhiteSpace(book.DescriptionEnd))
            {
                DescriptionEnd = book.DescriptionEnd;
            }
            if (!string.IsNullOrWhiteSpace(book.ListStart))
            {
                ListStart = book.ListStart;
            }
            if (!string.IsNullOrWhiteSpace(book.ListEnd))
            {
                ListEnd = book.ListEnd;
            }
            if (!string.IsNullOrWhiteSpace(book.TitleStart))
            {
                TitleStart = book.TitleStart;
            }
            if (!string.IsNullOrWhiteSpace(book.TitleEnd))
            {
                TitleEnd = book.TitleEnd;
            }
            if (!string.IsNullOrWhiteSpace(book.ContentStart))
            {
                ContentStart = book.ContentStart;
            }
            if (!string.IsNullOrWhiteSpace(book.ContentEnd))
            {
                ContentEnd = book.ContentEnd;
            }
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
            if (string.IsNullOrWhiteSpace(Host))
            {
                return;
            }
            SqlHelper.Conn.Open();
            var book = SqlHelper.First<BookRule>("Host = @host", new SqliteParameter("@host", Host));
            if (book == null)
            {
                book = new BookRule();
            }
            book.Host = Host;
            book.NameStart = NameStart;
            book.NameEnd = NameEnd;
            book.AuthorStart = AuthorStart;
            book.AuthorEnd = AuthorEnd;
            book.CoverStart = CoverStart;
            book.CoverEnd = CoverEnd;
            book.DescriptionStart = DescriptionStart;
            book.DescriptionEnd = DescriptionEnd;
            book.ListStart = ListStart;
            book.ListEnd = ListEnd;
            book.TitleStart = TitleStart;
            book.TitleEnd = TitleEnd;
            book.ContentStart = ContentStart;
            book.ContentEnd = ContentEnd;
            book.Save();
            SqlHelper.Conn.Close();
        }

        private RelayCommand _cancelCommand;

        /// <summary>
        /// Gets the CancelCommand.
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand
                    ?? (_cancelCommand = new RelayCommand(ExecuteCancelCommand));
            }
        }

        private void ExecuteCancelCommand()
        {
            NavigationService.NavigateTo(typeof(WebPage).FullName);
        }
    }
}
