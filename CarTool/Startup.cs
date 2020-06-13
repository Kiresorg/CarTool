using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CarTool.Startup))]
namespace CarTool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
