using Microsoft.Data.Sqlite;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Helper;

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

        public string Url { get; set; } = "";

        public void Delete()
        {
            if (Id > 0)
            {
                SqlHelper.Delete <BookChapter> (Id);
                Id = 0;
            }
        }

        public void Save()
        {
            if (Id > 0)
            {
                SqlHelper.Update<BookChapter>(new string[] {
                    "`BookId` = @book",
                    "`Name` = @name",
                    "`Content` = @content",
                    "`Position` = @position",
                    "`Url` = @url"
                }, Id,
                new SqliteParameter("@name", Name),
                new SqliteParameter("@book", BookId),
                new SqliteParameter("@content", Content),
                new SqliteParameter("@position", Position),
                new SqliteParameter("@url", Url));
            }
            else
            {

                Id = SqlHelper.InsertId<BookChapter>("`Name`, `BookId`, `Content`, `Position`, `Url`", "@name, @book, @content, @position, @url",
                new SqliteParameter("@name", Name),
                new SqliteParameter("@book", BookId),
                new SqliteParameter("@content", Content),
                new SqliteParameter("@position", Position),
                new SqliteParameter("@url", Url));
            }
        }

        public BookChapter()
        {

        }

        public BookChapter(SqliteDataReader reader)
        {
            Id = reader.GetInt32(0);

            BookId = reader.GetInt32(1);

            Name = reader.GetString(2);

            if (reader.FieldCount >= 6)
            {
                Content = reader.IsDBNull(3) ? "" : reader.GetString(3);


                if (!reader.IsDBNull(4))
                {
                    Position = reader.GetInt32(4);
                }
                if (!reader.IsDBNull(5))
                {
                    Url = reader.GetString(5);
                }
            } else
            {
                if (!reader.IsDBNull(3))
                {
                    Position = reader.GetInt32(3);
                }
                if (!reader.IsDBNull(4))
                {
                    Url = reader.GetString(4);
                }
            }
            
            
        }
    }
}
