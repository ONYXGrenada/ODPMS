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
using ODPMS.Helpers;
using Microsoft.UI;
using Windows.UI.Popups;
using ODPMS.Models;

namespace ODPMS.Dialogs
{
	public sealed partial class ResetPasswordContentDialog : ContentDialog
	{
        User User { get; set; }
        int? userId { get; set; }
		public ResetPasswordContentDialog(int? userId)
		{
            this.InitializeComponent();
            this.userId = userId;
            if (userId != null && userId != App.LoggedInUser.Id)
            {
                this.currentPassword_txtBlock.Visibility = Visibility.Collapsed;
                this.currentPassword_txt.Visibility = Visibility.Collapsed;
            }
		}

		private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
            if(userId != null && userId != App.LoggedInUser.Id)
            {
                User = DatabaseHelper.GetUser((int)this.userId);
                if (this.newPassword_txt.Password == this.newPasswordConfirmed_txt.Password)
                {
                    User.Salt = BCrypt.Net.BCrypt.GenerateSalt();
                    User.Password = BCrypt.Net.BCrypt.HashPassword(this.newPassword_txt.Password, User.Salt);
                    await User.UpdateUser(User);
                }
                else
                {
                    args.Cancel = true;
                    statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                    statusMessage_txtBlock.Text = "Please try again.";
                }
            } else
            {
                User = App.LoggedInUser;
                if (BCrypt.Net.BCrypt.HashPassword(currentPassword_txt.Password, User.Salt) == User.Password &&
                this.newPassword_txt.Password == this.newPasswordConfirmed_txt.Password)
                {
                    User.Salt = BCrypt.Net.BCrypt.GenerateSalt();
                    User.Password = BCrypt.Net.BCrypt.HashPassword(this.newPassword_txt.Password, User.Salt);
                    await User.UpdateUser(User);
                }
                else
                {
                    args.Cancel = true;
                    statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                    statusMessage_txtBlock.Text = "Please try again.";
                }
            }
        }

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

        private void NewPassword_Changed(object sender, RoutedEventArgs e)
        {
            if (this.newPassword_txt.Password != this.newPasswordConfirmed_txt.Password)
            {
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Passwords do not match. Please try again.";
            } else
            {
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Black);
                statusMessage_txtBlock.Text = "";
            }
        }
    }
}
