using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;

namespace ODPMS.Models
{
    [Table("receipts")]
    public class Receipt
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TicketNumber { get; set; }
        public string TicketType { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; }  
        public double Cost { get; set; }
        public double Paid { get; set; }
        public double Balance { get; set; }
        public string PaymentMethod { get; set; }
        public string ChequeNumber { get; set; }
        public string User { get; set; }

        [Ignore]
        public static string StatusMessage { get; set; }

        public string ToCsv()
        {
            return Id.ToString() + "," + TicketNumber.ToString() + "," + TicketType + "," + Created + "," + Status + "," + Cost.ToString() + "," + 
                Paid.ToString() + "," + Balance.ToString() + "," + PaymentMethod + "," + ChequeNumber + "," + User;
        }

        #region Database Functions
        public static async Task<List<Receipt>> GetAllReceipts()
        {
            try
            {
                await App.Database.Init();
                var query = App.Database.Current.Table<Receipt>();
                StatusMessage = string.Format("{0} record(s) found in the receipt table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<Receipt>();
        }

        public static async Task<List<Receipt>> GetReceiptsByStatus(string status)
        {
            try
            {
                await App.Database.Init();
                var query = App.Database.Current.Table<Receipt>().Where(v => v.Status.Equals(status));
                StatusMessage = string.Format("{0} record(s) found in the receipt table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<Receipt>();
        }

        public static async Task<List<Receipt>> GetReceiptsByReportFilter(DateTime fromDate, DateTime toDate, string status)
        {
            try
            {
                await App.Database.Init();
                AsyncTableQuery<Receipt> query;
                if (status == "All")
                    query = App.Database.Current.Table<Receipt>().Where(v => v.Created >= fromDate &&
                        v.Created <= toDate);
                else
                    query = App.Database.Current.Table<Receipt>().Where(v => v.Created >= fromDate &&
                        v.Created <= toDate &&
                        v.Status.Equals(status));

                StatusMessage = string.Format("{0} record(s) found in the receipt table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<Receipt>();
        }

        public static async Task<Receipt> GetReceipt(int id)
        {
            try
            {
                await App.Database.Init();
                var query = await App.Database.Current.GetAsync<Receipt>(id);
                StatusMessage = string.Format("{0} record(s) found in the receipt table)", query);

                return query;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new Receipt();
        }

        public static async Task UpdateReceipt(Receipt receipt)
        {
            int result = 0;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.UpdateAsync(receipt);
                StatusMessage = string.Format("{0} record(s) found in the receipt table)", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
        }

        public static async Task CreateReceipt(Receipt receipt)
        {
            int result = 0;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.InsertAsync(receipt);

                StatusMessage = string.Format("{0} record(s) added [Receipt: {1})", result, receipt.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", receipt.Id, ex.Message);
            }
        }

        public static async Task DeleteReceipt(int id)
        {
            int result = 0;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.DeleteAsync<Receipt>(id);

                StatusMessage = string.Format("{0} record(s) deleted [Receipt: {1})", result, id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", id, ex.Message);
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
                    e.PrintLine("B&H PHOTO & VIDEO"),
                    e.PrintLine("420 NINTH AVE."),
                    e.PrintLine("NEW YORK, NY 10001"),
                    e.PrintLine("(212) 502-6380 - (800)947-9975"),
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
                return StatusMessage = string.Format("Failed to print receipt to the printer. {0}", ex.Message);
            }
        }
        #endregion
    }
}
