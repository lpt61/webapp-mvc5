using System;
using System.ComponentModel.DataAnnotations;

namespace GreenSol.UI.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }

        [Range(5,20)]
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
    }
}