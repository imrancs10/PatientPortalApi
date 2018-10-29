using System.Web.Http;
using WebActivatorEx;
using PatientPortalApi;
using Swashbuckle.Application;
using PatientPortal.Infrastructure.Utility;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace PatientPortalApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                     .EnableSwagger(c =>
                     {
                         c.SingleApiVersion("v1", "PatientPortal");
                         c.OperationFilter(() => new FromHeaderAttributeOperationFilter());
                         c.ApiKey("Token")
                        .Description("Filling bearer token here")
                        .Name("Authorization")
                        .In("header");
                     })
                     .EnableSwaggerUi(c =>
                     {
                         c.EnableApiKeySupport("Authorization", "header");
                         c.InjectJavaScript(thisAssembly,
                            "PatientPortalApi.Swagger.SwaggerUiCustomization.js");
                     });
        }
    }
}
