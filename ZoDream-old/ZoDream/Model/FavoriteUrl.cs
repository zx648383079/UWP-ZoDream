using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Helper;

namespace ZoDream.Model
{
    public class FavoriteUrl : UrlItem
    {
        public void Delete()
        {
            if (Id < 1)
            {
                return;
            }
            SqlHelper.Delete<FavoriteUrl>(Id);
            Id = 0;
        }

        public void Save()
        {
            if (Id > 0)
            {
                SqlHelper.Update<FavoriteUrl>(new string[] {
                    "`Name` = @name",
                    "`Url` = @url"
                }, Id,
                new SqliteParameter("@name", Name),
                new SqliteParameter("@url", Url));
            }
            else
            {

                Id = SqlHelper.InsertId<FavoriteUrl>("`Name`, `Url`", "@name, @url",
                    new SqliteParameter("@name", Name),
                    new SqliteParameter("@url", Url));
            }
        }

        public FavoriteUrl(string title, string url) : base(title, url)
        {

        }

        public FavoriteUrl(SqliteDataReader reader)
        {
            Id = reader.GetInt32(0);
            Name = reader.GetString(1);
            Url = reader.GetString(2);
        }
    }
}
