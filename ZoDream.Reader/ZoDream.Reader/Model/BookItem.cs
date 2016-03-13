using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Reader.Model
{
    public class BookItem
    {
        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Url { get; set; }

        public ImageKind ImageKind { get; set; } = ImageKind.DEFAULT;

        [MaxLength(255)]
        public string Profile { get; set; }

        public string Author { get; set; }

        public string Kind { get; set; }

        public int Index { get; set; }

        public int Count { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime CreateTime { get; set; }

        public BookItem()
        {

        }

        public BookItem(string name)
        {
            Name = name;
        }

        public BookItem(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public BookItem(string name, string url, ImageKind kind)
        {
            Name = name;
            Url = url;
            ImageKind = kind;
        }

        public BookItem(string name, ImageKind kind)
        {
            Name = name;
            ImageKind = kind;
        }
    }

    public enum ImageKind
    {
        DEFAULT,
        QIDIAN,
        CHUANGSHI,
        OTHER
    }
}
