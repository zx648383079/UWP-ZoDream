using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ZoDream.Layout
{
    public class Pager : FrameworkElement
    {
        private ContainerVisual collection;

        protected Pager()
        {
            collection =  new ContainerVisual(this);

        }

        
    }
}
