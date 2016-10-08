using System.ComponentModel;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ZoDream.Layouts.Controls
{
    public sealed partial class HeroImage : UserControl, INotifyPropertyChanged
    {
        #region Image
        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(string), typeof(HeroImage), new PropertyMetadata(string.Empty));
        #endregion

        #region MobileImage
        public string MobileImage
        {
            get { return (string)GetValue(MobileImageProperty); }
            set { SetValue(MobileImageProperty, value); }
        }

        public static readonly DependencyProperty MobileImageProperty = DependencyProperty.Register("MobileImage", typeof(string), typeof(HeroImage), new PropertyMetadata(string.Empty));
        #endregion        

        private string _source;        

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public HeroImage()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            SetImageSource();
            Window.Current.SizeChanged += ControlSizeChanged;
        }

        private void ControlSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SetImageSource();
        }

        private void SetImageSource()
        {
            if (Window.Current.Bounds.Width < 500 && !string.IsNullOrEmpty(MobileImage))
            {
                Source = MobileImage;
            }
            else
            {
                Source = Image;
            }
            SetPropertyChanged("Source");
        }

        private void ControlUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= ControlSizeChanged;
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
