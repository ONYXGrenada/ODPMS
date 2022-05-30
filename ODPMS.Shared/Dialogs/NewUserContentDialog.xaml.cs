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
using ODPMS.Models;
using ODPMS.Helpers;
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
            // Create a new ticket object and display on content dialog
            //NewUser = DatabaseHelper.CreateUser();

            //this.ticketNumber_txtBlock.Text = NewTicket.Number.ToString();
            //this.ticketDate_txtBlock.Text = NewTicket.Created.ToString("MM/dd/yyyy");
            //this.ticketTime_txtBlock.Text = NewTicket.Created.ToString("T");
            //this.ticketGreeting_txtBlock.Text = "Thank you for your business!";
            //this.ticketTerms_txtBlock.Text = String.Format("The hourly rate is {0}. Lost tickets will result in a full date charge of $18.00", NewTicket.Rate.ToString());
        }

        private void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Add the new ticket object to the database
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string password = BCrypt.Net.BCrypt.HashPassword(this.newPassword_txt.Password, salt);
            string userType = ((ComboBoxItem)this.userType_cb.SelectedItem).Tag.ToString();
            NewUser = new User(null, this.username_txt.Text, password, salt, this.firstName_txt.Text, 
                this.lastName_txt.Text, userType, "Active", null);
            DatabaseHelper.AddUser(NewUser);
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
