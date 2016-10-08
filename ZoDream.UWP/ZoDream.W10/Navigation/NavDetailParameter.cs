using AppStudio.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Navigation
{
    public class NavDetailParameter
    {
        public IEnumerable<SchemaBase> Items { get; set; }
        public string SelectedId { get; set; }
    }
}