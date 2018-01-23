using GreenSol.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GreenSol.Domain.Entities
{
    public class Artist : IEntity
    {
        [HiddenInput(DisplayValue = false)]
        public int ArtistId { get; set; }

        [Display(Name="Artist")]
        [Required(ErrorMessage = "Please specify an artist")]
        public string Name { get; set; }
    }
}
