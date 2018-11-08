using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Model
{
    public class UrlItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public UrlItem()
        {

        }

        public UrlItem(string title, string url)
        {
            Name = title;
            Url = url;
        }

        public UrlItem(string title, Uri url)
        {
            Name = title;
            Url = url.AbsoluteUri;
        }
    }
}
