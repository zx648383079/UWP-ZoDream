using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Controls
{
    public class PageLine
    {
        public int Start { get; set; }

        public int Lnegth { get; set; }

        public bool IsNew { get; set; } = false;

        public int GetLineCount(int count)
        {
            return (int)Math.Ceiling((double)(Lnegth + 2) / count);
        }
    }
}
