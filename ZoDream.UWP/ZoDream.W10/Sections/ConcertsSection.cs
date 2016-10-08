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
    public class ConcertsSection : Section<Concerts1Schema>
    {
		private LocalStorageDataProvider<Concerts1Schema> _dataProvider;

		public ConcertsSection()
		{
			_dataProvider = new LocalStorageDataProvider<Concerts1Schema>();
		}

		public override async Task<IEnumerable<Concerts1Schema>> GetDataAsync(SchemaBase connectedItem = null)
        {
            var config = new LocalStorageDataConfig
            {
                FilePath = "/Assets/Data/Concerts.json",
            };
            return await _dataProvider.LoadDataAsync(config, MaxRecords);
        }

        public override async Task<IEnumerable<Concerts1Schema>> GetNextPageAsync()
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

        public override ListPageConfig<Concerts1Schema> ListPage
        {
            get 
            {
                return new ListPageConfig<Concerts1Schema>
                {
                    Title = "concerts",

                    Page = typeof(Pages.ConcertsListPage),

                    LayoutBindings = (viewModel, item) =>
                    {
						viewModel.Header = item.MonthText.ToSafeString();
                        viewModel.Title = item.City.ToSafeString();
                        viewModel.SubTitle = item.Time.ToString(DateTimeFormat.LongDate);
                        viewModel.Description = item.MonthText.ToSafeString();
						viewModel.Aside = item.Time.ToString(DateTimeFormat.CardTime);
						viewModel.Footer = item.Room.ToSafeString();

						viewModel.GroupBy = item.MonthText.SafeType();

						viewModel.OrderBy = item.Time;
                    },
					OrderType = OrderType.Ascending,
                    DetailNavigation = (item) =>
                    {
                        return null;
                    }
                };
            }
        }

        public override DetailPageConfig<Concerts1Schema> DetailPage
        {
            get
            {
                var bindings = new List<Action<ItemViewModel, Concerts1Schema>>();

                var actions = new List<ActionConfig<Concerts1Schema>>
                {
                };

                return new DetailPageConfig<Concerts1Schema>
                {
                    Title = "concerts",
                    LayoutBindings = bindings,
                    Actions = actions
                };
            }
        }
    }
}
