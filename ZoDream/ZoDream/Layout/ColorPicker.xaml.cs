using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ZoDream.Layout
{
    public sealed partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            this.InitializeComponent();
        }

        public static Color ToColor(string color)
        {
            var returnColor = Colors.Black;
            if (string.IsNullOrEmpty(color)) return returnColor;
            if (color[0] == '#')
            {
                // #fff;
                if (color.Length == 4)
                {
                    return Color.FromArgb(255, Convert.ToByte(color.Substring(1, 1), 16), Convert.ToByte(color.Substring(2, 1), 16), Convert.ToByte(color.Substring(3, 1), 16));
                }
                return Color.FromArgb(255, Convert.ToByte(color.Substring(1,2), 16), Convert.ToByte(color.Substring(3, 2), 16), Convert.ToByte(color.Substring(5, 2), 16));
            }
            var ms = Regex.Matches(color, @"\d+");
            switch (ms.Count)
            {
                case 3:
                    //255,255,255
                    returnColor = Color.FromArgb(255, Convert.ToByte(ms[0].Value), Convert.ToByte(ms[1].Value),
                        Convert.ToByte(ms[2].Value));
                    break;
                case 4:
                    //0,0,0,0.1
                    returnColor = Color.FromArgb(Convert.ToByte(Convert.ToDouble(ms[3].Value) * 255), Convert.ToByte(ms[0].Value), Convert.ToByte(ms[1].Value),
                        Convert.ToByte(ms[2].Value));
                    break;
            }
            return returnColor;
        }



        public Color ChooseColor
        {
            get { return (Color)GetValue(ChooseColorProperty); }
            set { SetValue(ChooseColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChooseColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChooseColorProperty =
            DependencyProperty.Register("ChooseColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Black));


    }
}
