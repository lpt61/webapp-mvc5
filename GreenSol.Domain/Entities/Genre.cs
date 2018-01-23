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
    public class Genre : IEntity
    {
        [HiddenInput(DisplayValue = false)]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Please specify a genre")]
        [Display(Name = "Genre")]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Album> Albums { get; set; }
    }
}
