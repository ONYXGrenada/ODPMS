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
using Microsoft.UI;
using System.Collections.ObjectModel;
using System.Globalization;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Dialogs
{
    public sealed partial class OtherTicketsContentDialog : ContentDialog
    {
        ObservableCollection<TicketTypeViewModel> ticketTypes = new ObservableCollection<TicketTypeViewModel>();
        private Ticket NewTicket;
        private TicketTypeViewModel NewTicketType;
        private double payAmount;
        private int PayTicketNumber { get; set; }
        public OtherTicketsContentDialog()
        {
            this.InitializeComponent();
            this.ticketType_cb.SelectedIndex = 0;
            ticketTypes = DatabaseHelper.GetTicketTypeList("Active");

            foreach (var ticketType in ticketTypes)
                if (ticketType.Type != "Hourly")
                    this.ticketType_cb.Items.Add(ticketType.Description);
        }

        private void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string registration = this.vehicleNum_txt.Text;
            string ticketDescription = this.ticketType_cb.Items[this.ticketType_cb.SelectedIndex].ToString();

            foreach (var ticketType in ticketTypes)
                if (ticketType.Description == ticketDescription)
                    NewTicketType = ticketType;

            double payAmount;
            Double.TryParse(this.paymentAmount_txt.Text, out payAmount);
            int customerId = 0;

            NewTicket = DatabaseHelper.CreateTicket(ticketDescription, customerId, registration);
            NewTicket.PayTicket(payAmount);
            DatabaseHelper.AddTicket(NewTicket);
        }

        private void SecondaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void PayAmount_Changed(object sender, RoutedEventArgs e)
        {
            string selectedItem = this.ticketType_cb.Items[this.ticketType_cb.SelectedIndex].ToString();
            foreach (var ticketType in ticketTypes)
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

            foreach (var ticketType in ticketTypes)
            {
                if (ticketType.Description == selectedItem)
                {
                    string fromDate = DateTime.Now.ToString("d MMMM, yyyy");
                    string toDate = ticketType.GetEndDate().ToString("d MMMM, yyyy");

                    //if (ticketType.Type == "Hourly")
                    //{
                    //    this.typeCost_txt.Text = "To be determined";
                    //    this.vehicleNum_txt.IsReadOnly = true;
                    //    this.vehicleNum_txt.Text = "";
                    //    this.paymentAmount_txt.IsReadOnly = true;
                    //    this.paymentAmount_txt.Text = "";
                    //    this.changeReturned_txtBlock.Text = "";
                    //}
                    
                    this.typeCost_txt.Text = ticketType.Rate.ToString();
                    this.vehicleNum_txt.IsReadOnly = false;
                    this.paymentAmount_txt.IsReadOnly = false;
                                            

                    this.typePeriod.Text = fromDate + " - " + toDate;
                    break;
                }
            }                
        }

        private async void vehicleNum_txt_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
