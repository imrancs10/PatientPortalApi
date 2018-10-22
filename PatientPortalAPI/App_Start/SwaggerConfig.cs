using System.Web.Http;
using WebActivatorEx;
using PatientPortalAPI;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace PatientPortalAPI
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "PatientPortalAPI");
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
        }
    }
}
