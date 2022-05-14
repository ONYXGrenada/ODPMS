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

namespace ODPMS.Pages
{
	public sealed partial class NewTicketContentDialog : ContentDialog
	{
		private Ticket NewTicket;
		public NewTicketContentDialog()
		{
			this.InitializeComponent();
		}

		private void NewTicket_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // Create a new ticket object and display on content dialog
            NewTicket = DatabaseHelper.CreateTicket();

			this.ticketNumber_txt.Text = NewTicket.Number.ToString();
            this.ticketDescription_txt.Text = NewTicket.Description.ToString();
            this.ticketCreated_txt.Text = NewTicket.Created.ToString();
            this.ticketRate_txt.Text = NewTicket.Rate.ToString();
        }
		private void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			// Add the new ticket object to the database
			DatabaseHelper.AddTicket(NewTicket);
		}

		private void CloseButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			// Discard the new ticket object
		}
	}
}
