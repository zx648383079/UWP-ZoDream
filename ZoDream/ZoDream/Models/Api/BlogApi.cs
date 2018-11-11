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
            return Tuple.Create(data, data != null && data.Count == perPage);
        }

        public async Task<Blog> GetBlog(uint id)
        {
            return await GetAsync<Blog>($"blog?id={id}");
        }
    }
}
