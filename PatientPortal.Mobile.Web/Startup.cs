using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PatientPortal.Mobile.Data;
using PatientPortal.Mobile.Web.StartupConfiguration;
using System;


namespace PatientPortal.Mobile.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            Func<PatientPortalMobileContext> dbprovider)
        {
            app.ConfigureApplication(env, loggerFactory);
            using (var dbContext = dbprovider())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
