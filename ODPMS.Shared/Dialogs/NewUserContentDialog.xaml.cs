using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using System.Collections.ObjectModel;

namespace ODPMS.Dialogs
{
	public sealed partial class NewUserContentDialog : ContentDialog
	{
        private User NewUser;
        public NewUserContentDialog()
		{
			this.InitializeComponent();
            this.userType_cb.SelectedIndex = 1;
        }

        private void NewUser_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
        }

        private async void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check for errors

            if (string.IsNullOrEmpty(this.username_txt.Text))
            {
                args.Cancel = true;
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Username is required.";
            }
            else if (string.IsNullOrEmpty(this.newPassword_txt.Password) || string.IsNullOrEmpty(this.newPasswordConfirmed_txt.Password))
            {
                args.Cancel = true;
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Password is required.";
            }
            else if (string.IsNullOrEmpty(((ComboBoxItem)this.userType_cb.SelectedItem).Tag.ToString()))
            {
                args.Cancel = true;
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "User Type is required.";
            } else
            {
                // Add the new ticket object to the database
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                string password = BCrypt.Net.BCrypt.HashPassword(this.newPasswordConfirmed_txt.Password, salt);
                string userType = ((ComboBoxItem)this.userType_cb.SelectedItem).Tag.ToString();

                NewUser = new();
                NewUser.Username = this.username_txt.Text;
                NewUser.Password = password;
                NewUser.Salt = salt;
                NewUser.FirstName = this.firstName_txt.Text;
                NewUser.LastName = this.lastName_txt.Text;
                NewUser.Type = userType;
                NewUser.Status = "Active";
                NewUser.IsReset = true;
                NewUser.Updated = DateTime.Now;
                NewUser.UpdatedBy = App.LoggedInUser.Username;
                NewUser.IsDeletable = true;
                await User.CreateUser(NewUser);                
            }
        }

        private void CloseButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Discard the new ticket object
        }

        private void Password_Changed(object sender, RoutedEventArgs e)
        {
            if (this.newPassword_txt.Password != this.newPasswordConfirmed_txt.Password)
            {
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Passwords do not match. Please try again.";
            }
            else
            {
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Black);
                statusMessage_txtBlock.Text = "";
            }
        }

        private void LastName_TextChanged(object sender, RoutedEventArgs e)
        {
            if (this.firstName_txt.Text != null && this.lastName_txt.Text != null)
                this.username_txt.Text = this.firstName_txt.Text.Substring(0, 1).ToLower() + this.lastName_txt.Text.ToLower();
        }
    }
}
