using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ODPMS.Models
{
    [Table("floats")]
    public class CashFloat
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public float Amount { get; set; }
        public string User { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        [Ignore]
        public static string StatusMessage { get; set; }

        #region Database Functions
        public static async Task<List<CashFloat>> GetAllCashFloats()
        {
            try
            {
                await App.Database.Init();
                var query = App.Database.Current.Table<CashFloat>();
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", await query.CountAsync());

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new List<CashFloat>();
        }

        public static async Task<CashFloat> GetCashFloat(int id)
        {
            try
            {
                await App.Database.Init();
                var query = await App.Database.Current.GetAsync<CashFloat>(id);
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", query);

                return query;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
            return new CashFloat();
        }

        public static async Task UpdateCashFloat(CashFloat cashFloat)
        {
            int result = 0;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.UpdateAsync(cashFloat);
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }
        }

        public static async Task CreateCashFloat(CashFloat cashFloat)
        {
            int result = 0;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.InsertAsync(cashFloat);

                StatusMessage = string.Format("{0} record(s) added [Ticket: {1})", result, cashFloat.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", cashFloat.Id, ex.Message);
            }
        }

        public static async Task DeleteCashFloat(int id)
        {
            int result = 0;
            try
            {
                await App.Database.Init();
                result = await App.Database.Current.DeleteAsync<CashFloat>(id);

                StatusMessage = string.Format("{0} record(s) deleted [Ticket: {1})", result, id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", id, ex.Message);
            }
        }
        #endregion
    }
}
