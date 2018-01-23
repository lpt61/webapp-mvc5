using GreenSol.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSol.Domain.Abstract
{
    public interface IOrderProcessor
    {
        void ProcessOrder(ShoppingCart cart, OrderDetail orderDetail);
    }
}
