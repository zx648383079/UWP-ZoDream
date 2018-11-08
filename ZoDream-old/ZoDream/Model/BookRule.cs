using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Helper;

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

        public string CoverStart { get; set; }

        public string CoverEnd { get; set; }

        public string DescriptionStart { get; set; }

        public string DescriptionEnd { get; set; }

        public string ListStart { get; set; }

        public string ListEnd { get; set; }

        public string TitleStart { get; set; }

        public string TitleEnd { get; set; }

        public string ContentStart { get; set; }

        public string ContentEnd { get; set; }

        public void Delete()
        {
            if (Id < 1)
            {
                return;
            }
            SqlHelper.Delete<BookRule>(Id);
            Id = 0;
        }

        public void Save()
        {
            if (Id > 0)
            {
                SqlHelper.Update<Book>(new string[] {
                    "`Host` = @Host",
                    "`NameStart` = @NameStart",
                    "`NameEnd` = @NameEnd",
                    "`AuthorStart` = @AuthorStart",
                    "`AuthorEnd` = @AuthorEnd",
                    "`CoverStart` = @CoverStart",
                    "`CoverEnd` = @CoverEnd",
                    "`DescriptionStart` = @DescriptionStart",
                    "`DescriptionEnd` = @DescriptionEnd",
                    "`ListStart` = @ListStart",
                    "`ListEnd` = @ListEnd",
                    "`TitleStart` = @TitleStart",
                    "`TitleEnd` = @TitleEnd",
                    "`ContentStart` = @ContentStart",
                    "`ContentEnd` = @ContentEnd"
                }, Id,
                new SqliteParameter("@Host", Host),
                new SqliteParameter("@NameStart", NameStart),
                new SqliteParameter("@NameEnd", NameEnd),
                new SqliteParameter("@AuthorStart", AuthorStart),
                new SqliteParameter("@AuthorEnd", AuthorEnd),
                new SqliteParameter("@CoverStart", CoverStart),
                new SqliteParameter("@CoverEnd", CoverEnd),
                new SqliteParameter("@DescriptionStart", DescriptionStart),
                new SqliteParameter("@DescriptionEnd", DescriptionEnd),
                new SqliteParameter("@ListStart", ListStart),
                new SqliteParameter("@ListEnd", ListEnd),
                new SqliteParameter("@TitleStart", TitleStart),
                new SqliteParameter("@TitleEnd", TitleEnd),
                new SqliteParameter("@ContentStart", ContentStart),
                new SqliteParameter("@ContentEnd", ContentEnd));
            }
            else
            {

                Id = SqlHelper.InsertId<Book>("`Host`, `NameStart`, `NameEnd`, `AuthorStart`, `AuthorEnd`, `CoverStart`, `CoverEnd`, `DescriptionStart`, `DescriptionEnd`, `ListStart`, `ListEnd`, `TitleStart`, `TitleEnd`, `ContentStart`, `ContentEnd`",
                    "@Host, @NameStart, @NameEnd, @AuthorStart, @AuthorEnd, @CoverStart, @CoverEnd, @DescriptionStart, @DescriptionEnd, @ListStart, @ListEnd, @TitleStart, @TitleEnd, @ContentStart, @ContentEnd",
                    new SqliteParameter("@Host", Host),
                    new SqliteParameter("@NameStart", NameStart),
                    new SqliteParameter("@NameEnd", NameEnd),
                    new SqliteParameter("@AuthorStart", AuthorStart),
                    new SqliteParameter("@AuthorEnd", AuthorEnd),
                    new SqliteParameter("@CoverStart", CoverStart),
                    new SqliteParameter("@CoverEnd", CoverEnd),
                    new SqliteParameter("@DescriptionStart", DescriptionStart),
                    new SqliteParameter("@DescriptionEnd", DescriptionEnd),
                    new SqliteParameter("@ListStart", ListStart),
                    new SqliteParameter("@ListEnd", ListEnd),
                    new SqliteParameter("@TitleStart", TitleStart),
                    new SqliteParameter("@TitleEnd", TitleEnd),
                    new SqliteParameter("@ContentStart", ContentStart),
                    new SqliteParameter("@ContentEnd", ContentEnd));
            }
        }

        public BookRule()
        {

        }

        public BookRule(SqliteDataReader reader)
        {
            Id = reader.GetInt32(0);
            if (!reader.IsDBNull(1))
            {
                Host = reader.GetString(1);
            }
            if (!reader.IsDBNull(2))
            {
                 NameStart = reader.GetString(2);
            }
            if (!reader.IsDBNull(3))
            {
                 NameEnd = reader.GetString(3);
            }
            if (!reader.IsDBNull(4))
            {
                 AuthorStart = reader.GetString(4);
            }
            if (!reader.IsDBNull(5))
            {
                 AuthorEnd = reader.GetString(5);
            }
            if (!reader.IsDBNull(6))
            {
                 CoverStart = reader.GetString(6);
            }
            if (!reader.IsDBNull(7))
            {
                 CoverEnd = reader.GetString(7);
            }
            if (!reader.IsDBNull(8))
            {
                 DescriptionStart = reader.GetString(8);
            }
            if (!reader.IsDBNull(9))
            {
                 DescriptionEnd = reader.GetString(9);
            }
            if (!reader.IsDBNull(10))
            {
                 ListStart = reader.GetString(10);
            }
            if (!reader.IsDBNull(11))
            {
                 ListEnd = reader.GetString(11);
            }
            if (!reader.IsDBNull(12))
            {
                 TitleStart = reader.GetString(12);
            }
            if (!reader.IsDBNull(13))
            {
                 TitleEnd = reader.GetString(13);
            }
            if (!reader.IsDBNull(14))
            {
                 ContentStart = reader.GetString(14);
            }
            if (!reader.IsDBNull(15))
            {
                 ContentEnd = reader.GetString(15);
            }
    }
    }
}
