using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Globalization;
using ODPMS.Models;
using ODPMS.Helpers;
using Windows.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.Linq;

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

        private async void ReportExport_Clicked(object sender, RoutedEventArgs e)
        {

            FileSavePicker picker = new();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeChoices.Add("Csv", new List<string>() { ".csv" });
            picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            picker.SuggestedFileName = "Activity Codes";
            picker.SettingsIdentifier = "settingsIdentifier";
            picker.DefaultFileExtension = ".csv";

            var hwnd = WindowNative.GetWindowHandle(App._window);  // App.m_window?
            InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile saveFile = await picker.PickSaveFileAsync();
            if (saveFile != null)
            {
                List<string> TicketListStr = new List<string>();
                TicketListStr.Add("Id,Number,Type,Description,Created,Closed,Status,Rate,Cost,Balance,User");
                // Save file was picked, you can now write in it
                foreach (Ticket ticket in TicketList)
                {
                    TicketListStr.Add(ticket.ToCsv());
                }

                await FileIO.WriteLinesAsync(saveFile, TicketListStr);

            }
            else
            {
                // No file was picked or the dialog was cancelled.
            }

            //myButton.Content = $"{file?.DisplayName}";
            //var test = new CsvHelper.CsvWriter(TicketList, CultureInfo.InvariantCulture);

            //var fileSavePicker = new FileSavePicker();
            //fileSavePicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            //fileSavePicker.SuggestedFileName = "myfile.txt";
            //fileSavePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt", ".text" });
            //StorageFile saveFile = await fileSavePicker.PickSaveFileAsync();
            //if (saveFile != null)
            //{
            //    // Save file was picked, you can now write in it
            //    await FileIO.WriteTextAsync(saveFile, "Hello, world!");
            //}
            //else
            //{
            //    // No file was picked or the dialog was cancelled.
            //}

            // Use this code to associate the dialog to the appropriate AppWindow by setting
            // the dialog's XamlRoot to the same XamlRoot as an element that is already present in the AppWindow.
            //var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //picker.FileTypeFilter.Add(".xlsx");
            //var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            //WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            //Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            //if (file != null)
            //{
            //    //SettingsHelper.ExcelFile = file;
            //    //this.dataFile_txt.Text = SettingsHelper.ExcelFile.Path;
            //}

            //using (var writer = new StreamWriter("C:\\Users\\ozimb\\Downloads\\file.csv"))
            //using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //{
            //    csv.WriteRecords(TicketList);
            //}


        }
    }
}
