using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GreenSol.UI.Startup))]
namespace GreenSol.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
