using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ZoDream.Model;
using ZoDream.View;

namespace ZoDream.ViewModel
{
    public class ViewModelLocator
    {

        private static void Register<T>(bool createImmediately = false) where T : class
        {
            SimpleIoc.Default.Register<T>(createImmediately);
        }

        internal static T Get<T>() where T : class
        {
            return SimpleIoc.Default.GetInstance<T>();
        }

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new NavigationService();
            nav.Configure(typeof(WebPage).FullName, typeof(WebPage));


            SimpleIoc.Default.Register<INavigationService>(() => nav);

            SimpleIoc.Default.Register<IDialogService, DialogService>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            Register<MainViewModel>();
            Register<ChatViewModel>();
            Register<FriendViewModel>();
            Register<HistoryViewModel>();
        }
        
        public MainViewModel Main => Get<MainViewModel>();

        public ChatViewModel Chat => Get<ChatViewModel>();


        public FriendViewModel Friend => Get<FriendViewModel>();

        public HistoryViewModel History => Get<HistoryViewModel>();
    }
}
