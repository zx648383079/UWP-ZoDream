using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

using AppStudio.Uwp;
using AppStudio.Uwp.Commands;
using AppStudio.DataProviders;

using ZoDream.Sections;
using ZoDream.Navigation;

namespace ZoDream.ViewModels
{
    class DetailRelatedViewModel<TSchema> : DetailViewModel<TSchema> where TSchema : SchemaBase
    {

        private ListViewModel _list;
        public ListViewModel List
        {
            get { return _list; }
            set { SetProperty(ref _list, value); }
        }

        private string _relatedContentStatus;

        public string RelatedContentStatus
        {
            get { return _relatedContentStatus; }
            set { SetProperty(ref _relatedContentStatus, value); }
        }

        private bool _canLoadMorePages;

        public bool CanLoadMorePages
        {
            get { return _canLoadMorePages; }
            set { SetProperty(ref _canLoadMorePages, value); }
        }

        public DetailRelatedViewModel(Section<TSchema> section, ListViewModel list) : base(section)
        {
            List = list;
        }

        public override async Task LoadStateAsync(NavDetailParameter detailParameter)
        {
            await base.LoadStateAsync(detailParameter);

            RelatedContentStatus = "LoadingRelatedContent".StringResource();

            var originalSelected = detailParameter.Items.FirstOrDefault(i => i._id == detailParameter.SelectedId);

            if (originalSelected != null)
            {
                await _list.LoadDataAsync(false, originalSelected);
            }

            if (_list.Items == null || _list.Items.Count == 0)
            {
                RelatedContentStatus = "ThereIsNotRelatedContent".StringResource();
                _list.HasMorePages = false;
            }
            else
            {
                RelatedContentStatus = string.Empty;
            }
            CanLoadMorePages = _list.HasMorePages;
        }

        public ICommand LoadRelatedContentNextPageCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    CanLoadMorePages = false;
                    RelatedContentStatus = "LoadingRelatedContent".StringResource();
                    await _list.LoadNextPageAsync();
                    RelatedContentStatus = string.Empty;
                    CanLoadMorePages = _list.HasMorePages;
                });
            }
        }
    }
}