using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using System.Collections.ObjectModel;
using ZoDream.Model;
using GalaSoft.MvvmLight.Command;
using ZoDream.View;

namespace ZoDream.ViewModel
{
    public class HistoryViewModel : BaseViewModel
    {
        public HistoryViewModel(INavigationService navigationService) : base(navigationService)
        {
            Favorites.Add(new UrlItem("搜狗小说", "http://k.sogou.com?v=2"));
        }

        /// <summary>
        /// The <see cref="Favorites" /> property's name.
        /// </summary>
        public const string FavoritesPropertyName = "Favorites";

        private ObservableCollection<UrlItem> _favorites = new ObservableCollection<UrlItem>();

        /// <summary>
        /// Sets and gets the Favorites property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<UrlItem> Favorites
        {
            get
            {
                return _favorites;
            }
            set
            {
                Set(FavoritesPropertyName, ref _favorites, value);
            }
        }

        private RelayCommand<UrlItem> _navigateCommand;

        /// <summary>
        /// Gets the NavigateCommand.
        /// </summary>
        public RelayCommand<UrlItem> NavigateCommand
        {
            get
            {
                return _navigateCommand
                    ?? (_navigateCommand = new RelayCommand<UrlItem>(ExecuteNavigateCommand));
            }
        }

        private void ExecuteNavigateCommand(UrlItem parameter)
        {
            NavigationService.NavigateTo(typeof(WebPage).FullName, parameter.Url);
        }

        private RelayCommand _addFavoriteCommand;

        /// <summary>
        /// Gets the AddFavoriteCommand.
        /// </summary>
        public RelayCommand AddFavoriteCommand
        {
            get
            {
                return _addFavoriteCommand
                    ?? (_addFavoriteCommand = new RelayCommand(ExecuteAddFavoriteCommand));
            }
        }

        private async void ExecuteAddFavoriteCommand()
        {
            var favoriteDialog = new FavoriteDialog();
            if (await favoriteDialog.ShowAsync() == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                Favorites.Add(favoriteDialog.Url);
            }
        }
    }
}
