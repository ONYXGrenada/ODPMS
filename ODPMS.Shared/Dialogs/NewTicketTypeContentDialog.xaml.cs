using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        private async void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check for errors

            if (string.IsNullOrEmpty(this.description_txt.Text))
            {
                args.Cancel = true;
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Description is required.";
            }
            else if (string.IsNullOrEmpty(this.period_txt.Text))
            {
                args.Cancel = true;
                statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                statusMessage_txtBlock.Text = "Quantity is required.";
            }
            else if (string.IsNullOrEmpty(this.unitCost_txt.Text))
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
                double rate;
                Double.TryParse(this.unitCost_txt.Text, out rate);
                int period;
                Int32.TryParse(this.period_txt.Text, out period);
                string selectedStatus = this.typeStatus_cb.SelectionBoxItem.ToString();
                string user = App.LoggedInUser.Username;

                TicketType NewTicketType = new TicketType();
                NewTicketType.Type = type;
                NewTicketType.Description = description;
                NewTicketType.Period = period;
                NewTicketType.Rate = rate;
                NewTicketType.Status = selectedStatus;
                NewTicketType.User = user;
                NewTicketType.Created = DateTime.Now;
                NewTicketType.Updated = DateTime.Now;
                NewTicketType.UpdatedBy = user;
                NewTicketType.IsDeletable = true;

                await TicketType.CreateTicketType(NewTicketType);
            }
        }

        private void SecondaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
