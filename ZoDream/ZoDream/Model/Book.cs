using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Model
{
    public class Book
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public int Index { get; set; }

        public int Count { get; set; }

        public string Thumb { get; set; }

        public DateTime ReadTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
