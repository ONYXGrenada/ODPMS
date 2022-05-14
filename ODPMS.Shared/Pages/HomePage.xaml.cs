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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            TicketList.Loaded += TicketList_Loaded;
        }

        private void TicketList_Loaded(object sender, RoutedEventArgs e)
        {
            // Set focus so the first item of the listview has focus
            // instead of some item which is not visible on page load
            TicketList.Focus(FocusState.Programmatic);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            TicketList.ItemsSource = await Ticket.GetTicketsAsync();
        }

        private async void NewTicket_Clicked(object sender, RoutedEventArgs e)
        {
            //DisplayTicketDialog();
            ContentDialog ticketDialog = new NewTicketContentDialog();
            ticketDialog.XamlRoot = this.XamlRoot;
            await ticketDialog.ShowAsync();
        }
         
        private void PayTicket_Clicked(object sender, RoutedEventArgs e)
        {

        }

        //private async void DisplayTicketDialog()
        //{
        //    ContentDialog ticketDialog = new ContentDialog();
        //    ticketDialog.Content = new NewTicketContentDialog();

        //    //await ticketDialog.ShowAsync();
        //}

    }
}
