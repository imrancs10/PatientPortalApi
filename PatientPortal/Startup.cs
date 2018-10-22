using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Owin;

[assembly: OwinStartupAttribute(typeof(PatientPortal.Startup))]
namespace PatientPortal
{
    public partial class Startup
    {
        public void Configuration(AppBuilder app)
        {
        }
    }
}
