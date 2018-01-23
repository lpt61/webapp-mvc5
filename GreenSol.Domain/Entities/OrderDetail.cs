using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GreenSol.Domain.Entities
{
    public class OrderDetail
    {
        [HiddenInput(DisplayValue = false)]
        public string CustomerId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public Status Status { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int OrderDetailId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public System.DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        public string Email { get; set; }

        [Display(Name="Phone number")]
        [Required(ErrorMessage = "Please enter your phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the address")]
        public string Adrress { get; set; }

        [Required(ErrorMessage = "Please enter a city name")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter a state name")]
        public string State { get; set; }

        public string Zip { get; set; }

        [Required(ErrorMessage = "Please enter a country name")]
        public string Country { get; set; }

        public bool GiftWrap { get; set; }

        [ScaffoldColumn(false)]
        [HiddenInput(DisplayValue = false)]
        public decimal Total { get; set; }

        [HiddenInput(DisplayValue = false)]
        public List<Order> Order { get; set; }
    }
}
