using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GreenSol.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //default
            routes.MapRoute(
                null,
                "",
                 new
                 {
                     controller = "Home",
                     action = "Index",
                 }
            );

            routes.MapRoute(
                null,
                "Album/List",
                 new
                 {
                     controller = "Album",
                     action = "List",
                     genre = (string)null,
                     page = 1,
                 }
            );
            
            
            #region Album
            #region Album/List , pageSize appeared as a query string, Request.QueryString["pageSize"]
            // localhost:8212/Album/List
            routes.MapRoute(
                null,
                "Album/List",
                 new{
                     controller = "Album",
                     action = "List",
                     genre = (string)null,
                     page = 1,
                 }
            );

            // localhost:8212/Album/List/Page3
            routes.MapRoute(
                null,
                "Album/List/Page{page}",
                new { 
                    controller = "Album", 
                    action = "List", 
                    genre = (string)null,
                    
                },
                new { 
                    page = @"\d+",
                }
            );

            // localhost:8212/Album/List/Pop
            routes.MapRoute(
                null,
                "Album/List/{genre}",
                new
                {
                    controller = "Album",
                    action = "List",
                    page = 1,
                }
            );

            // localhost:8212/Album/List/Pop/Page2
            routes.MapRoute(
                null,
                "Album/List/{genre}/Page{page}",
                new
                {
                    controller = "Album",
                    action = "List"
                },
                new
                {
                    page = @"\d+",
                }
            );

            routes.MapRoute(null, "{controller}/{action}");
            #endregion Album/List
            #endregion Album

            #region pageSize appeared as RouteData.Values["pageSize"]
            // localhost:8212/pageSize=9
            //routes.MapRoute(
            //    null,
            //    "pageSize={pageSize}",
            //    new
            //    {
            //        controller = "Album",
            //        action = "List",
            //        genre = (string)null,
            //        page = 1
            //    },
            //    new
            //    {
            //        pageSize = @"\d+",
            //    }
            //);

            // localhost:8212/Page2/pageSize=9
            //routes.MapRoute(
            //    null,
            //    "Page{page}/pageSize={pageSize}",
            //    new
            //    {
            //        controller = "Album",
            //        action = "List",
            //        genre = (string)null,
            //    },
            //    new
            //    {
            //        page = @"\d+",
            //        pageSize = @"\d+",
            //    }
            //);

            // localhost:8212/Pop/pageSize=9
            //routes.MapRoute(
            //    null,
            //    "{genre}/pageSize={pageSize}",
            //    new
            //    {
            //        controller = "Album",
            //        action = "List",
            //        page = 1,
            //    },
            //    new
            //    {
            //        pageSize = @"\d+",
            //    }
            //);

            // localhost:8212/Pop/Page2/pageSize=9
            //routes.MapRoute(
            //    null,
            //    "{genre}/Page{page}/pageSize={pageSize}",
            //    new
            //    {
            //        controller = "Album",
            //        action = "List"
            //    },
            //    new
            //    {
            //        page = @"\d+",
            //        pageSize = @"\d+",
            //    }
            //);
            #endregion


        }
    }
}
