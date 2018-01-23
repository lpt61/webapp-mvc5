using GreenSol.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using GreenSol.Domain.Entities;
using GreenSol.Domain.Abstract;

using Newtonsoft.Json;
using System.Web.SessionState;

namespace GreenSol.UI.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class HomeController : Controller
    {
        private IAlbumRepository repository;
        private AppUser CurrentUser
        {
            get
            {
                //using Microsoft.AspNet.Identity;
                return UserManager.FindByName(HttpContext.User.Identity.Name);
            }
        }
        private AppUserManager UserManager
        {
            get
            {
                //using Microsoft.AspNet.Identity.Owin;
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        public HomeController(IAlbumRepository repo)
        {
            this.repository = repo;
        }

        public ActionResult UserDetail()
        {
            return View(GetData("Index"));
        }

        [Authorize(Roles = "Users")]
        public ActionResult OtherAction()
        {
            return View("Index", GetData("OtherAction"));
        }

        private Dictionary<string, object> GetData(string actionName)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("Action", actionName);
            dict.Add("User", HttpContext.User.Identity.Name);
            dict.Add("Authenticated", HttpContext.User.Identity.IsAuthenticated);
            dict.Add("Auth Type", HttpContext.User.Identity.AuthenticationType);
            dict.Add("In Users Role", HttpContext.User.IsInRole("Users"));
            return dict;
        }

        [Authorize]
        public ActionResult UserProps()
        {
            return View(CurrentUser);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UserProps(Cities city)
        {
            AppUser user = CurrentUser;
            user.City = city;
            user.SetCountryFromCity(city);
            await UserManager.UpdateAsync(user);
            return View(user);
        }        

        //////////////////////////////////////////////////////////////
        public ActionResult Index()
        {
            // Get most popular albums
            var albums = GetTopSellingAlbums(5);

            /*I want every user can only get 1 special deal per day,
             * To assure that, when the user clicks the "get daily deal" button, 
             * I store the daily deal AlbumId in the session
             * then set session time out to 1 day 1440 mins (do this in Web.config)
             */
            try
            {
                ViewBag.DailyDealId = Session["DailyDealId"].ToString();
            }
            catch (NullReferenceException)
            {
                //Set default value if the Session["DailyDealId"] does not have one;
                ViewBag.DailyDealId = "NULL";
            }

            return View(albums);
        }

        private List<Album> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            return repository.Albums
                .OrderByDescending(a => a.Order.Count())
                .Take(count)
                .ToList();
        }

        //public ActionResult DailyDeal()
        //{
        //    var album = GetDailyDeal();

        //    return PartialView("_DailyDeal", album);
        //}

        public ActionResult DailyDeal()
        {
            var result = JsonConvert.SerializeObject("Sorry, this is for registered members only.");

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var album = repository.Albums.Where(a => a.AlbumId == 7).First();

                album.Price *= 0.5m;
                result = JsonConvert.SerializeObject(
                        new Album() { AlbumId = album.AlbumId, Name = album.Name, Artist = album.Artist, Price = album.Price },
                        Formatting.Indented,
                        new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
                    );
                Session["DailyDealId"] = album.AlbumId.ToString();
            }
            return Content( result, "application/json");
        }

        // Select an album and discount it by 50%
        //private Album GetDailyDeal()
        //{
        //    var album = repository.Albums
        //        .OrderBy(a => System.Guid.NewGuid())
        //        .First();

        //    album.Price *= 0.5m;
        //    return album;
        //}
    }
}