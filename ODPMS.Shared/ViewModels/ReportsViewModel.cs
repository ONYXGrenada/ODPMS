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
            SelectedStatus = new ComboBoxItem() { Name = "All", Content = "All" };
        }

        [ICommand]
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
            var tickets = await Ticket.GetTicketsByReportFilter(fromDate, toDate, status);

            if (TicketList.Count != 0)
                TicketList.Clear();

            foreach (var ticket in tickets)
                TicketList.Add(ticket);

            if (TicketList.Count > 0)
                IsNotEmpty = true;
        }

        [ICommand]
        private async void ReportExport()
        {

            FileSavePicker picker = new();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeChoices.Add("Csv", new List<string>() { ".csv" });
            picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            picker.SuggestedFileName = "Ticket Report";
            picker.SettingsIdentifier = "settingsIdentifier";
            picker.DefaultFileExtension = ".csv";

            Window window = (Application.Current as App)?.Window;
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile saveFile = await picker.PickSaveFileAsync();
            if (saveFile != null)
            {
                List<string> TicketListStr = new List<string>();
                TicketListStr.Add("Id,Type,Description,Created,Closed,Status,CustomerId,Registration,Period,Rate,Cost,PayAmount,Balance,User");
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
        }
    }
}
