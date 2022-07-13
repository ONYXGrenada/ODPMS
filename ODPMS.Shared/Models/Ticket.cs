using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using ESCPOS_NET;
using System.Net.Sockets;

namespace ODPMS.Models
{
    [Table("tickets")]
    public class Ticket
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }        
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Closed { get; set; }
        public string Status { get; set; }
        public int? CustomerId { get; set; }
        public string Registration { get; set; }
        public int Period { get; set; }
        public double Rate { get; set; }
        public double Cost { get; set; }
        public double PayAmount { get; set; }
        public double Balance { get; set; }
        public string User { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeletable { get; set; }

        [Ignore]
        public static string StatusMessage { get; set; }

        public override string ToString()
        {
            return Type;
        }

        public string ToCsv()
        {
            return Id + "," + Type + "," + Description + "," + Created + "," + Closed + "," + Status + "," + CustomerId + "," + 
                Registration + "," + Period + "," + Rate + "," + Cost + "," + PayAmount + "," + Balance + "," + User + "," + Updated + "," + UpdatedBy;
        }

        public void UpdateCost()
        {
            int gracePeriod = 5;

            if (Type == "Hourly" && Status == "Open")
            {
                Closed = DateTime.Now;
                TimeSpan ts = (DateTime)Closed - Created;

                if (ts.TotalMinutes % 60 >= gracePeriod)
                    Cost = Rate * Math.Ceiling(ts.TotalHours);
                else
                    Cost = Rate * Math.Floor(ts.TotalHours);
            }
        }

        public void UpdateDeletable()
        {
            TimeSpan ts = DateTime.Now - Created;
            if (ts.TotalMinutes <= 5)
                IsDeletable = true;
            else
                IsDeletable = false;
        }

        public void PayTicket(double payAmount)
        {
            if (payAmount == 0)
                return;

            int gracePeriod = 5;
            PayAmount += payAmount;

            if (Type == "Hourly")
            {
                TimeSpan ts = (DateTime)Closed - Created;

                if (ts.TotalMinutes % 60 >= gracePeriod)
                    Cost = Rate * Math.Ceiling(ts.TotalHours);
                else
                    Cost = Rate * Math.Floor(ts.TotalHours);
            }

            if (PayAmount > Cost)
                Balance = 0;
            else
                Balance = Cost - PayAmount;

            if (Balance == 0)
                Status = "Paid";
            else if (Balance > 0)
                Status = "Partial";
        }

        public void UpdateClosed()
        {
            if (Type == "Daily")
                Closed = Created.AddDays(Period);

            else if (Type == "Weekly")
                Closed = Created.AddDays(Period * 7);

            else if (Type == "Monthly")
                Closed = Created.AddDays(Period * 30);
        }

        #region Database Functions
        public static async Task<List<Ticket>> GetAllTickets()
        {
            try
            {
                await App.Database.Init();
                var query = App.Database.Current.Table<Ticket>();
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<Ticket>();
        }

        public static async Task<List<Ticket>> GetTicketsByType(string type)
        {
            try
            {
                await App.Database.Init();
                var query = App.Database.Current.Table<Ticket>().Where(v => v.Type.Equals(type));
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<Ticket>();
        }
        
        public static async Task<List<Ticket>> GetTicketsByStatus(string status)
        {
            try
            {
                await App.Database.Init();
                var query = App.Database.Current.Table<Ticket>().Where(v => v.Status.Equals(status));
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<Ticket>();
        }

        public static async Task<List<Ticket>> GetTicketsByReportFilter(DateTime fromDate, DateTime toDate, string status)
        {
            try
            {
                await App.Database.Init();
                AsyncTableQuery<Ticket> query;
                if (status == "All")
                    query = App.Database.Current.Table<Ticket>().Where(v => v.Created >= fromDate &&
                        v.Created <= toDate);
                else
                    query = App.Database.Current.Table<Ticket>().Where(v => v.Created >= fromDate &&
                        v.Created <= toDate &&
                        v.Status.Equals(status));

                StatusMessage = string.Format("{0} record(s) found in the ticket table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<Ticket>();
        }

        public static async Task<Ticket> GetTicket(int id)
        {
            try
            {
                await App.Database.Init();
                var query = await App.Database.Current.GetAsync<Ticket>(id);
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", query);

                return query;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new Ticket();
        }

        public static async Task UpdateTicket(Ticket ticket)
        {
            int result = 0;
            ticket.Updated = DateTime.Now;
            ticket.UpdatedBy = App.LoggedInUser.Username;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.UpdateAsync(ticket);
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
        }

        public static async Task CreateTicket(Ticket ticket)
        {
            int result = 0;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.InsertAsync(ticket);

                StatusMessage = string.Format("{0} record(s) added [Ticket: {1})", result, ticket.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", ticket.Id, ex.Message);
            }
        }

        public static async Task DeleteTicket(int id)
        {
            int result = 0;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.DeleteAsync<Ticket>(id);

                StatusMessage = string.Format("{0} record(s) deleted [Ticket: {1})", result, id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", id, ex.Message);
            }
        }

        public static async Task DeleteTicket(Ticket ticket)
        {
            int result = 0;
            ticket.Status = "Delete";
            ticket.Updated = DateTime.Now;
            ticket.UpdatedBy = App.LoggedInUser.Username;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.UpdateAsync(ticket);

                StatusMessage = string.Format("{0} record(s) found in the ticket table)", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
        }
        #endregion

        #region Print Functions
        public string ToPrint(Settings settings)
        {
            try
            {
                var printer = new SerialPrinter(portName: settings.ReceiptSettings.PrinterCOMPort, baudRate: 115200);
                var e = new EPSON();
                printer.Write(
                  ByteSplicer.Combine(
                    e.CenterAlign(),
                    e.PrintImage(File.ReadAllBytes("ms-appx:///Assets/Images/logo-placeholder.png"), true),
                    e.PrintLine("\n"),
                    e.SetBarcodeHeightInDots(360),
                    e.SetBarWidth(BarWidth.Default),
                    e.SetBarLabelPosition(BarLabelPrintPosition.None),
                    e.PrintBarcode(BarcodeType.ITF, Id.ToString()),
                    e.PrintLine("\n"),
                    e.PrintLine(settings.CompanySettings.CompanyName),
                    e.PrintLine(settings.CompanySettings.CompanyAddress),
                    e.PrintLine(settings.CompanySettings.CompanyEmail),
                    e.PrintLine(settings.CompanySettings.CompanyPhone),
                    e.SetStyles(PrintStyle.Underline),
                    e.PrintLine("www.bhphotovideo.com"),
                    e.SetStyles(PrintStyle.None),
                    e.PrintLine("\n"),
                    e.LeftAlign(),
                    e.PrintLine("Order: 123456789        Date: 02/01/19"),
                    e.PrintLine("\n"),
                    e.PrintLine("\n"),
                    e.SetStyles(PrintStyle.FontB),
                    e.PrintLine("1   TRITON LOW-NOISE IN-LINE MICROPHONE PREAMP"),
                    e.PrintLine("    TRFETHEAD/FETHEAD                        89.95         89.95"),
                    e.PrintLine("----------------------------------------------------------------"),
                    e.RightAlign(),
                    e.PrintLine("SUBTOTAL         89.95"),
                    e.PrintLine("Total Order:         89.95"),
                    e.PrintLine("Total Payment:         89.95"),
                    e.PrintLine("\n"),
                    e.LeftAlign(),
                    e.SetStyles(PrintStyle.Bold | PrintStyle.FontB),
                    e.PrintLine("SOLD TO:                        SHIP TO:"),
                    e.SetStyles(PrintStyle.FontB),
                    e.PrintLine("  FIRSTN LASTNAME                 FIRSTN LASTNAME"),
                    e.PrintLine("  123 FAKE ST.                    123 FAKE ST."),
                    e.PrintLine("  DECATUR, IL 12345               DECATUR, IL 12345"),
                    e.PrintLine("  (123)456-7890                   (123)456-7890"),
                    e.PrintLine("  CUST: 87654321"),
                    e.PrintLine("\n"),
                    e.PrintLine("\n")
                  )
                );
                return StatusMessage = "Success";
            }
            catch (Exception ex)
            {
                return StatusMessage = string.Format("Failed to print ticket to the printer. {0}", ex.Message);
            }
        }
        #endregion
    }
}
