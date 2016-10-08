using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ZoDream.Layouts.List
{
    public class ListLayoutBase : UserControl
    {
        #region ItemsSource
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(ListLayoutBase), new PropertyMetadata(null));
        #endregion

        #region OneRowModeEnabled
        public bool OneRowModeEnabled
        {
            get { return (bool)GetValue(OneRowModeEnabledProperty); }
            set { SetValue(OneRowModeEnabledProperty, value); }
        }

        public static readonly DependencyProperty OneRowModeEnabledProperty = DependencyProperty.Register("OneRowModeEnabled", typeof(bool), typeof(ListLayoutBase), new PropertyMetadata(false));
        #endregion

        #region ItemClickCommand
        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemClickCommandProperty = DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(ListLayoutBase), new PropertyMetadata(null));
        #endregion        

        #region EndOfScrollCommand
        public ICommand EndOfScrollCommand
        {
            get { return (ICommand)GetValue(EndOfScrollCommandProperty); }
            set { SetValue(EndOfScrollCommandProperty, value); }
        }

        public static readonly DependencyProperty EndOfScrollCommandProperty = DependencyProperty.Register("EndOfScrollCommand", typeof(ICommand), typeof(ListLayoutBase), new PropertyMetadata(null));
        #endregion        

        #region Header
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ListLayoutBase), new PropertyMetadata(null));
        #endregion

        #region HeaderTemplate
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ListLayoutBase), new PropertyMetadata(null));
        #endregion
    }
}
