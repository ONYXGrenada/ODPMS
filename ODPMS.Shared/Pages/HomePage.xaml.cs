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
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public ObservableCollection<Ticket> TicketList { get; set; }
        public HomePage()
        {
            this.InitializeComponent();
            TicketListDataGrid.Loaded += TicketListDataGrid_Loaded;
        }

        private void TicketListDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // Set focus so the first item of the listview has focus
            // instead of some item which is not visible on page load
            TicketListDataGrid.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TicketList = DatabaseHelper.GetTicketListViewData();
        }

        private async void NewTicket_Clicked(object sender, RoutedEventArgs e)
        {
            // Display the new ticket dialog
            ContentDialog ticketDialog = new NewTicketContentDialog();
            ticketDialog.XamlRoot = this.XamlRoot;
            await ticketDialog.ShowAsync();
        }
         
        private async void PayTicket_Clicked(object sender, RoutedEventArgs e)
        {
            // Display the pay ticket dialog
            ValidTicketMessage.Text = "";
            int ticketNumber;
            if (Int32.TryParse(this.TicketNumber.Text, out ticketNumber))
            {
                ticketNumber = Int32.Parse(this.TicketNumber.Text);
                if (DatabaseHelper.CheckTicket(ticketNumber))
                {
                    ContentDialog payDialog = new PayTicketContentDialog(ticketNumber);
                    payDialog.XamlRoot = this.XamlRoot;
                    await payDialog.ShowAsync();
                } 
                else
                {
                    ValidTicketMessage.Text = string.Format("The ticket number you entered does not exist or is not open.");
                }
            }
            else
            {
                ValidTicketMessage.Text = string.Format("That was not a valid ticket number.");
            }
        }

    }
}
