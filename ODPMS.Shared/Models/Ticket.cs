using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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

        //public static async Task<ObservableCollection<Ticket>> GetTicketsAsync()
        //{
        //    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Tickets.txt"));
        //    IList<string> lines = await FileIO.ReadLinesAsync(file);

        //    ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();

        //    for (int i = 0; i < lines.Count; i += 10)
        //    {
        //        try
        //        {
        //            tickets.Add(new Ticket(Int32.Parse(lines[i]), Int32.Parse(lines[i + 1]), lines[i + 2], lines[i + 3], DateTime.Parse(lines[i + 4]),
        //                DateTime.Parse(lines[i + 5]), lines[i + 6], double.Parse(lines[i + 7]), double.Parse(lines[i + 8]), double.Parse(lines[i + 9]), 
        //                double.Parse(lines[i + 10]), lines[i + 11]));
        //        } 
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.ToString());
        //        }
                
        //    }

        //    return tickets;
        //}

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
