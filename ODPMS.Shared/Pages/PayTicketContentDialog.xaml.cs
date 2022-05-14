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

namespace ODPMS.Pages
{
	public sealed partial class PayTicketContentDialog : ContentDialog
	{
        private Ticket NewTicket;
        double payAmount;
        public PayTicketContentDialog()
		{
			this.InitializeComponent();
		}

        private void PayTicket_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // Get ticket object from database
            NewTicket = DatabaseHelper.FindTicket(3);

            this.TicketNumberText.Text = "Ticket Number: " + NewTicket.Number;
            this.TicketStartTimeText.Text = "Start Time: " + NewTicket.Created;
            this.TicketEndTimeText.Text = "End Time: " + NewTicket.Closed;
            this.TicketDurationText.Text = "Duration: " + NewTicket.Cost / NewTicket.Rate;
            this.TicketCostText.Text = "Cost: " + NewTicket.Cost;
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

            this.ChangeReturned.Text = (NewTicket.Cost - payAmount).ToString();
        }
    }
}
