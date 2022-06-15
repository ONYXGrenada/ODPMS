using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ODPMS.Models
{
    [Table("ticketType")]
    public class TicketType
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Quantity {  get; set; }
        public double UnitCost { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
        public DateTime ActivityDate { get; set; }

        public TicketType(int? id, string type, string description, int quantity, double unitCost, string status, string user, DateTime activityDate)
        {
            Id = id;
            Type = type;
            Description = description;
            Quantity = quantity;
            UnitCost = unitCost;
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
    }

    public class TicketTypeViewModel
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double UnitCost { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
        public DateTime ActivityDate { get; set; }

        public TicketTypeViewModel(int? id, string type, string description, int quantity, double unitCost, string status, string user, DateTime activityDate)
        {
            Id = id;
            Type = type;
            Description = description;
            Quantity = quantity;
            UnitCost = unitCost;
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
