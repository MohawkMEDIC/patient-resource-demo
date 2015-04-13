using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Health.Startup))]
namespace Health
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
