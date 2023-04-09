using System.Globalization;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ODPMS.ViewModels
{
    public partial class ReportsViewModel : BaseViewModel
    {
        [ObservableProperty]
        bool isNotEmpty;

        public ObservableCollection<Ticket> TicketList { get; } = new();
        public ObservableCollection<Receipt> ReceiptList { get; } = new();
        public ObservableCollection<ComboBoxItem> StatusList { get; } = new();
        public ObservableCollection<ComboBoxItem> ReportTypeList { get; } = new();

        [ObservableProperty]
        System.Nullable<DateTimeOffset> fromDate;

        [ObservableProperty]
        TimeSpan fromTime;

        [ObservableProperty]
        System.Nullable<DateTimeOffset> toDate;

        [ObservableProperty]
        TimeSpan toTime;

        [ObservableProperty]
        ComboBoxItem selectedStatus;

        [ObservableProperty]
        ComboBoxItem selectedReport;

        [ObservableProperty]
        Visibility visibleTicketList;

        [ObservableProperty]
        Visibility visibleReceiptList;

        [ObservableProperty]
        bool statusIsEnabled;

        public ReportsViewModel()
        {
            Title = "Reports";
            Init();
        }

        private void Init()
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            FromTime = new TimeSpan(0, 0, 0, 0);
            ToTime = new TimeSpan(0, 23, 59, 59);

            ReportTypeList.Add(new ComboBoxItem() { Name = "Receipts", Content = "Receipts" });
            ReportTypeList.Add(new ComboBoxItem() { Name = "Tickets", Content = "Tickets" });
            SelectedReport = ReportTypeList[0];

            StatusList.Add(new ComboBoxItem() { Name = "All", Content = "All" });            
            StatusList.Add(new ComboBoxItem() { Name = "Closed", Content = "Closed"});
            StatusList.Add(new ComboBoxItem() { Name = "Delete", Content = "Delete" });
            StatusList.Add(new ComboBoxItem() { Name = "Open", Content = "Open" });
            StatusList.Add(new ComboBoxItem() { Name = "Paid", Content = "Paid"});
            SelectedStatus = StatusList[0];

            VisibleTicketList = Visibility.Collapsed;
            VisibleReceiptList = Visibility.Collapsed;

            StatusIsEnabled = false;
        }

        [RelayCommand]
        private async void ReportSubmit()
        {
            IsNotEmpty = false;
            var cultureInfo = new CultureInfo("en-US");
            // Create from date and time for use with database query
            string fromDateStr = FromDate.ToString();
            string fromTimeStr = FromTime.ToString();
            fromDateStr = fromDateStr.Substring(0, fromDateStr.IndexOf(" "));
            DateTime fromDate = DateTime.Parse(fromDateStr + " " + fromTimeStr, cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

            // Create to date and time for use with database query
            string toDateStr = ToDate.ToString();
            string toTimeStr = ToTime.ToString();
            toDateStr = toDateStr.Substring(0, toDateStr.IndexOf(" "));
            DateTime toDate = DateTime.Parse(toDateStr + " " + toTimeStr, cultureInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

            //Retrieves status for use with database query
            string status = SelectedStatus.Name.ToString();

            //var tickets = DatabaseHelper.GetTicketListRange(fromDate, toDate, status);
            if (SelectedReport.Name == "Tickets")
            {
                var tickets = await Ticket.GetTicketsByReportFilter(fromDate, toDate, status);

                if (TicketList.Count != 0)
                    TicketList.Clear();

                foreach (var ticket in tickets)
                    TicketList.Add(ticket);

                if (TicketList.Count > 0)
                    IsNotEmpty = true;

                VisibleTicketList = Visibility.Visible;
                VisibleReceiptList = Visibility.Collapsed;
            }

            else if (SelectedReport.Name == "Receipts")
            {
                var receipts = await Receipt.GetReceiptsByReportFilter(fromDate, toDate, status);

                if (ReceiptList.Count != 0)
                    ReceiptList.Clear();

                foreach (var receipt in receipts)
                    ReceiptList.Add(receipt);

                if (ReceiptList.Count > 0)
                    IsNotEmpty = true;

                VisibleTicketList = Visibility.Collapsed;
                VisibleReceiptList = Visibility.Visible;
            }
        }

        [RelayCommand]
        private async void ReportExport()
        {
            string suggFileName = "";
            List<string> ExportListStr = new List<string>();

            if (SelectedReport.Name == "Tickets")
            {
                suggFileName = "Ticket Report";
                ExportListStr.Add("Id,Type,Description,Created,Closed,Status,CustomerId,Registration,Period,Rate,Cost,PayAmount,Balance,User,Updated,UpdatedBy");
            }

            else if (SelectedReport.Name == "Receipts")
            {
                suggFileName = "Receipt Report";
                ExportListStr.Add("Id,TicketNumber,TicketType,Created,Status,Cost,Paid,Balance,PaymentMethod,ChequeNumber,User");
            }

            FileSavePicker picker = new();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeChoices.Add("Csv", new List<string>() { ".csv" });
            picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            picker.SuggestedFileName = suggFileName;
            picker.SettingsIdentifier = "settingsIdentifier";
            picker.DefaultFileExtension = ".csv";

            Window window = (Application.Current as App)?.Window;
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile saveFile = await picker.PickSaveFileAsync();
            if (saveFile != null)
            {

                // Save file was picked, you can now write in it
                if (SelectedReport.Name == "Tickets")
                    foreach (Ticket ticket in TicketList)
                        ExportListStr.Add(ticket.ToCsv());

                if (SelectedReport.Name == "Receipts")
                    foreach (Receipt receipt in ReceiptList)
                        ExportListStr.Add(receipt.ToCsv());

                await FileIO.WriteLinesAsync(saveFile, ExportListStr);
            }
            else
            {
                // No file was picked or the dialog was cancelled.
            }
        }

        [RelayCommand]
        public void SelectedReport_Changed()
        {
            if (SelectedReport.Name == "Receipts")
            {
                SelectedStatus = StatusList[0];
                StatusIsEnabled = false;
            }

            else if (SelectedReport.Name == "Tickets")
            {
                StatusIsEnabled = true;
            }
        }
    }
}
