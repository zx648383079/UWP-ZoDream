using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Models.Api
{
    public class BookApi : BaseApi
    {
        /**
         * 获取博客列表
         */
        public async Task<Tuple<IList<Book>, bool>> GetListAsync(uint page = 1, uint perPage = 10)
        {
            var data = await GetPageAsync<Book>($"book?per_page={perPage}&page={page}");
            if (data == null)
            {
                return Tuple.Create<IList<Book>, bool>(null, false);
            }
            return Tuple.Create(data.Data, data.Paging.More);
        }

        public async Task<Book> GetBook(uint id)
        {
            return await GetAsync<Book>($"book?id={id}");
        }
        
        public async Task<IList<BookChapter>> GetChapterListAsync(int book)
        {
            var data = await GetPageAsync<BookChapter>($"book/chapter?book={book}");
            if (data == null)
            {
                return null;
            }
            return data.Data;
        }

        public async Task<BookChapter> GetChapterAsync(uint id)
        {
            return await GetAsync<BookChapter>($"book/chapter?id={id}");
        }
    }
}
