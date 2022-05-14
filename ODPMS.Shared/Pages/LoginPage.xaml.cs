using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            App._window.ExtendsContentIntoTitleBar = true;
            App._window.SetTitleBar(AppTitleBar);
        }

        private void Login_Clicked(object sender, RoutedEventArgs e)
        {
            App.IsUserLoggedIn = Login(Username.Text, Password.Password);
            if (App.IsUserLoggedIn)
            {
                Frame rootFrame = new Frame();
                rootFrame.Navigate(typeof(MainPage));
                App._window.Content = rootFrame;
            } else
            {
                StatusMessage.Foreground = new SolidColorBrush(Colors.Red);
                StatusMessage.Text = "Invalid login information. Please try again";
            }
        }

        private void Password_Changed(object sender, RoutedEventArgs e)
        {
            StatusMessage.Foreground = new SolidColorBrush(Colors.Black);
            StatusMessage.Text = "";
        }

        private void Key_Pressed(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Login_Clicked(sender, e);
            }
        }

        private bool Login(string username, string password)
        {
            if (username == "Admin" && password == "admin")
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
