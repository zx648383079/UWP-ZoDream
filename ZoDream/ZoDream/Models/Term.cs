using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Models
{
    public class Term
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string Thumb { get; set; }
        public int BlogCount { get; set; }
    }
}
