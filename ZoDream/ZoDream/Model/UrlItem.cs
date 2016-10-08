using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Model
{
    public class UrlItem
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public UrlItem()
        {

        }

        public UrlItem(string title, string url)
        {
            Title = title;
            Url = url;
        }

        public UrlItem(string title, Uri url)
        {
            Title = title;
            Url = url.AbsoluteUri;
        }
    }
}
