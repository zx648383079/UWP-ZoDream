using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;

using AppStudio.DataProviders;
using AppStudio.Uwp.Cache;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.DataSync;

using ZoDream.Navigation;
using ZoDream.Sections;
using ZoDream.Services;

namespace ZoDream.ViewModels
{
    public abstract class GroupedListViewModel : ListViewModelBase
    {
        protected bool _canLoadNextPage = true;

        public GroupedListViewModel(string title, string sectionName)
        {
            Title = title;
            SectionName = sectionName;
        }

        protected ObservableCollection<GroupedItemViewModel> _groupedItems = new ObservableCollection<GroupedItemViewModel>();

        public ObservableCollection<GroupedItemViewModel> Items
        {
            get { return _groupedItems; }
            protected set { SetProperty(ref _groupedItems, value); }
        }

        #region Commands
        private RelayCommand _loadNextPageCommand;
        public RelayCommand LoadNextPageCommand
        {
            get
            {
                if (_loadNextPageCommand == null)
                {
                    _loadNextPageCommand = new RelayCommand(
                    async () =>
                    {
                        if (!IsBusy)
                        {
                            await LoadNextPageAsync();
                        }
                    }, () => _canLoadNextPage);
                }
                return _loadNextPageCommand;
            }
        }
        #endregion
    }
    public class GroupedListViewModel<TSchema> : GroupedListViewModel where TSchema : SchemaBase
    {
        private Section<TSchema> _section;
        private List<ItemViewModel> _items = new List<ItemViewModel>();

        public GroupedListViewModel(Section<TSchema> section) : base(section.ListPage.Title, section.Name)
        {
            _section = section;

            HasLocalData = !section.NeedsNetwork;

            if (section.NeedsNetwork)
            {
                Actions.Add(new ActionInfo
                {
                    Command = Refresh,
                    Style = ActionKnownStyles.Refresh,
                    Name = "RefreshButton",
                    ActionType = ActionType.Primary
                });
            }

            Actions.Add(new ActionInfo
            {
                Command = LoadNextPageCommand,
                Style = "AppBarNextPage",
                Name = "LoadMoreItemsButton",
                ActionType = ActionType.Primary
            });
        }

        public override async Task LoadDataAsync(bool forceRefresh = false, SchemaBase connected = null)
        {
            try
            {
                HasLoadDataErrors = false;
                IsBusy = true;

                _items.Clear();
                SourceItems.Clear();

                var loaderSettings = LoaderSettings.FromSection(_section, _section.Name, forceRefresh);
                var loaderOutcome = await DataLoader.LoadAsync(loaderSettings, () => _section.GetDataAsync(), (items) => ParseItems(items));

                LastUpdated = loaderOutcome.Timestamp;
                _isDataProviderInitialized = loaderOutcome.IsFreshData;
                _isFirstPage = true;

                if (HasItems)
                {
                    GroupItems();
                }
            }
            catch (Exception ex)
            {
                HasLoadDataErrors = true;
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override async Task LoadNextPageAsync()
        {
            try
            {
                HasLoadDataErrors = false;
                IsBusy = true;
                IsLoadingNextPage = true;

                if (!_isDataProviderInitialized && _isFirstPage)
                {
                    await LoadDataAsync(true);
                    _isFirstPage = false;
                }

                await DataLoader.LoadAsync(LoaderSettings.NoCache(_section.NeedsNetwork), () => _section.GetNextPageAsync(), (items) => ParseItems(items));

                if (HasItems)
                {
                    GroupItems();
                }
            }
            catch (Exception ex)
            {
                HasLoadDataErrors = true;
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                IsBusy = false;
                IsLoadingNextPage = false;
            }
        }

        private void ParseItems(IEnumerable<TSchema> content)
        {
            SourceItems.AddRange(content);

            foreach (var item in content)
            {
                var parsedItem = new ItemViewModel
                {
                    Id = item._id,
                    NavInfo = _section.ListPage.DetailNavigation(item)
                };
                _section.ListPage.LayoutBindings(parsedItem, item);
                _items.Add(parsedItem);
            }
            HasItems = _items.Count > 0;
            _canLoadNextPage = content.Count() > 0;
            LoadNextPageCommand.OnCanExecuteChanged();
        }

        private void GroupItems()
        {
            var gItems = _items
                            .GroupBy(i => i.GroupBy)
                            .OrderBy(gi => gi.Key)
                            .Select(gi => new GroupedItemViewModel
                            {
                                Header = GetHeader(gi),
                                Items = new ObservableCollection<ItemViewModel>(gi.OrderBy(i => i.OrderBy, _section.ListPage.OrderType))
                            })
                            .ToList();

            _groupedItems.Sync(gItems);
        }

        private static string GetHeader(IEnumerable<ItemViewModel> items)
        {
            var currentItem = items.FirstOrDefault();
            if (currentItem != null)
            {
                return currentItem.Header;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}