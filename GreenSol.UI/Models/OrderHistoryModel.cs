using GreenSol.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenSol.UI.Models
{
    public class OrderHistoryModel
    {
        public int OrderDetailsId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public Status Status { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}