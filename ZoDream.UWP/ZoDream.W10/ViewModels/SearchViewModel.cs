using System;
using System.Collections.Generic;
using AppStudio.Uwp;
using AppStudio.Uwp.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Sections;
namespace ZoDream.ViewModels
{
    public class SearchViewModel : PageViewModelBase
    {
        public SearchViewModel() : base()
        {
			Title = "ZoDream";
            Albums = ViewModelFactory.NewList(new AlbumsSection());
            Concerts = ViewModelFactory.NewList(new ConcertsSection());
            BandNews = ViewModelFactory.NewList(new BandNewsSection());
            Gallery = ViewModelFactory.NewList(new GallerySection());
            TheyAre = ViewModelFactory.NewList(new TheyAreSection());
                        
        }

        private string _searchText;
        private bool _hasItems = true;

        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        public bool HasItems
        {
            get { return _hasItems; }
            set { SetProperty(ref _hasItems, value); }
        }

		public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand<string>(
                async (text) =>
                {
                    await SearchDataAsync(text);
                }, SearchViewModel.CanSearch);
            }
        }      
        public ListViewModel Albums { get; private set; }
        public ListViewModel Concerts { get; private set; }
        public ListViewModel BandNews { get; private set; }
        public ListViewModel Gallery { get; private set; }
        public ListViewModel TheyAre { get; private set; }
        public async Task SearchDataAsync(string text)
        {
            this.HasItems = true;
            SearchText = text;
            var loadDataTasks = GetViewModels()
                                    .Select(vm => vm.SearchDataAsync(text));

            await Task.WhenAll(loadDataTasks);
			this.HasItems = GetViewModels().Any(vm => vm.HasItems);
        }

        private IEnumerable<ListViewModel> GetViewModels()
        {
            yield return Albums;
            yield return Concerts;
            yield return BandNews;
            yield return Gallery;
            yield return TheyAre;
        }
		private void CleanItems()
        {
            foreach (var vm in GetViewModels())
            {
                vm.CleanItems();
            }
        }
		public static bool CanSearch(string text) { return !string.IsNullOrWhiteSpace(text) && text.Length >= 3; }
    }
}
