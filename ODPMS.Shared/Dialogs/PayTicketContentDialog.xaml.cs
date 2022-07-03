using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Globalization;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System.Windows.Input;
using Microsoft.UI;

namespace ODPMS.Dialogs
{
	public sealed partial class PayTicketContentDialog : ContentDialog
	{
        private Ticket ticket;
        private Receipt receipt;
        private bool printReceipt;
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        private double payAmount;
        private int PayTicketNumber { get; set; }
        public PayTicketContentDialog(int PayTicketNumber)
		{
			this.InitializeComponent();
            this.PayTicketNumber = PayTicketNumber;
            Init();
		}

        //private async void PayTicket_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        //{
        //    // Get ticket object from database
        //    ticket = await Ticket.GetTicket(this.PayTicketNumber);

        //    ticket.UpdateCost();

        //    this.ticketNumber_txtBlock.Text = "Ticket Number: " + ticket.Id;
        //    this.ticketStatus_txtBlock.Text = "Status: " + ticket.Status;
        //    this.ticketStartTime_txtBlock.Text = "Start Time: " + ticket.Created;
        //    this.ticketEndTime_txtBlock.Text = "End Time: " + ticket.Closed;
        //    this.ticketDuration_txtBlock.Text = "Duration: " + ticket.Cost / ticket.Rate + " Hours";
        //    this.ticketCost_txtBlock.Text = "Cost: " + ticket.Cost.ToString("C", CultureInfo.CurrentCulture);
        //}

        async void Init()
        {
            // Get Ticket
            ticket = await Ticket.GetTicket(this.PayTicketNumber);
            ticket.UpdateCost();

            var receipts = await Receipt.GetAllReceipts();

            // New Receipt
            receipt = new();
            if (receipts.Count == 0)
                receipt.Id = 1;
            else
                receipt.Id = receipts.Select(x => x.Id).Max() + 1;
            receipt.TicketNumber = ticket.Id;
            receipt.TicketType = ticket.Type;
            receipt.Created = DateTime.Now;
            receipt.Cost = ticket.Cost;
            receipt.User = App.LoggedInUser.Username;

            this.ticketNumber_txtBlock.Text = "Ticket Number: " + ticket.Id;
            this.ticketStatus_txtBlock.Text = "Status: " + ticket.Status;
            this.ticketStartTime_txtBlock.Text = "Start Time: " + ticket.Created;
            this.ticketEndTime_txtBlock.Text = "End Time: " + ticket.Closed;
            this.ticketDuration_txtBlock.Text = "Duration: " + ticket.Cost / ticket.Rate + " Hours";
            this.ticketCost_txtBlock.Text = "Cost: " + ticket.Cost.ToString("C", CultureInfo.CurrentCulture);
            
            this.receiptNumber_txtBlock.Text = receipt.Id.ToString();
            this.receiptDate_txtBlock.Text = receipt.Created.ToString("MM/dd/yyyy");
            this.receiptTime_txtBlock.Text = receipt.Created.ToString("T");
            this.ticketNumberReceipt_txtBlock.Text = receipt.TicketNumber.ToString();
            this.receiptCost_txtBlock.Text = receipt.Cost.ToString("C", CultureInfo.CurrentCulture);
            this.receiptPaid_txtBlock.Text = receipt.Paid.ToString("C", CultureInfo.CurrentCulture);
            this.receiptBalance_txtBlock.Text = receipt.Balance.ToString("C", CultureInfo.CurrentCulture);

            if (LocalSettings.Values["CompanyName"] != null)
                this.companyName_txtBlock.Text = LocalSettings.Values["CompanyName"] as string;

            if (LocalSettings.Values["CompanyAddress"] != null)
                this.companyAddress_txtBlock.Text = LocalSettings.Values["CompanyAddress"] as string;

            if (LocalSettings.Values["CompanyEmail"] != null)
                this.companyEmail_txtBlock.Text = LocalSettings.Values["CompanyEmail"] as string;

            if (LocalSettings.Values["CompanyPhone"] != null)
                this.companyPhone_txtBlock.Text = LocalSettings.Values["CompanyPhone"] as string;

            if (LocalSettings.Values["CompanyLogo"] != null)
            {
                string clogo = LocalSettings.Values["CompanyLogo"] as string;
                if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\" + clogo))
                {
                    Uri resourceUri = new Uri(ApplicationData.Current.LocalFolder.Path + "\\" + clogo, UriKind.Relative);
                    this.companyLogo_img.Source = new BitmapImage(resourceUri);
                }
            }

            if (LocalSettings.Values["ReceiptMessage"] != null)
                this.receiptMessage_txtBlock.Text = LocalSettings.Values["ReceiptMessage"] as string;

            if (LocalSettings.Values["ReceiptDisclaimer"] != null)
                this.receiptDisclaimer_txtBlock.Text = LocalSettings.Values["ReceiptDisclaimer"] as string;
        }

        

        private async void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Pay execute the pay function to display change and update ticket in the database
            if (Double.TryParse(this.paymentAmount_txt.Text, out payAmount))
                payAmount = double.Parse(this.paymentAmount_txt.Text);
            else
                payAmount = 0.0;

            // Attempt to print receipt
            if (printReceipt)
            {
                string printStatus = receipt.ToPrint();
                if (printStatus == "Success")
                {
                    ticket.PayTicket(payAmount);
                    await Ticket.UpdateTicket(ticket);
                    await Receipt.CreateReceipt(receipt);
                }
                else
                {
                    args.Cancel = true;
                    statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Red);
                    statusMessage_txtBlock.Text = printStatus;
                }
            }
            else
            {
                ticket.PayTicket(payAmount);
                await Ticket.UpdateTicket(ticket);
                await Receipt.CreateReceipt(receipt);
            }
        }

        private void CloseButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Discard the new ticket object and cancel payment.
        }

        private void PayAmount_Changed(object sender, RoutedEventArgs e)
        {
            statusMessage_txtBlock.Foreground = new SolidColorBrush(Colors.Black);
            statusMessage_txtBlock.Text = "";
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
                this.receipt.Paid = payAmount;
                this.receipt.Balance = change;
                this.receiptPaid_txtBlock.Text = receipt.Paid.ToString("C", CultureInfo.CurrentCulture);
                this.receiptBalance_txtBlock.Text = receipt.Balance.ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                this.changeReturned_txtBlock.Text = string.Format("Please return {0} to the customer", (change*-1).ToString("C", CultureInfo.CurrentCulture));
                this.receipt.Paid = payAmount;
                this.receipt.Balance = change;
                this.receiptPaid_txtBlock.Text = receipt.Paid.ToString("C", CultureInfo.CurrentCulture);
                this.receiptBalance_txtBlock.Text = receipt.Balance.ToString("C", CultureInfo.CurrentCulture);
            }
        }

        private void PrintReceipt_Checked(object sender, RoutedEventArgs e)
        {
            printReceipt = true;
            if (this.statusMessage_txtBlock != null)
                this.statusMessage_txtBlock.Text = null;
        }
        
        private void PrintReceipt_Unchecked(object sender, RoutedEventArgs e)
        {
            printReceipt = false;
            if (this.statusMessage_txtBlock != null)
                this.statusMessage_txtBlock.Text = null;            
        }
    }
}
