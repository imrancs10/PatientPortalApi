using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Owin;

[assembly: OwinStartupAttribute(typeof(PatientPortalAPI.Startup))]
namespace PatientPortalAPI
{
    public partial class Startup
    {
        public void Configuration(AppBuilder app)
        {
        }
    }
}
