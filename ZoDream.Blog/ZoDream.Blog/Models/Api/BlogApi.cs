using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Blog.Models.Api
{
    public class BlogApi: BaseApi
    {
        public async Task<IList<Blog>> GetListAsync(int per_page = 1, int page = 10)
        {
            return await GetPageAsync<Blog>($"/blog?per_page={per_page}&page={page}");
        }
    }
}
