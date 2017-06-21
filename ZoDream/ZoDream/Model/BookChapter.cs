using Microsoft.Data.Sqlite;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Model
{
    public class BookChapter
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int BookId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Content { get; set; }

        public int Position { get; set; } = 99;

        public string Url { get; set; }

        public BookChapter(SqliteDataReader reader)
        {
            Id = reader.GetInt32(0);

            BookId = reader.GetInt32(1);

            Name = reader.GetString(2);
            
            if (!reader.IsDBNull(3))
            {
                Position = reader.GetInt32(3);
            }
            if (!reader.IsDBNull(4))
            {
                Url = reader.GetString(4);
            }
        }

        public BookChapter(SqliteDataReader reader, bool full)
        {
            Id = reader.GetInt32(0);

            BookId = reader.GetInt32(1);

            Name = reader.GetString(2);

            Content = reader.GetString(3);

            if (!reader.IsDBNull(4))
            {
                Position = reader.GetInt32(4);
            }
            if (!reader.IsDBNull(5))
            {
                Url = reader.GetString(5);
            }
        }
    }
}
