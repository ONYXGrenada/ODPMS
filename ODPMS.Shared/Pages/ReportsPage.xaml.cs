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
using System.Globalization;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ODPMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportsPage : Page
    {
        public ObservableCollection<Ticket> TicketList { get; set; }
        public ReportsPage()
        {
            this.InitializeComponent();
            //TicketListView.Loaded += TicketListView_Loaded;
        }

        //private void TicketListView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // Set focus so the first item of the listview has focus
        //    // instead of some item which is not visible on page load
        //    TicketListView.Focus(FocusState.Programmatic);
        //    int test = 0;
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //TicketListView.ItemsSource = DatabaseHelper.GetTicketListViewData();
        }

        private void ReportSubmit_Clicked(object sender, RoutedEventArgs e)
        {
            var cultureInfo = new CultureInfo("en-US");
            // Get requested data from database
            string fromDateStr = this.fromDate_pkr.SelectedDate.ToString();
            fromDateStr = fromDateStr.Substring(0, fromDateStr.IndexOf(" "));
            string fromTimeStr = this.fromTime_pkr.SelectedTime.ToString();
            DateTime fromDate = DateTime.Parse(fromDateStr + " " + fromTimeStr, cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

            string toDateStr = this.toDate_pkr.SelectedDate.ToString();
            toDateStr = toDateStr.Substring(0, toDateStr.IndexOf(" "));
            string toTimeStr = this.toTime_pkr.SelectedTime.ToString();
            DateTime toDate = DateTime.Parse(toDateStr + " " + toTimeStr, cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

            string status = "Paid";

            //string status = this.status_cbox.Items[this.status_cbox.SelectedIndex].ToString();


            //this.text_text.Text = status;

            //5/12/2022


            //string fromDateStr = this.fromDate_pkr.SelectedDate.ToString();
            //string toDateStr = this.toDate_pkr.SelectedDate.ToString();
            //string status = this.status_cbox.SelectedItem.ToString();
            TicketList = DatabaseHelper.GetTicketListRange(fromDate, toDate, status);
            TicketListDataGrid.ItemsSource = TicketList;
        }
    }
}
