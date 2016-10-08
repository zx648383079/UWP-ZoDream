using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using AppStudio.Uwp;
using AppStudio.Uwp.Commands;
using AppStudio.DataProviders;

using ZoDream.Pages;
using ZoDream.Navigation;

namespace ZoDream.ViewModels
{
    public abstract class ListViewModelBase : PageViewModelBase
    {
        protected bool _isDataProviderInitialized;
        protected bool _isFirstPage;

        protected List<SchemaBase> SourceItems { get; } = new List<SchemaBase>();

        public abstract Task LoadDataAsync(bool forceRefresh = false, SchemaBase connected = null);
        public abstract Task LoadNextPageAsync();

        private bool _hasItems;
        public bool HasItems
        {
            get { return _hasItems; }
            protected set { SetProperty(ref _hasItems, value); }
        }

        private bool _isLoadingNextPage;
        public bool IsLoadingNextPage
        {
            get { return _isLoadingNextPage; }
            set { SetProperty(ref _isLoadingNextPage, value); }
        }

        public RelayCommand<ItemViewModel> ItemClickCommand
        {
            get
            {
                return new RelayCommand<ItemViewModel>(
                (item) =>
                {
                    AppNavigation.Navigate(item, SourceItems);
                });
            }
        }

        public ICommand Refresh
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    ShellPage.Current.Frame.ScrollToTop();
                    await LoadDataAsync(true);
                });
            }
        }
    }
}
