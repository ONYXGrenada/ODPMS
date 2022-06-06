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
    public sealed partial class NewSpecialTicketContentDialog : ContentDialog
    {
        ObservableCollection<TicketTypeViewModel> ticketTypes = new ObservableCollection<TicketTypeViewModel>();
        private Ticket NewTicket;
        private double payAmount;
        private int PayTicketNumber { get; set; }

        public NewSpecialTicketContentDialog()
        {
            this.InitializeComponent();
            ticketTypes = DatabaseHelper.GetTicketTypeList("Active");

            foreach (var ticketType in ticketTypes)
                this.ticketType_cb.Items.Add(ticketType.Description);
        }

        private void PrimaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void SecondaryButton_Clicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void PayAmount_Changed(object sender, RoutedEventArgs e)
        {
            if (Double.TryParse(this.paymentAmount_txt.Text, out payAmount))
            {
                payAmount = double.Parse(this.paymentAmount_txt.Text);
            }
            else
            {
                payAmount = 0.0;
            }
            double change = NewTicket.Cost - payAmount;
            if (change > 0)
            {
                this.changeReturned_txtBlock.Text = string.Format("The customer still has {0} outstanding", change.ToString("C", CultureInfo.CurrentCulture));
            }
            else
            {
                this.changeReturned_txtBlock.Text = string.Format("Please return {0} to the customer", (change * -1).ToString("C", CultureInfo.CurrentCulture));
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
                    string toDate = DateTime.Now.AddDays(ticketType.Quantity).ToString("d MMMM, yyyy");
                    this.typeCost_txt.Text = ticketType.UnitCost.ToString();
                    this.typePeriod.Text = fromDate + " - " + toDate;
                    break;
                }
            }                
        }
    }
}
