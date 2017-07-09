using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using System.Net.Sockets;
using GalaSoft.MvvmLight.Command;
using ZoDreamToolkit.Common;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.Model;

namespace ZoDream.ViewModel
{
    public class HttpViewModel : BaseViewModel
    {
        private NotificationMessageAction<Book> _addBook;

        public HttpViewModel(INavigationService navigationService) : base(navigationService)
        {
            Messenger.Default.Register<NotificationMessageAction<Book>>(this, "book", m =>
            {
                _addBook = m;
            });
            Ip = HttpHelper.GetLocalIp();
        }

        /// <summary>
        /// The <see cref="BtnLabel" /> property's name.
        /// </summary>
        public const string BtnLabelPropertyName = "BtnLabel";

        private string _btnLabel = "启动";

        /// <summary>
        /// Sets and gets the BtnLabel property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string BtnLabel
        {
            get
            {
                return _btnLabel;
            }
            set
            {
                Set(BtnLabelPropertyName, ref _btnLabel, value);
            }
        }

        /// <summary>
        /// The <see cref="Ip" /> property's name.
        /// </summary>
        public const string IpPropertyName = "Ip";

        private string _ip = string.Empty;

        /// <summary>
        /// Sets and gets the Ip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Ip
        {
            get
            {
                return _ip;
            }
            set
            {
                Set(IpPropertyName, ref _ip, value);
            }
        }

        /// <summary>
        /// The <see cref="Port" /> property's name.
        /// </summary>
        public const string PortPropertyName = "Port";

        private int _port = 80;

        /// <summary>
        /// Sets and gets the Port property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                Set(PortPropertyName, ref _port, value);
            }
        }

        private RelayCommand _startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand
                    ?? (_startCommand = new RelayCommand(ExecuteStartCommand));
            }
        }

        private void ExecuteStartCommand()
        {

        }
    }
}
