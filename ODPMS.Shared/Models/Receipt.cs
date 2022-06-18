using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public float Cost { get; set; }
        public float Paid { get; set; }
        public float Balance { get; set; }
        public string PaymentMethod { get; set; }
        public string ChequeNumber { get; set; }
        public string User { get; set; }

        [Ignore]
        public static string StatusMessage { get; set; }

        #region Database Functions
        public static async Task<List<Receipt>> GetAllReceipts()
        {
            try
            {
                //await Init();
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
                //await Init();
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
                //await Init();
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
                //await App.Database.Init();
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
                result = await App.Database.Current.DeleteAsync<Receipt>(id);

                StatusMessage = string.Format("{0} record(s) deleted [Receipt: {1})", result, id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", id, ex.Message);
            }
        }
        #endregion
    }
}
