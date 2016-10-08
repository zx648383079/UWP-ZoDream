using AppStudio.Uwp.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace ZoDream.Layouts.Detail
{
    public sealed partial class RelatedContentDetailLayout : BaseDetailLayout
    {
        #region RelatedContentTemplate
        public DataTemplate RelatedContentTemplate
        {
            get { return (DataTemplate)GetValue(RelatedContentTemplateProperty); }
            set { SetValue(RelatedContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty RelatedContentTemplateProperty = DependencyProperty.Register("RelatedContentTemplate", typeof(DataTemplate), typeof(BaseDetailLayout), new PropertyMetadata(null));
        #endregion        

        public RelatedContentDetailLayout()
        {
            InitializeComponent();
        }
    }
}
