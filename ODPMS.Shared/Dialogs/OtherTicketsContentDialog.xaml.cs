using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Sockets;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using BarcodeLib;
using Windows.Storage.Streams;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Dialogs
{
    public sealed partial class OtherTicketsContentDialog : ContentDialog
    {
        ObservableCollection<TicketType> TicketTypesList = new ObservableCollection<TicketType>();
        private Ticket NewTicket;
        private TicketType NewTicketType;
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        private double payAmount;
        private int PayTicketNumber { get; set; }
        
        public OtherTicketsContentDialog()
        {
            this.InitializeComponent();
            Init();
            
        }

        async void Init()
        {
            // Get ticket types
            var ticketTypes = await TicketType.GetTicketTypesByStatus("Active");

            if (TicketTypesList.Count != 0)
                TicketTypesList.Clear();

            foreach (var ticketType in ticketTypes)
                if (ticketType.Type != "Hourly")
                {
                    this.ticketType_cb.Items.Add(ticketType.Description);
                    TicketTypesList.Add(ticketType);
                }

            this.ticketType_cb.SelectedIndex = 0;
            this.ticketType_cb.SelectionChanged += ticketType_cb_SelectionChanged;

            // Create new ticket object
            var tickets = await Ticket.GetAllTickets();

            NewTicket = new();
            if (tickets.Count == 0)
                NewTicket.Id = 1;
            else
                NewTicket.Id = tickets.Select(x => x.Id).Max() + 1;

            NewTicket.Created = DateTime.Now;
            NewTicket.Status = "Open";
            NewTicket.User = App.LoggedInUser.Username;
            NewTicket.Updated = DateTime.Now;
            NewTicket.UpdatedBy = App.LoggedInUser.Username;
            NewTicket.IsDeletable = true;


            // Set field values
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

            if (LocalSettings.Values["TicketMessage"] != null)
                this.ticketMessage_txtBlock.Text = LocalSettings.Values["TicketMessage"] as string;

            if (LocalSettings.Values["TicketDisclaimer"] != null)
                this.ticketDisclaimer_txtBlock.Text = LocalSettings.Values["TicketDisclaimer"] as string;

            this.ticketNumber_txtBlock.Text = NewTicket.Id.ToString();
            this.ticketDate_txtBlock.Text = NewTicket.Created.ToString("MM/dd/yyyy");
            this.ticketTime_txtBlock.Text = NewTicket.Created.ToString("T");
            generateBarCode(NewTicket.Id.ToString());
        }

        private async void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string registration = this.vehicleNum_txt.Text;
            string ticketDescription = this.ticketType_cb.Items[this.ticketType_cb.SelectedIndex].ToString();

            foreach (var ticketType in TicketTypesList)
                if (ticketType.Description == ticketDescription)
                    NewTicketType = ticketType;

            double payAmount;
            Double.TryParse(this.paymentAmount_txt.Text, out payAmount);
            int customerId = 0;

            //var tickets = await Ticket.GetAllTickets();

            //NewTicket = new();
            //if (tickets.Count == 0)
            //    NewTicket.Id = 1;
            //else
            //    NewTicket.Id = tickets.Select(x => x.Id).Max() + 1;

            NewTicket.Type = NewTicketType.Type;
            NewTicket.Description = NewTicketType.Description;
            //NewTicket.Created = DateTime.Now;
            //NewTicket.Status = "Open";
            NewTicket.CustomerId = customerId;
            NewTicket.Registration = registration;
            NewTicket.Period = NewTicketType.Period;
            NewTicket.Rate = NewTicketType.Rate;
            NewTicket.Cost = NewTicketType.Rate;
            NewTicket.Balance = NewTicketType.Rate;
            //NewTicket.User = App.LoggedInUser.Username;
            //NewTicket.Updated = DateTime.Now;
            //NewTicket.UpdatedBy = App.LoggedInUser.Username;
            //NewTicket.IsDeletable = true;

            NewTicket.UpdateClosed();

            if (payAmount > 0)
                NewTicket.PayTicket(payAmount);

            await Ticket.CreateTicket(NewTicket);
        }

        private void SecondaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void PayAmount_Changed(object sender, RoutedEventArgs e)
        {
            string selectedItem = this.ticketType_cb.Items[this.ticketType_cb.SelectedIndex].ToString();
            foreach (var ticketType in TicketTypesList)
                if (ticketType.Description == selectedItem)
                    NewTicketType = ticketType;

            if (NewTicketType.Description == "Hourly")
            {
                this.changeReturned_txtBlock.Text = "";
            }
            else
            {
                if (Double.TryParse(this.paymentAmount_txt.Text, out payAmount))
                {
                    payAmount = double.Parse(this.paymentAmount_txt.Text);
                }
                else
                {
                    payAmount = 0.0;
                }
                double change = NewTicketType.Rate - payAmount;
                if (change > 0)
                {
                    this.changeReturned_txtBlock.Text = string.Format("The customer still has {0} outstanding", change.ToString("C", CultureInfo.CurrentCulture));
                }
                else
                {
                    this.changeReturned_txtBlock.Text = string.Format("Please return {0} to the customer", (change * -1).ToString("C", CultureInfo.CurrentCulture));
                }
            }
            
        }

        private void ticketType_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = this.ticketType_cb.Items[this.ticketType_cb.SelectedIndex].ToString();

            foreach (var ticketType in TicketTypesList)
            {
                if (ticketType.Description == selectedItem)
                {
                    string fromDate = DateTime.Now.ToString("d MMMM, yyyy");
                    string toDate = ticketType.GetEndDate().ToString("d MMMM, yyyy");
                    
                    this.typeCost_txt.Text = ticketType.Rate.ToString("C", CultureInfo.CurrentCulture);
                    this.vehicleNum_txt.IsReadOnly = false;
                    this.paymentAmount_txt.IsReadOnly = false;

                    this.typePeriod.Text = fromDate + " - " + toDate;
                    break;
                }
            }                
        }

        public async void generateBarCode(String ticketNumber)
        {
            //Create barcode
            Barcode barcode = new Barcode();
            await ApplicationData.Current.LocalFolder.CreateFileAsync("ticket" + ticketNumber + ".jpg", CreationCollisionOption.OpenIfExists);
            string filePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ticket" + ticketNumber + ".jpg");
            //barcode.IncludeLabel = true;
            barcode.Encode(TYPE.CODE93, ticketNumber, 200, 100);
            barcode.SaveImage(filePath, SaveTypes.JPG);


            //Place barcode on ticket

            var barcodePath = await ApplicationData.Current.LocalFolder.GetFileAsync("ticket" + ticketNumber + ".jpg");
            using (IRandomAccessStream fileStream = await barcodePath.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(fileStream);

                this.ticketBarCode_img.Source = image;
            }
        }
    }
}
