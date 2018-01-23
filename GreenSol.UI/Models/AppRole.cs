using Microsoft.AspNet.Identity.EntityFramework;

namespace GreenSol.UI.Models
{
    //I dont want to use the class IdentityRole directly, because it relies on EF to store role data
    //so I create a class derived from it
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string name) : base(name) { }
    }
}