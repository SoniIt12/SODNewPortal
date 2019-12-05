using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SOD.Startup))]
namespace SOD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
