using GreenSol.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GreenSol.Domain.Entities
{
    public class Album : IEntity
    {
        [HiddenInput(DisplayValue = false)]
        public int AlbumId { get; set; }

        [DisplayName("Genre")]
        public int GenreId { get; set; }

        [DisplayName("Artist")]
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "Please enter a title")]
        [Display(Name = "Album title")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a date")]
        //[DisplayFormat(DataFormatString = ("{0:dd/MM/yyyy}"))]
        [Display(Name = "Date release")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please enter a price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public decimal Price { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public string TrackList { get; set; }

        public virtual Genre Genre { get; set; }

        public virtual Artist Artist { get; set; }

        public virtual List<Order> Order { get; set; }


    }
}
