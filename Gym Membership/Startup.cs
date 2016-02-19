using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gym_Membership.Startup))]
namespace Gym_Membership
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
