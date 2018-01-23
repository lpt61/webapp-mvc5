using GreenSol.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenSol.UI.Models
{
    public class ShoppingCartViewModel
    {
        [Required(ErrorMessage="Your cart is empty")]
        public List<Cart> Lines { get; set; }
        public decimal TotalPrice { get; set; }
        public string ReturnUrl { get; set; }
    }
}