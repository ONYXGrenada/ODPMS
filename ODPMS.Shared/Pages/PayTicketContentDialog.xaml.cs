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

namespace ODPMS.Pages
{
	public sealed partial class PayTicketContentDialog : ContentDialog
	{
        private Ticket NewTicket;
        private double payAmount;
        private int PayTicketNumber { get; set; }
        public PayTicketContentDialog(int PayTicketNumber)
		{
			this.InitializeComponent();
            this.PayTicketNumber = PayTicketNumber;
		}

        private void PayTicket_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // Get ticket object from database
            NewTicket = DatabaseHelper.FindTicket(this.PayTicketNumber);

            this.TicketNumberText.Text = "Ticket Number: " + NewTicket.Number;
            this.TicketStatusText.Text = "Status: " + NewTicket.Status;
            this.TicketStartTimeText.Text = "Start Time: " + NewTicket.Created;
            this.TicketEndTimeText.Text = "End Time: " + NewTicket.Closed;
            this.TicketDurationText.Text = "Duration: " + NewTicket.Cost / NewTicket.Rate + " Hours";
            this.TicketCostText.Text = "Cost: " + NewTicket.Cost.ToString("C", CultureInfo.CurrentCulture);
        }
        private void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Pay execute the pay function to display change and update ticket in the database
            //NewTicket.Status = "Paid";
            DatabaseHelper.PayTicket(NewTicket);
        }

        private void CloseButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Discard the new ticket object and cancel payment.
        }

        private void PayAmount_Changed(object sender, RoutedEventArgs e)
        {            
            if (Double.TryParse(this.PaymentAmount.Text,out payAmount))
            {
                payAmount = double.Parse(this.PaymentAmount.Text);
            }
            else
            {
                payAmount = 0.0;
            }
            double change = NewTicket.Cost - payAmount;
            if (change > 0)
            {
                this.ChangeReturned.Text = string.Format("The customer still has {0} outstanding", change.ToString("C", CultureInfo.CurrentCulture));
            }
            else
            {
                this.ChangeReturned.Text = string.Format("Please return {0} to the customer", (change*-1).ToString("C", CultureInfo.CurrentCulture));
            }
        }
    }
}
