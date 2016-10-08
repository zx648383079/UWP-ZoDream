using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;
using AppStudio.Uwp;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Navigation;
using AppStudio.Uwp.Commands;
using AppStudio.DataProviders;

using AppStudio.DataProviders.Rss;
using AppStudio.DataProviders.Flickr;
using AppStudio.DataProviders.Menu;
using AppStudio.DataProviders.Html;
using AppStudio.DataProviders.LocalStorage;
using ZoDream.Sections;


namespace ZoDream.ViewModels
{
    public class MainViewModel : PageViewModelBase
    {
        public ListViewModel Albums { get; private set; }
        public ListViewModel Concerts { get; private set; }
        public ListViewModel BandNews { get; private set; }
        public ListViewModel Gallery { get; private set; }
        public ListViewModel Links { get; private set; }
        public ListViewModel TheyAre { get; private set; }

        public MainViewModel(int visibleItems) : base()
        {
            Title = "ZoDream ¿Í»§¶Ë";
            Albums = ViewModelFactory.NewList(new AlbumsSection(), visibleItems);
            Concerts = ViewModelFactory.NewList(new ConcertsSection(), visibleItems);
            BandNews = ViewModelFactory.NewList(new BandNewsSection(), visibleItems);
            Gallery = ViewModelFactory.NewList(new GallerySection(), visibleItems);
            Links = ViewModelFactory.NewList(new LinksSection());
            TheyAre = ViewModelFactory.NewList(new TheyAreSection(), visibleItems);

            if (GetViewModels().Any(vm => !vm.HasLocalData))
            {
                Actions.Add(new ActionInfo
                {
                    Command = RefreshCommand,
                    Style = ActionKnownStyles.Refresh,
                    Name = "RefreshButton",
                    ActionType = ActionType.Primary
                });
            }
        }

		#region Commands
		public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var refreshDataTasks = GetViewModels()
                        .Where(vm => !vm.HasLocalData).Select(vm => vm.LoadDataAsync(true));

                    await Task.WhenAll(refreshDataTasks);
					LastUpdated = GetViewModels().OrderBy(vm => vm.LastUpdated, OrderType.Descending).FirstOrDefault()?.LastUpdated;
                    OnPropertyChanged("LastUpdated");
                });
            }
        }
		#endregion

        public async Task LoadDataAsync()
        {
            var loadDataTasks = GetViewModels().Select(vm => vm.LoadDataAsync());

            await Task.WhenAll(loadDataTasks);
			LastUpdated = GetViewModels().OrderBy(vm => vm.LastUpdated, OrderType.Descending).FirstOrDefault()?.LastUpdated;
            OnPropertyChanged("LastUpdated");
        }

        private IEnumerable<ListViewModel> GetViewModels()
        {
            yield return Albums;
            yield return Concerts;
            yield return BandNews;
            yield return Gallery;
            yield return Links;
            yield return TheyAre;
        }
    }
}
