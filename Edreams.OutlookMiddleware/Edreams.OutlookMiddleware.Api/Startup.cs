using System.IO;
using Edreams.OutlookMiddleware.Api.Middleware;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common.Configuration;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Security;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Edreams.OutlookMiddleware.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
            services.AddScoped<SecurityContextMiddleware>();
            services.AddScoped<ISecurityContext, SecurityContext>();

            services.AddSingleton<IEdreamsConfiguration>(_ => new EdreamsConfiguration
            {
                StoragePath = _configuration.GetValue<string>("StoragePath"),
                EdreamsExtensibilityUrl = _configuration.GetValue<string>("EdreamsExtensibilityUrl"),
                EdreamsTokenKey = _configuration.GetValue<string>("EdreamsTokenKey"),
                EdreamsTokenValue = _configuration.GetValue<string>("EdreamsTokenValue"),
            });

            services.AddControllers();
            services.AddBusinessLogic();

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "e-DReaMS Outlook Middleware",
                    Version = "v1"
                });

                var apiXml = Path.Combine(System.AppContext.BaseDirectory, "Edreams.OutlookMiddleware.Api.xml");
                c.IncludeXmlComments(apiXml);
                var dtoXml = Path.Combine(System.AppContext.BaseDirectory, "Edreams.OutlookMiddleware.DataTransferObjects.xml");
                c.IncludeXmlComments(dtoXml);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "e-DReaMS Outlook Middleware v1"));

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<SecurityContextMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}