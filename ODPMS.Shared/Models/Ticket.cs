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
        public int Id { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Closed { get; set; }
        public string Status { get; set; }
        public float Rate { get; set; }
        public float Cost { get; set; }
        public float Balance { get; set; }
        public string User { get; set; }

        public Ticket(int id, int number, string type, string description, DateTime created, DateTime closed, string status, float rate, float cost, float balance, string user)
        {
            Id = id;
            Number = number;
            Type = type;
            Description = description;
            Created = created;
            Closed = closed;
            Status = status;
            Rate = rate;
            Cost = cost;
            Balance = balance;
            User = user;
        }

        public static async Task<ObservableCollection<Ticket>> GetTicketsAsync()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Tickets.txt"));
            IList<string> lines = await FileIO.ReadLinesAsync(file);

            ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();

            for (int i = 0; i < lines.Count; i += 10)
            {
                try
                {
                    tickets.Add(new Ticket(Int32.Parse(lines[i]), Int32.Parse(lines[i + 1]), lines[i + 2], lines[i + 3], DateTime.Parse(lines[i + 4]),
                    DateTime.Parse(lines[i + 5]), lines[i + 6], float.Parse(lines[i + 7]), float.Parse(lines[i + 8]), float.Parse(lines[i + 9]), lines[i + 10]));
                } 
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                
            }

            return tickets;
        }

        public override string ToString()
        {
            return Type;
        }
    }
}
