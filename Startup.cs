using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Examination_System.Startup))]
namespace Examination_System
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
