using System;
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
using System.Globalization;

namespace ODPMS.Dialogs
{
	public sealed partial class PayTicketContentDialog : ContentDialog
	{
        private Ticket ticket;
        private double payAmount;
        private int PayTicketNumber { get; set; }
        public PayTicketContentDialog(int PayTicketNumber)
		{
			this.InitializeComponent();
            this.PayTicketNumber = PayTicketNumber;
		}

        private async void PayTicket_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // Get ticket object from database
            //ticket = DatabaseHelper.FindTicket(this.PayTicketNumber);
            ticket = await Ticket.GetTicket(this.PayTicketNumber);

            ticket.UpdateCost();

            this.ticketNumber_txtBlock.Text = "Ticket Number: " + ticket.Id;
            this.ticketStatus_txtBlock.Text = "Status: " + ticket.Status;
            this.ticketStartTime_txtBlock.Text = "Start Time: " + ticket.Created;
            this.ticketEndTime_txtBlock.Text = "End Time: " + ticket.Closed;
            this.ticketDuration_txtBlock.Text = "Duration: " + ticket.Cost / ticket.Rate + " Hours";
            this.ticketCost_txtBlock.Text = "Cost: " + ticket.Cost.ToString("C", CultureInfo.CurrentCulture);
        }
        private async void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Pay execute the pay function to display change and update ticket in the database
            if (Double.TryParse(this.paymentAmount_txt.Text, out payAmount))
            {
                payAmount = double.Parse(this.paymentAmount_txt.Text);
            }
            else
            {
                payAmount = 0.0;
            }
            ticket.PayTicket(payAmount);
            await Ticket.UpdateTicket(ticket);
            //DatabaseHelper.PayTicket(ticket);
        }

        private void CloseButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Discard the new ticket object and cancel payment.
        }

        private void PayAmount_Changed(object sender, RoutedEventArgs e)
        {            
            if (Double.TryParse(this.paymentAmount_txt.Text,out payAmount))
            {
                payAmount = double.Parse(this.paymentAmount_txt.Text);
            }
            else
            {
                payAmount = 0.0;
            }
            double change = ticket.Cost - payAmount;
            if (change > 0)
            {
                this.changeReturned_txtBlock.Text = string.Format("The customer still has {0} outstanding", change.ToString("C", CultureInfo.CurrentCulture));
            }
            else
            {
                this.changeReturned_txtBlock.Text = string.Format("Please return {0} to the customer", (change*-1).ToString("C", CultureInfo.CurrentCulture));
            }
        }
    }
}
