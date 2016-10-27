using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChatterApp.Startup))]
namespace ChatterApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
