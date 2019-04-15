using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }

        public ICollection<OrderItem> Items { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }
}