using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using ZoDream.Reader.Model;

namespace ZoDream.Reader.Converters
{
    public class BookImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((ImageKind)value)
            {
                case ImageKind.OTHER:
                    return "ms-appx:///Assets/book3.jpg";
                case ImageKind.CHUANGSHI:
                    return "ms-appx:///Assets/book2.jpg";
                case ImageKind.QIDIAN:
                case ImageKind.DEFAULT:
                default:
                    return "ms-appx:///Assets/book1.jpg";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
