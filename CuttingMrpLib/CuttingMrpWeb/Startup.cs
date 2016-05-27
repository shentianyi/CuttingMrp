using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CuttingMrpWeb.Startup))]
namespace CuttingMrpWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
