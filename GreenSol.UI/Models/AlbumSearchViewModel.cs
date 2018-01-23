using System.Collections.Generic;
using GreenSol.Domain.Abstract;
using GreenSol.Domain.Concrete;
using GreenSol.Domain.Entities;
using System.Web.Mvc;

namespace GreenSol.UI.Models
{
    public class AlbumSearchViewModel
    {
        //public IEnumerable<SomeClass> Data { get; set; }
        public IEnumerable<Album> Data { get; set; }

        public IEnumerable<AbstractSearch> SearchCriteria { get; set; }
    }
}