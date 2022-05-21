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
        }   

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //TicketListView.ItemsSource = DatabaseHelper.GetTicketListViewData();
            this.fromDate_pkr.Date = DateTime.Now;
            this.toDate_pkr.Date = DateTime.Now;
            this.fromTime_pkr.Time = new TimeSpan(0, 0, 0, 0);
            this.toTime_pkr.Time = new TimeSpan(0, 23, 59, 59);
        }

        private void ReportSubmit_Clicked(object sender, RoutedEventArgs e)
        {
            var cultureInfo = new CultureInfo("en-US");
            // Create from date and time for use with database query
            string fromDateStr = this.fromDate_pkr.Date.ToString();
            string fromTimeStr = this.fromTime_pkr.SelectedTime.ToString();
            fromDateStr = fromDateStr.Substring(0, fromDateStr.IndexOf(" "));            
            DateTime fromDate = DateTime.Parse(fromDateStr + " " + fromTimeStr, cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

            // Create to date and time for use with database query
            string toDateStr = this.toDate_pkr.Date.ToString();
            string toTimeStr = this.toTime_pkr.SelectedTime.ToString();
            toDateStr = toDateStr.Substring(0, toDateStr.IndexOf(" "));            
            DateTime toDate = DateTime.Parse(toDateStr + " " + toTimeStr, cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

            //Retrieves status for use with database query
            string selectedStatus = this.status_cbox.SelectionBoxItem.ToString();

            TicketList = DatabaseHelper.GetTicketListRange(fromDate, toDate, selectedStatus);
            this.TicketListDataGrid.ItemsSource = TicketList;
        }
    }
}
