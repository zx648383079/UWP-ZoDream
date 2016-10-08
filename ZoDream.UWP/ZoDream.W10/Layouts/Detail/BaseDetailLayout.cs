using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ZoDream.ViewModels;

namespace ZoDream.Layouts.Detail
{
    public abstract class BaseDetailLayout : UserControl
    {
        #region ViewModel
        public DetailViewModel ViewModel
        {
            get { return (DetailViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(DetailViewModel), typeof(BaseDetailLayout), new PropertyMetadata(null));
        #endregion        

        public BaseDetailLayout()
        {
        }
    }
}
