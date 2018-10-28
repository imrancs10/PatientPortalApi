using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Owin;

[assembly: OwinStartupAttribute(typeof(PatientPortalApi.Startup))]
namespace PatientPortalApi
{
    public partial class Startup
    {
        public void Configuration(AppBuilder app)
        {
        }
    }
}
