using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ZoDream.Layouts.Controls
{
    public sealed partial class SectionListItemHeader : UserControl
    {
        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SectionListItemHeader), new PropertyMetadata(string.Empty));
        #endregion

        public SectionListItemHeader()
        {
            this.InitializeComponent();
        }
    }
}
