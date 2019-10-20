using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Models.Api
{
    public class BlogApi: BaseApi
    {
        /**
         * 获取博客列表
         */
        public async Task<Tuple<IList<Blog>, bool>> GetListAsync(uint page = 1, uint perPage = 10)
        {
            var data = await GetPageAsync<Blog>($"blog?per_page={perPage}&page={page}");
            if (data == null)
            {
                return Tuple.Create<IList<Blog>, bool>(null, false);
            }
            return Tuple.Create(data.Data, data.Paging.More);
        }

        public async Task<Blog> GetBlogAsync(uint id)
        {
            return await GetAsync<Blog>($"blog?id={id}");
        }

        public async Task<Blog> GetContentAsync(uint id)
        {
            return await GetAsync<Blog>($"blog/home/content?id={id}");
        }
    }
}
