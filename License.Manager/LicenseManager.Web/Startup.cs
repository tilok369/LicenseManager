using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LicenseManager.Web.Startup))]
namespace LicenseManager.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
