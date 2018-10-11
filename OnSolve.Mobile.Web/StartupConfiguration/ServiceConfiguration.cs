using AsyncPoco;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Web.Filters;
using OnSolve.Mobile.Web.Infrastructure.Adaptors;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services;
using OnSolve.Mobile.Web.Services.Interface;
using Serilog;
using SessionManager;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace OnSolve.Mobile.Web.StartupConfiguration
{
    static class ServiceConfiguration
    {
        const string TokenIssuer = "OnSolve.Mobile.Web";
        const string TokenSecretConfKey = "JwtToken:Secret";
        const string TokenValidityConfKey = "JwtToken:TokenValidity";
        const string ClockSkewConfKey = "JwtToken:ClockSkew";

        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration)
              .CreateLogger();
            services.AddSingleton(configuration);
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new XamarinApiMappingProfile());
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddJwtAuthentication(configuration);
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ModelStateActionFilter));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ENSUserOnly", policy => policy.RequireClaim("ENSUserId"));
            });
            services.RegisterDependencies(configuration);
            services.AddSwagger();
        }

        static void RegisterDependencies(this IServiceCollection services, IConfiguration Configuration)
        {
            // Register dependencies for application
            services.AddSingleton<ISMSService, SMSService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
            services.AddSingleton<IVerificationCodeGeneration, VerificationCodeGeneration>();
            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddSingleton<IMobileUserService, MobileUserService>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHttpNotificationClient, HttpNotificationClient>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IContactsService, ContactsService>();
            services.AddSingleton<SessionManagerSoap>(_ => new SessionManagerSoapClient(SessionManagerSoapClient.EndpointConfiguration.SessionManagerSoap));
            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IFCMTokenService, FCMTokenService>();

            var opt = new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseSqlServer(Configuration.GetConnectionString("OnSolveMobileDatabase"))
                .Options;
            services.AddSingleton<Func<OnSolveMobileContext>>
            (
                () => new OnSolveMobileContext(opt)
            );

            var dbConnectionString = Configuration.GetValue<string>("Swn402Database:ConnectionString");
            var dbProviderName = Configuration.GetValue<string>("Swn402Database:ProviderName");
            var dbTimeout = Configuration.GetValue<int>("Swn402Database:DbQueryTimeout");
            services.AddSingleton<IDatabase>(_ => new Database(dbConnectionString, dbProviderName) { CommandTimeout = dbTimeout });
        }

        static void AddSwagger(this IServiceCollection services)
        {
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "OnSolve Mobile API",
                    Version = "v1",
                    Description = "Api's created for OnSolve Mobile application. Not authorized currently."
                });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    In = "header",
                    Description = "Please insert ApiKey with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });

                var xmlFile = "XmlDocumentation.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>(TokenSecretConfKey)));
            var clockSkew = TimeSpan.FromSeconds(configuration.GetValue<int>(ClockSkewConfKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = signingKey,
                ClockSkew = clockSkew,
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                AuthenticationType = "JWT",
                ValidateAudience = false,
                ValidateIssuer = false
            };

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenValidationParameters;
            });
        }
    }
}
