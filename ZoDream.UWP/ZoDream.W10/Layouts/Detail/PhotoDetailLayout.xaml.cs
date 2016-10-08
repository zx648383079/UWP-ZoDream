using Windows.UI.Xaml;
namespace ZoDream.Layouts.Detail
{
    public sealed partial class PhotoDetailLayout : BaseDetailLayout
    {
        #region MaxH
        public double MaxW
        {
            get { return (double)GetValue(MaxWProperty); }
            set { SetValue(MaxWProperty, value); }
        }

        public static readonly DependencyProperty MaxWProperty = DependencyProperty.Register("MaxW", typeof(double), typeof(PhotoDetailLayout), new PropertyMetadata(0D));
        #endregion

        #region MaxH
        public double MaxH
        {
            get { return (double)GetValue(MaxHProperty); }
            set { SetValue(MaxHProperty, value); }
        }

        public static readonly DependencyProperty MaxHProperty = DependencyProperty.Register("MaxH", typeof(double), typeof(PhotoDetailLayout), new PropertyMetadata(0D));
        #endregion        

        public PhotoDetailLayout()
        {
            InitializeComponent();
        }

        private void ScrollViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            MaxH = e.NewSize.Height;
            MaxW = e.NewSize.Width;
        }
    }
}
