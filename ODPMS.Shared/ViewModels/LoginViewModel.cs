using Microsoft.UI;

namespace ODPMS.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        string statusMessage;

        [ObservableProperty]
        SolidColorBrush statusMessageColor;

        [ICommand]
        private async void Login()
        {
            App.LoggedInUser = await User.Login(Username, Password);

            if (App.LoggedInUser != null)
                App.IsUserLoggedIn = true;
            else
                App.IsUserLoggedIn = false;

            if (App.IsUserLoggedIn)
            {
                if (App.LoggedInUser.IsReset)
                {
                    ContentDialog resetPasswordDialog = new ResetPasswordContentDialog(App.LoggedInUser.Id);
                    resetPasswordDialog.XamlRoot = (Application.Current as App)?.Window.Content.XamlRoot;
                    ContentDialogResult result = await resetPasswordDialog.ShowAsync();

                    if (result == ContentDialogResult.Secondary)
                    {
                        Application.Current.Exit();
                        return;
                    }
                    else
                    {
                        App.LoggedInUser.IsReset = false;
                        await User.UpdateUser(App.LoggedInUser);
                    }
                }
                Frame rootFrame = new Frame();
                rootFrame.Navigate(typeof(MainPage));
                Window window = (Application.Current as App)?.Window;
                window.Content = rootFrame;
            }
            else
            {
                StatusMessageColor = new SolidColorBrush(Colors.Red);
                StatusMessage = "Invalid login information. Please try again";
            }
        }

        [ICommand]
        private void PasswordChanged()
        {
            StatusMessageColor = new SolidColorBrush(Colors.Black);
            StatusMessage = "";
        }
    }
}
