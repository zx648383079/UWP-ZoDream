using Microsoft.Data.Sqlite;
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

        public string Author { get; set; }

        public string Description { get; set; }

        public BookKinds Kind { get; set; } = BookKinds.其他;

        public bool IsLocal { get; set; } = true;

        public int ChapterId { get; set; }

        public DateTime ReadTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public Book()
        {

        }

        public Book(SqliteDataReader reader)
        {
            Id = reader.GetInt32(0);
            Name = reader.GetString(1);
            Index = reader.GetInt32(2);
            Count = reader.GetInt32(3);
            Thumb = reader.IsDBNull(4) ? "/Assets/default.jpg" : reader.GetString(4);
            if (!reader.IsDBNull(5))
            {
                ReadTime =  reader.GetDateTime(5);
            }
            if (!reader.IsDBNull(6))
            {
                UpdateTime = reader.GetDateTime(6);
            }
        }
    }
}
