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
using ZoDream.Helper;
using GalaSoft.MvvmLight.Messaging;

namespace ZoDream.ViewModel
{
    public class HistoryViewModel : BaseViewModel
    {
        private NotificationMessageAction<Uri> _downUrl;

        public HistoryViewModel(INavigationService navigationService) : base(navigationService)
        {
            Messenger.Default.Register<NotificationMessageAction<Uri>>(this, "down", m =>
            {
                _downUrl = m;
                var uri = m.Sender as Uri;
                if (uri != null)
                {
                    _downFile(uri);
                }
            });
            SqlHelper.Conn.Open();
            using (var reader = SqlHelper.Select<FavoriteUrl>())
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        Favorites.Add(new FavoriteUrl(reader));
                    }
                }
            }
            SqlHelper.Conn.Close();
        }

        private void _downFile(Uri uri)
        {
            var file = new FileUrl(uri);
            FileList.Add(file);
            file.DownFileAsync();
        }

        /// <summary>
        /// The <see cref="Favorites" /> property's name.
        /// </summary>
        public const string FavoritesPropertyName = "Favorites";

        private ObservableCollection<FavoriteUrl> _favorites = new ObservableCollection<FavoriteUrl>();

        /// <summary>
        /// Sets and gets the Favorites property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<FavoriteUrl> Favorites
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

        /// <summary>
        /// The <see cref="FileList" /> property's name.
        /// </summary>
        public const string FileListPropertyName = "FileList";

        private ObservableCollection<FileUrl> _fileList = new ObservableCollection<FileUrl>();

        /// <summary>
        /// Sets and gets the FileList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<FileUrl> FileList
        {
            get
            {
                return _fileList;
            }
            set
            {
                Set(FileListPropertyName, ref _fileList, value);
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
                SqlHelper.Conn.Open();
                favoriteDialog.Url.Save();
                SqlHelper.Conn.Close();
            }
        }
    }
}
