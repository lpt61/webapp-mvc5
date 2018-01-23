using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GreenSol.Domain.Entities
{
    public class Cart
    {
        /// <summary>
        /// //////////////////
        /// </summary>
        [Key]
        public int LineId { get; set; }
        public string CartId { get; set; }
        public int AlbumId { get; set; }
        public int Count { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        public virtual Album Album { get; set; }
        ///       
    }
}
