using GreenSol.UI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace GreenSol.UI.Infrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(): base("EFDbContext") {}

        //Constructor specifies a seed class
        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }

        //Creates instances of the class needed by the OWIN
        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    //Seed method
    //using Microsoft.AspNet.Identity;
    //I have to create instances of AppUserManager and AppRoleManager directly 
    //because the PerformInitialSetup method is called before the OWIN configuration is complete

    //After enabling Migration, The Configuration.cs in the Migration folder will initialize the db
    //So, I have to change the base class of IdentityDbInit from DropCreateDatabaseIfModelChanges<AppIdentityDbContext> to NullDatabaseInitializer<AppIdentityDbContext>
    //and also comment out all of its method to prevent this class from affecting the db

    //DropCreateDatabaseAlways
    //DropCreateDatabaseIfModelChanges
    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
    {
        //this method require an empty database (Delete the old and create a new empty db)
        protected override void Seed(AppIdentityDbContext context)
        {            
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public void PerformInitialSetup(AppIdentityDbContext context)
        {
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

            string roleName = "Administrators";
            string userName = "Admin";
            string password = "MySecret";
            string email = "admin@example.com";

            if (!roleMgr.RoleExists(roleName))
            {
                roleMgr.Create(new AppRole(roleName));
            }

            AppUser user = userMgr.FindByName(userName);
            if (user == null)
            {
                userMgr.Create(new AppUser { UserName = userName, Email = email }, password);
                user = userMgr.FindByName(userName);
            }

            if (!userMgr.IsInRole(user.Id, roleName))
            {
                userMgr.AddToRole(user.Id, roleName);
            }
        }
    }
}