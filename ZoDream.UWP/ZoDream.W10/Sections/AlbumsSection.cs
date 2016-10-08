using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppStudio.DataProviders;
using AppStudio.DataProviders.Core;
using AppStudio.DataProviders.LocalStorage;
using AppStudio.Uwp;
using Windows.ApplicationModel.Appointments;
using System.Linq;

using ZoDream.Navigation;
using ZoDream.ViewModels;

namespace ZoDream.Sections
{
    public class AlbumsSection : Section<Albums1Schema>
    {
		private LocalStorageDataProvider<Albums1Schema> _dataProvider;

		public AlbumsSection()
		{
			_dataProvider = new LocalStorageDataProvider<Albums1Schema>();
		}

		public override async Task<IEnumerable<Albums1Schema>> GetDataAsync(SchemaBase connectedItem = null)
        {
            var config = new LocalStorageDataConfig
            {
                FilePath = "/Assets/Data/Albums.json",
            };
            return await _dataProvider.LoadDataAsync(config, MaxRecords);
        }

        public override async Task<IEnumerable<Albums1Schema>> GetNextPageAsync()
        {
            return await _dataProvider.LoadMoreDataAsync();
        }

        public override bool HasMorePages
        {
            get
            {
                return _dataProvider.HasMoreItems;
            }
        }

        public override bool NeedsNetwork
        {
            get
            {
                return false;
            }
        }

        public override ListPageConfig<Albums1Schema> ListPage
        {
            get 
            {
                return new ListPageConfig<Albums1Schema>
                {
                    Title = "albums",

                    Page = typeof(Pages.AlbumsListPage),

                    LayoutBindings = (viewModel, item) =>
                    {
                        viewModel.Title = item.Title.ToSafeString();
                        viewModel.SubTitle = item.TrackList.ToSafeString();
                        viewModel.ImageUrl = ItemViewModel.LoadSafeUrl(item.ImageUrl.ToSafeString());
                    },
                    DetailNavigation = (item) =>
                    {
						return NavInfo.FromPage<Pages.AlbumsDetailPage>(true);
                    }
                };
            }
        }

        public override DetailPageConfig<Albums1Schema> DetailPage
        {
            get
            {
                var bindings = new List<Action<ItemViewModel, Albums1Schema>>();
                bindings.Add((viewModel, item) =>
                {
                    viewModel.PageTitle = item.Title.ToSafeString();
                    viewModel.Title = item.Year.ToSafeString();
                    viewModel.Description = item.TrackList.ToSafeString();
                    viewModel.ImageUrl = ItemViewModel.LoadSafeUrl(item.ImageUrl.ToSafeString());
                    viewModel.Content = null;
                });

                var actions = new List<ActionConfig<Albums1Schema>>
                {
                };

                return new DetailPageConfig<Albums1Schema>
                {
                    Title = "albums",
                    LayoutBindings = bindings,
                    Actions = actions
                };
            }
        }
    }
}
