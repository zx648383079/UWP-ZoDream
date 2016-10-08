using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ZoDream.Layouts.List
{
    public sealed partial class HtmlSection : UserControl
    {
        #region Properties
        #region HtmlContent
        public string HtmlContent
        {
            get { return (string)GetValue(HtmlContentProperty); }
            set { SetValue(HtmlContentProperty, value); }
        }

        public static readonly DependencyProperty HtmlContentProperty = DependencyProperty.Register("HtmlContent", typeof(string), typeof(HtmlSection), new PropertyMetadata(string.Empty));
        #endregion

        #region Header
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(HtmlSection), new PropertyMetadata(null));
        #endregion

        #region HeaderTemplate
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HtmlSection), new PropertyMetadata(null));
        #endregion
        #endregion

        public HtmlSection()
        {
            DataContext = this;
            this.InitializeComponent();
        }
    }
}
