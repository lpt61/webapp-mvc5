using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using GreenSol.UI.Models;

namespace GreenSol.UI.Infrastructure
{
    //I register this class in ConfigureAuth() method of Startup.Auth.cs 
    public class AppRoleManager : RoleManager<AppRole>, IDisposable
    {
        public AppRoleManager(RoleStore<AppRole> store) : base(store)
        {
        }

        //allow the OWIN start class to create instances for each request where Identity data is accessed, 
        //which means I don’t have to disseminate details of how role data is stored throughout the application.
        //I can just obtain and operate on instances of the AppRoleManager class
        public static AppRoleManager Create(
            IdentityFactoryOptions<AppRoleManager> options, 
            IOwinContext context)
        {
            return new AppRoleManager(new RoleStore<AppRole>(context.Get<AppIdentityDbContext>()));
        }
    }
}