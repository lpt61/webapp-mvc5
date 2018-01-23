using GreenSol.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GreenSol.UI.Controllers
{
    public class NavController : Controller
    {
        private IAlbumRepository repository;

        public NavController(IAlbumRepository repo)
        {
            repository = repo;
        }
 
        //public PartialViewResult Menu() {
        //IEnumerable<string> genres = repository.Albums
        //    .Select(x => x.Genre.Name)
        //    .Distinct()
        //    .OrderBy(x => x);

        //return PartialView(genres);
        //}

        public PartialViewResult Menu(string genre = null)
        {
            ViewBag.SelectedGenre = genre;
            IEnumerable<string> genres = repository.Albums
                .Select(x => x.Genre.Name)
                .Distinct()
                .OrderBy(x => x);

            return PartialView("FlexMenu", genres);
        }
    }
}