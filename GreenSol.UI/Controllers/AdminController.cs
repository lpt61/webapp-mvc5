using GreenSol.Domain.Abstract;
using GreenSol.Domain.Concrete;
using GreenSol.Domain.Entities;
using GreenSol.UI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GreenSol.UI.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        private IAlbumRepository repository;

        public AdminController(IAlbumRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index()
        {
            return View(repository.Albums);
        }

        // GET: /StoreManager/Edit/5
        public ViewResult Edit(int albumId)
        {
            ViewBag.GenresList = Url.Action("QuickSearchGenre", "Admin");
            ViewBag.ArtistsList = Url.Action("QuickSearchArtist", "Admin");

            Album album = repository.Albums.FirstOrDefault(p => p.AlbumId == albumId);           
            return View(album);
        }       
    
        // POST:
        [HttpPost]
        public ActionResult Edit(Album album, HttpPostedFileBase image = null)
        {
            ViewBag.GenresList = Url.Action("QuickSearchGenre", "Admin");
            ViewBag.ArtistsList = Url.Action("QuickSearchArtist", "Admin");

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    album.ImageMimeType = image.ContentType;
                    album.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(album.ImageData, 0, image.ContentLength);
                }
                repository.SaveAlbum(album);
                TempData["message"] = string.Format("{0} has been saved", album.Name);
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(album);
            }
        }

        #region Search
        public ActionResult QuickSearchArtist(string term)
        {
            var artists = GetTextAutoComplete(SearchCriteria.Artists, term, "Name").Select(a => new { value = a.Name });
            return Json(artists, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuickSearchGenre(string term)
        {
            var genres = GetTextAutoComplete(SearchCriteria.Genres, term, "Name").Select(a => new { value = a.Name });

            return Json(genres, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<IEntity> GetTextAutoComplete(SearchCriteria collection, string searchString, string property)
        {
            //Direct LINQ expression
            //return repository.Genres.Where(g => g.Name.ToLower().Contains(searchString.ToLower())).ToList();         
            
            //Manually build up LINQ expression
            IQueryable<IEntity> queryable = null;

            if (collection == SearchCriteria.Genres)
                queryable = repository.Genres.AsQueryable();
            else if (collection == SearchCriteria.Artists)
                queryable = repository.Artists.AsQueryable();

            List<TextSearch> searchCriteria = new List<TextSearch>()
            {
                new TextSearch()
                {
                    SearchTerm = searchString,
                    //Comparator = TextComparators.Contains,
                    Property = property,
                }
            };
            var data = queryable.ApplySearchCriterias(searchCriteria);

            return data;
        }
        #endregion

        public ViewResult Create()
        {
            ViewBag.GenresList = Url.Action("QuickSearchGenre", "Admin");
            ViewBag.ArtistsList = Url.Action("QuickSearchArtist", "Admin");

            return View("Edit", new Album());
        }

        [HttpPost]
        public ActionResult Delete(int albumId)
        {
            Album deletedAlbum = repository.DeleteAlbum(albumId);
            if (deletedAlbum != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedAlbum.Name);
            }
            return RedirectToAction("Index");
        }

        public ActionResult OrderList()
        {
            //var model = new OrderHistoryModel()
            //{
            //    Orders = repository.Orders.Where(o => o.CusId == cusId).OrderBy(o => o.OrderDetails.OrderDate)
            //};
            var model = new List<OrderHistoryModel>() { };
            var orderDetail = repository.OrderDetails.Where(od => od.Status == Status.Processing || od.Status == Status.Delivering).OrderBy(od => od.OrderDate);
            foreach (var od in orderDetail)
            {
                var orders = repository.Orders.Where(o => o.OrderDetailId == od.OrderDetailId);
                model.Add(new OrderHistoryModel()
                {
                    OrderDetailsId = od.OrderDetailId,
                    OrderDate = od.OrderDate,
                    Total = od.Total,
                    Status = od.Status,
                    Orders = orders
                });
            }

            return View(model);
        }

        public JsonResult ProcessOrder(int id)
        {
            string strStatus = repository.ProcessOrder(id, true);

            return Json(strStatus, JsonRequestBehavior.AllowGet);
        }
    }
}