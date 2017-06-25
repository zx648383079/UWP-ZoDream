using Microsoft.Data.Sqlite;
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

        public BookRule()
        {

        }

        public BookRule(SqliteDataReader reader)
        {
            Id = reader.GetInt32(0);
            Host = reader.GetString(1);
            NameStart = reader.GetString(2);
            NameEnd = reader.GetString(3);
            AuthorStart = reader.GetString(4);
            AuthorEnd = reader.GetString(5);
            CoverStart = reader.GetString(6);
            CoverEnd = reader.GetString(7);
            DescriptionStart = reader.GetString(8);
            DescriptionEnd = reader.GetString(9);
            ListStart = reader.GetString(10);
            ListEnd = reader.GetString(11);
            TitleStart = reader.GetString(12);
            TitleEnd = reader.GetString(13);
            ContentStart = reader.GetString(14);
            ContentEnd = reader.GetString(15);
    }
    }
}
