using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Reflection;

namespace PatientPortal.Mobile.Web.StartupConfiguration
{
    static class AppConfiguration
    {
        public static void ConfigureApplication(this IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            app.UseStaticFiles();
            app.ConfigureSwagger();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //adding resources folder to be used for static files.
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources")),
            //    RequestPath = "/Resources"
            //});
        }

        public static void ConfigureSwagger(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PatientPortal Mobile API V1");
            });
        }        
    }
}
