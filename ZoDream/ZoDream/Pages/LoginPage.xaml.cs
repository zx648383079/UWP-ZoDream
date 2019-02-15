using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZoDream.Models.Api;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ZoDream.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private UserApi userApi = new UserApi();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginAsync();
            //Frame.Navigate(typeof(ProfilePage));
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(username.Text))
            {
                username.Focus(FocusState.Keyboard);
                return;
            }
            if (string.IsNullOrWhiteSpace(password.Password))
            {
                password.Focus(FocusState.Keyboard);
                return;
            }
            var data = await userApi.Login(username.Text, password.Password);

        }
    }
}
