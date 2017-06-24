using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Model
{
    public class BookRule
    {
        public int Id { get; set; }

        public string Host { get; set; }

        public string NameStart { get; set; }

        public string NameEnd { get; set; }

        public string AuthorStart { get; set; }
        public string AuthorEnd { get; set; }

        public string DescriptionStart { get; set; }

        public string DescriptionEnd { get; set; }

        public string ListStart { get; set; }

        public string ListEnd { get; set; }

        public string TitleStart { get; set; }

        public string TitleEnd { get; set; }

        public string ContentStart { get; set; }

        public string ContentEnd { get; set; }
    }
}
