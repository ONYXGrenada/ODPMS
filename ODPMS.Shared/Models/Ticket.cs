using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ODPMS.Models
{
    public class Ticket
    {
        public int? Id { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Closed { get; set; }
        public string Status { get; set; }
        public int? CustomerId { get; set; }
        public string? Registration { get; set; }
        public double Rate { get; set; }
        public double Cost { get; set; }
        public double PayAmount { get; set; }
        public double Balance { get; set; }
        public string User { get; set; }

        public Ticket(int? id, int number, string type, string description, DateTime created, DateTime? closed, string status, int customerId, string registration, double rate, double cost, double payAmount, double balance, string user)
        {
            Id = id;
            Number = number;
            Type = type;
            Description = description;
            Created = created;
            Closed = closed;
            Status = status;
            CustomerId = customerId;
            Registration = registration;
            Rate = rate;
            Cost = cost;
            PayAmount = payAmount;
            Balance = balance;
            User = user;
        }

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
            return Id.ToString() + "," + Number.ToString() + "," + Type + "," + Description + "," + Created + "," + Closed.ToString() + "," + Status +
                "," + CustomerId.ToString() + "," + Registration + "," + Rate.ToString() + "," + Cost.ToString() + "," + PayAmount.ToString() + "," + 
                Balance.ToString() + "," + User;
        }

        public double UpdateCost()
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

            return Cost;
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
    }

    public class TicketViewModel
    {
        public int? Id { get; set; }
        public int? Number { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Closed { get; set; }
        public string Status { get; set; }
        public int CustomerId { get; set; }
        public string Registration { get; set; }
        public double Rate { get; set; }
        public double Cost { get; set; }
        public double PayAmount { get; set; }
        public double Balance { get; set; }
        public string User { get; set; }

        public TicketViewModel(int? id, int? number, string type, string description, DateTime created, DateTime? closed, string status, int customerId, string registration, double rate, double cost, double payAmount, double balance, string user)
        {
            Id = id;
            Number = number;
            Type = type;
            Description = description;
            Created = created;
            Closed = closed;
            Status = status;
            CustomerId = customerId;
            Registration = registration;
            Rate = rate;
            Cost = cost;
            PayAmount = payAmount;
            Balance = balance;
            User = user;
        }
    }
}
