using System;
using System.Collections.Generic;
using System.Text;

namespace ODPMS.Models
{
    [Table("receipts")]
    public class Receipt
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Number { get; set; }
    }
}
