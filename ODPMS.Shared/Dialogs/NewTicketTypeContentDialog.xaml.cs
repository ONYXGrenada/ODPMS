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
    public sealed partial class NewTicketTypeContentDialog : ContentDialog
    {
        public NewTicketTypeContentDialog()
        {
            this.InitializeComponent();
        }

        private void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check for errors

            if (string.IsNullOrEmpty(this.description_txt.Text))
            {
                args.Cancel = true;
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Description is required.";
            }
            if (string.IsNullOrEmpty(this.quantity_txt.Text))
            {
                args.Cancel = true;
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Quantity is required.";
            }
            if (string.IsNullOrEmpty(this.unitCost_txt.Text))
            {
                args.Cancel = true;
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Unit cost is required.";
            }
            else
            {
                // Add the new ticketType object to the database
                string type = this.ticketType_cb.SelectionBoxItem.ToString();
                string description = this.description_txt.Text;
                double unitCost;
                Double.TryParse(this.unitCost_txt.Text, out unitCost);
                int quantity;
                Int32.TryParse(this.quantity_txt.Text, out quantity);
                string selectedStatus = this.typeStatus_cb.SelectionBoxItem.ToString();
                string user = App.LoggedInUser.Username;
                DateTime activityDate = DateTime.Now;

                TicketType NewTicketType = new TicketType(null, type, description, quantity, unitCost, selectedStatus, user, activityDate);
                DatabaseHelper.AddTicketType(NewTicketType);
            }
        }

        private void SecondaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
