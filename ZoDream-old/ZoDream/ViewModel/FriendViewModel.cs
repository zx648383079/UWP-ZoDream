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
    public class FriendViewModel:BaseViewModel
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
        /// The <see cref="FriendList" /> property's name.
        /// </summary>
        public const string FriendListPropertyName = "FriendList";

        private ObservableCollection<FriendItem> _friendList = new ObservableCollection<FriendItem>();

        public FriendViewModel(INavigationService navigationService) : base(navigationService)
        {
            FriendList.Add(new FriendItem() { Name = "aaaaa", Pubished = DateTime.Parse("2011-10-11") });
            FriendList.Add(new FriendItem() { Name = "aa", Avatar = "/Assets/StoreLogo.png", Content = "24343243243242343243243243243243", Count = 3, Pubished = DateTime.Now });
        }

        /// <summary>
        /// Sets and gets the FriendList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<FriendItem> FriendList
        {
            get
            {
                return _friendList;
            }
            set
            {
                Set(FriendListPropertyName, ref _friendList, value);
            }
        }
        

    }
}
