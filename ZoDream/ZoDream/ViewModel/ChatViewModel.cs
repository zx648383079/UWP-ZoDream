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
    public class ChatViewModel:BaseViewModel
    {
       


        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private string _title = "与  聊天中";

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                Set(TitlePropertyName, ref _title, value);
            }
        }

        /// <summary>
        /// The <see cref="MessageList" /> property's name.
        /// </summary>
        public const string MessageListPropertyName = "MessageList";

        private ObservableCollection<ChatItem> _messageList = new ObservableCollection<ChatItem>();

        /// <summary>
        /// Sets and gets the MessageList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ChatItem> MessageList
        {
            get
            {
                return _messageList;
            }
            set
            {
                Set(MessageListPropertyName, ref _messageList, value);
            }
        }

        /// <summary>
        /// The <see cref="IsPullRefresh" /> property's name.
        /// </summary>
        public const string IsPullRefreshPropertyName = "IsPullRefresh";

        private bool _isPullRefresh = false;

        public ChatViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        /// <summary>
        /// Sets and gets the IsPullRefresh property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsPullRefresh
        {
            get
            {
                return _isPullRefresh;
            }
            set
            {
                Set(IsPullRefreshPropertyName, ref _isPullRefresh, value);
            }
        }

    }
}
