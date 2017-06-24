using GalaSoft.MvvmLight;
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
    public class Book: ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public int Index { get; set; } = 0;

        public int Count { get; set; } = 0;

        public string Thumb { get; set; } = "/Assets/default.jpg";

        public string Author { get; set; }

        public string Description { get; set; }

        public BookKinds Kind { get; set; } = BookKinds.其他;

        public bool IsLocal { get; set; } = true;

        public int LastChapter { get; set; }

        public DateTime ReadTime { get; set; }

        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// The <see cref="IsChecked" /> property's name.
        /// </summary>
        public const string IsCheckedPropertyName = "IsChecked";

        private bool _isChecked = false;

        /// <summary>
        /// Sets and gets the IsChecked property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                Set(IsCheckedPropertyName, ref _isChecked, value);
            }
        }

        /// <summary>
        /// The <see cref="EditMode" /> property's name.
        /// </summary>
        public const string EditModePropertyName = "EditMode";

        private bool _editMode = false;

        /// <summary>
        /// Sets and gets the EditMode property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                Set(EditModePropertyName, ref _editMode, value);
            }
        }


        public void Delete()
        {
            if (Id < 1)
            {
                return;
            }
            SqlHelper.Delete<Book>(Id);
            SqlHelper.Delete<BookChapter>("BookId=" + Id);
            Id = 0;
        }

        public void Save()
        {
            if (Id > 0)
            {
                SqlHelper.Update<Book>(new string[] {
                    "`Name` = @name",
                    "`Index` = @index",
                    "`Count` = @count",
                    "`LastChapter` = @chapter"
                }, Id, 
                new SqliteParameter("@name", Name), 
                new SqliteParameter("@index", Index), 
                new SqliteParameter("@count", Count),
                new SqliteParameter("@chapter", LastChapter));
            } else
            {
                
                Id = SqlHelper.InsertId<Book>("`Name`, `Index`, `Count`, `LastChapter`", "@name, @index, @count, @chapter", new SqliteParameter("@name", Name),
                new SqliteParameter("@index", Index),
                new SqliteParameter("@count", Count),
                new SqliteParameter("@chapter", LastChapter));
            }
        }

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
            if (string.IsNullOrWhiteSpace(Thumb))
            {
                Thumb = "/Assets/default.jpg";
            }
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
