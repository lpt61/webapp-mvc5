using GreenSol.Domain.Abstract;
using GreenSol.Domain.Entities;
using GreenSol.UI.Infrastructure.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using GreenSol.Domain.Concrete;

namespace GreenSol.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<EFDbContext>());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());
            ModelBinders.Binders.Add(typeof(AbstractSearch), new AbstractSearchModelBinder());

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Using a unique claim to identify a user
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == "AspNet.Identity.SecurityStamp").Value;
        }
    }
}
