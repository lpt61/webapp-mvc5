using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSol.Domain.Entities
{
    public class Order
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int AlbumId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Album Album { get; set; }
        public virtual OrderDetail OrderDetails { get; set; }
    }

    public enum Status{
        Processing,
        Delivering,
        Delivered,
        Canceled
    }
}
