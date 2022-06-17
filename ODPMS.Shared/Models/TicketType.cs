using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ODPMS.Models
{
    [Table("ticketType")]
    public class TicketType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Quantity {  get; set; }
        public double Rate { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
        public DateTime ActivityDate { get; set; }
        public bool IsDeletable { get; set; }
        
        [Ignore]
        public static string StatusMessage { get; set; }

        public TicketType(int id, string type, string description, int quantity, double rate, string status, string user, DateTime activityDate)
        {
            Id = id;
            Type = type;
            Description = description;
            Quantity = quantity;
            Rate = rate;
            Status = status;
            User = user;
            ActivityDate = activityDate;
        }

        public TicketType() { }

        public DateTime GetEndDate()
        {
            DateTime endDate = DateTime.Now;
            if (Type == "Hourly")
                endDate = endDate.AddDays(1).Subtract(new TimeSpan(0, 0, 0, 0, 1));

            if (Type == "Daily")
                endDate = endDate.AddDays(Quantity);

            else if (Type == "Weekly")
                endDate = endDate.AddDays(Quantity * 7);

            else if (Type == "Monthly")
                endDate = endDate.AddDays(Quantity * 30);

            return endDate;
        }

        #region Database Functions
        public static async Task<List<TicketType>> GetAllTicketTypes()
        {
            try
            {
                //await Init();
                //return await conn.Table<Ticket>().ToListAsync();
                var query = App.Database.Current.Table<TicketType>();
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", query);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<TicketType>();
        }

        public static async Task<TicketType> GetTicketType(int id)
        {
            try
            {
                //await Init();
                //return await conn.Table<Ticket>().ToListAsync();
                var query = await App.Database.Current.GetAsync<TicketType>(id);
                StatusMessage = string.Format("{0} record(s) found in the ticket table)", query);

                return query;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new TicketType();
        }

        public static async Task CreateTicketType(TicketType ticketType)
        {
            int result = 0;
            try
            {
                //await App.Database.Init();
                result = await App.Database.Current.InsertAsync(ticketType);

                StatusMessage = string.Format("{0} record(s) added [Ticket: {1})", result, ticketType.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", ticketType.Id, ex.Message);
            }
        }

        public static async Task DeleteTicketType(int id)
        {
            int result = 0;
            try
            {
                result = await App.Database.Current.DeleteAsync<TicketType>(id);

                StatusMessage = string.Format("{0} record(s) deleted [Ticket: {1})", result, id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", id, ex.Message);
            }
        }
        #endregion
    }

    public class TicketTypeViewModel
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
        public DateTime ActivityDate { get; set; }

        public TicketTypeViewModel(int? id, string type, string description, int quantity, double rate, string status, string user, DateTime activityDate)
        {
            Id = id;
            Type = type;
            Description = description;
            Quantity = quantity;
            Rate = rate;
            Status = status;
            User = user;
            ActivityDate = activityDate;
        }

        public DateTime GetEndDate()
        {
            DateTime endDate = DateTime.Now;
            if (Type == "Hourly")
                endDate = endDate.AddDays(1).Subtract(new TimeSpan(0, 0, 0, 0, 1));

            if (Type == "Daily")
                endDate = endDate.AddDays(Quantity);

            else if (Type == "Weekly")
                endDate = endDate.AddDays(Quantity * 7);

            else if (Type == "Monthly")
                endDate = endDate.AddDays(Quantity * 30);

            return endDate;
        }
    }
}
