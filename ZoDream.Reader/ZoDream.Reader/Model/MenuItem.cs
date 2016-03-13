using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ZoDream.Reader.Model
{
    public class MenuItem
    {
        public string Name { get; set; }

        public Symbol Symbol { get; set; }

        public Type PageType { get; set; }

        public MenuItem()
        {

        }

        public MenuItem(string name, Symbol symblo, Type page)
        {
            Name = name;
            PageType = page;
            Symbol = symblo;
        }
    }
}
