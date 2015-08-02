using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FeatureToggles.Dashboard.Startup))]

namespace FeatureToggles.Dashboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
