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
        public async Task<IList<Blog>> GetListAsync(int page = 1, int perPage = 10)
        {
            return await GetPageAsync<Blog>($"/blog?per_page={perPage}&page={page}");
        }
    }
}
