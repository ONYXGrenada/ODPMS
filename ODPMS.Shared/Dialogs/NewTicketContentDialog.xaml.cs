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
using BarcodeLib;
using Windows.Storage;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace ODPMS.Dialogs
{
	public sealed partial class NewTicketContentDialog : ContentDialog
	{
		private Ticket NewTicket;
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        public NewTicketContentDialog()
		{
			this.InitializeComponent();
			Init();
		}

		void Init()
		{
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
                this.ticketGreeting_txtBlock.Text = LocalSettings.Values["TicketMessage"] as string;

            if (LocalSettings.Values["TicketDisclaimer"] != null)
                this.ticketDisclaimer_txtBlock.Text = LocalSettings.Values["TicketDisclaimer"] as string;
        }

		private async void NewTicket_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
		{
			// Create a new ticket object and display on content dialog

			//NewTicket = DatabaseHelper.CreateTicket("Hourly Ticket", 0, "0");
			var tickets = await Ticket.GetAllTickets();
			var ticketType = await TicketType.GetTicketType(1);

            NewTicket = new();
			if (tickets.Count == 0)
				NewTicket.Id = 1;
			else
				NewTicket.Id = tickets.Select(x => x.Id).Max() + 1;
			NewTicket.Type = "Hourly";
			NewTicket.Description = ticketType.Description;
            NewTicket.Created = DateTime.Now;
			NewTicket.Status = "Open";
			NewTicket.Rate = ticketType.Rate;
            NewTicket.Period = ticketType.Period;
            NewTicket.User = App.LoggedInUser.Username;
            NewTicket.Updated = DateTime.Now;
            NewTicket.UpdatedBy = App.LoggedInUser.Username;
            NewTicket.IsDeletable = true;

            this.ticketNumber_txtBlock.Text = NewTicket.Id.ToString();
			this.ticketDate_txtBlock.Text = NewTicket.Created.ToString("MM/dd/yyyy");
			this.ticketTime_txtBlock.Text = NewTicket.Created.ToString("T");
			//this.ticketGreeting_txtBlock.Text = "Thank you for your business!";
			//this.ticketDisclaimer_txtBlock.Text = String.Format("The hourly rate is {0}. " +
   //             "Lost tickets will result in a full date charge of $18.00", NewTicket.Rate.ToString());
			generateBarCode(NewTicket.Id.ToString());
        }

		private async void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			// Add the new ticket object to the database
			await Ticket.CreateTicket(NewTicket);
		}

		private void CloseButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			// Discard the new ticket object
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
