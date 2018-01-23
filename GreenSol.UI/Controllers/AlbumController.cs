using GreenSol.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GreenSol.UI.Models;
using GreenSol.Domain.Entities;
using System.Web.Routing;

using System.Web.SessionState;

namespace GreenSol.UI.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class AlbumController : Controller
    {
        private IAlbumRepository repository;

        private int PageSize = 5;
        
        public AlbumController(IAlbumRepository albumRepository)
        {
            this.repository = albumRepository;
        }

        public ViewResult List(string genre, int page = 1)
        {
            int pageSize = PageSize;

            //if you use route route values
            //if (this.RouteData.Values["pageSize"] != null)
            //    Session["pageSize"] = this.RouteData.Values["pageSize"];

            //Use query string
            if (Request.QueryString["pageSize"] != null)
            {
                // pageSize value will last until the user set a new value for it or the user log out
                Session["pageSize"] = Request.QueryString["pageSize"];
            }

            try
            {
                var ps = int.Parse(Session["pageSize"].ToString());
                if (ps >= 5)
                    pageSize = ps;
            }
            catch {
                pageSize = PageSize;
            }

            var model = new AlbumsListViewModel
            {
                Albums = repository.Albums
                    .Where(a => (genre == null || a.Genre.Name == genre))
                    .OrderBy(p => p.AlbumId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = genre == null ? repository.Albums.Count() : repository.Albums.Where(a => a.Genre.Name == genre).Count()
                },
                CurrentGenre = genre
            };
            return View(model);
        }

        public ActionResult AlbumDetails(int albumId)
        {
            var album = repository.Albums.FirstOrDefault(a => a.AlbumId == albumId);

            return View(album);
        }

        public FileContentResult GetImage(int albumId)
        {
            Album album = repository.Albums.FirstOrDefault(p => p.AlbumId == albumId);
            if (album != null)
                if (album.ImageData != null)
                    return File(album.ImageData, album.ImageMimeType);
                else
                {
                    byte[] byteArr = System.IO.File.ReadAllBytes(
                        System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/placeholder2.png"));
                    return File(byteArr, "png"); ;
                }
            return null;
        }
    }
}