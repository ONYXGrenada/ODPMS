using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static SQLite.SQLite3;

namespace ODPMS.Models
{
    [Table("tickets")]
    public class Ticket
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }        
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Closed { get; set; }
        public string Status { get; set; }
        public int? CustomerId { get; set; }
        public string? Registration { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public double Cost { get; set; }
        public double PayAmount { get; set; }
        public double Balance { get; set; }
        public string User { get; set; }

        [Ignore]
        public static string StatusMessage { get; set; }

        public Ticket(int? id, string type, string description, DateTime created, DateTime? closed, string status, int customerId, string registration, int quantity, double rate, double cost, double payAmount, double balance, string user)
        {
            Id = id;            
            Type = type;
            Description = description;
            Created = created;
            Closed = closed;
            Status = status;
            CustomerId = customerId;
            Registration = registration;
            Quantity = quantity;
            Rate = rate;
            Cost = cost;
            PayAmount = payAmount;
            Balance = balance;
            User = user;

            if (type == "Hourly")
                UpdateCost();
            else
                UpdateClosed();
        }

        public Ticket() { }

        public override string ToString()
        {
            return Type;
        }

        public string ToCsv()
        {
            return Id.ToString() + "," + Type + "," + Description + "," + Created + "," + Closed.ToString() + "," + Status +
                "," + CustomerId.ToString() + "," + Registration + Quantity.ToString() + "," + "," + Rate.ToString() + "," + Cost.ToString() + "," + PayAmount.ToString() + "," + 
                Balance.ToString() + "," + User;
        }

        public void UpdateCost()
        {
            int gracePeriod = 5;
            Closed = DateTime.Now;

            if (Type == "Hourly")
            {
                TimeSpan ts = (DateTime)Closed - Created;

                if (ts.TotalMinutes % 60 >= gracePeriod)
                    Cost = Rate * Math.Ceiling(ts.TotalHours);
                else
                    Cost = Rate * Math.Floor(ts.TotalHours);
            }
            //return Cost;
        }

        public void PayTicket(double payAmount)
        {
            int gracePeriod = 5;
            Status = "Paid";
            PayAmount += payAmount;

            if (Type == "Hourly")
            {
                TimeSpan ts = (DateTime)Closed - Created;

                if (ts.TotalMinutes % 60 >= gracePeriod)
                    Cost = Rate * Math.Ceiling(ts.TotalHours);
                else
                    Cost = Rate * Math.Floor(ts.TotalHours);
            }
            Balance = Cost - PayAmount;
        }

        public void UpdateClosed()
        {
            //DateTime endDate = DateTime.Now;
            //if (Type == "Hourly")
            //    return
                //endDate = endDate.AddDays(1).Subtract(new TimeSpan(0, 0, 0, 0, 1));

            if (Type == "Daily")
                Closed = Created.AddDays(Quantity);

            else if (Type == "Weekly")
                Closed = Created.AddDays(Quantity * 7);

            else if (Type == "Monthly")
                Closed = Created.AddDays(Quantity * 30);

            //return endDate;
        }

        #region Database Functions
        public static async Task<List<Ticket>> GetAllTickets()
        {
            try
            {
                //await Init();
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
                //await Init();
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
                //await Init();
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
                //await Init();
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
            try
            {
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
                //await App.Database.Init();
                result = await App.Database.Current.InsertAsync(ticket);

                StatusMessage = string.Format("{0} record(s) added [Ticket: {1})", result, ticket.Id);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", ticket.Id, ex.Message);
            }
        }

        #endregion
    }



    public class TicketViewModel
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Closed { get; set; }
        public string Status { get; set; }
        public string Registration { get; set; }
        public double Cost { get; set; }
        public double Balance { get; set; }
        public string User { get; set; }

        public TicketViewModel(int? id, string type, string description, DateTime created, DateTime? closed, string status, string registration, double cost, double balance, string user)
        {
            Id = id;
            Type = type;
            Description = description;
            Created = created;
            Closed = closed;
            Status = status;
            Registration = registration;
            Cost = cost;
            Balance = balance;
            User = user;
        }
    }
}
