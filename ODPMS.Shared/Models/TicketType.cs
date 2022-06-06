using System;
using System.Collections.Generic;
using System.Text;

namespace ODPMS.Models
{
    class TicketType
    {
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

    }
}
