using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace ZoDream.Models
{
    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        // 这里为了简单使用了Tuple<IList<T>, bool>作为返回值，第一项是新项目集合，第二项是否还有更多，也可以自定义实体类
        Func<uint, Task<Tuple<IList<T>, bool>>> _dataFetchDelegate = null;

        public IncrementalLoadingCollection(Func<uint, Task<Tuple<IList<T>, bool>>> dataFetchDelegate)
        {
            this._dataFetchDelegate = dataFetchDelegate ?? throw new ArgumentNullException("dataFetchDelegate");
        }

        public bool HasMoreItems
        {
            get;
            private set;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }

            _busy = true;

            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        protected async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                if (OnLoadMoreStarted != null)
                {
                    OnLoadMoreStarted(count);
                }

                // 我们忽略了CancellationToken，因为我们暂时不需要取消，需要的可以加上
                var result = await this._dataFetchDelegate(count);

                var items = result.Item1;

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Add(item);
                    }
                }

                // 是否还有更多
                HasMoreItems = result.Item2;

                // 加载完成事件
                OnLoadMoreCompleted?.Invoke(items == null ? 0 : items.Count);

                return new LoadMoreItemsResult { Count = items == null ? 0 : (uint)items.Count };
            }
            finally
            {
                _busy = false;
            }
        }


        public delegate void LoadMoreStarted(uint count);
        public delegate void LoadMoreCompleted(int count);

        public event LoadMoreStarted OnLoadMoreStarted;
        public event LoadMoreCompleted OnLoadMoreCompleted;

        protected bool _busy = false;
    }
}
