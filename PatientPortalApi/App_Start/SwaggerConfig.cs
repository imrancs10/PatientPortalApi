using System.Web.Http;
using WebActivatorEx;
using PatientPortalApi;
using Swashbuckle.Application;
using System.IO;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace PatientPortalApi
{
    public class SwaggerConfig
    {
        protected static string GetXmlCommentsPath()
        {
            return Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "bin", "PatientPortalApi.xml");
        }
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "PatientPortalApi");

                        c.IncludeXmlComments(GetXmlCommentsPath());

                        c.ApiKey("Token")
                            .Description("Filling bearer token here")
                            .Name("Authorization")
                            .In("header");
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.EnableApiKeySupport("Authorization", "header");
                    });
        }
    }
}
