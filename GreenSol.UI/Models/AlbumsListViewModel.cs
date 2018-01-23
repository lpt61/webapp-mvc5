using GreenSol.Domain.Entities;
using System.Collections.Generic;

namespace GreenSol.UI.Models
{
    //provide an instance of the PagingInfo view model class to the view
    //instead of using this class, I could do this using the view bag feature
    public class AlbumsListViewModel
    {
        public IEnumerable<Album> Albums { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentGenre { get; set; }
    }
}