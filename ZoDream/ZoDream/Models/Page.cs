using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Models
{
    public class Page<T>
    {
        public Paging Paging { get; set; }

        public IList<T> Data { get; set; }
    }
}
