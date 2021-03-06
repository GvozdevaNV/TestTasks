﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task2.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }        
        [Required]
        public int Count { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal Amount { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}