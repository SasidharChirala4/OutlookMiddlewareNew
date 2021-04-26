using System.IO;
using System.Security.Principal;
using Edreams.Common.AzureServiceBus._DependencyInjection;
using Edreams.Common.Logging._DependencyInjection;
using Edreams.Common.Security._DependencyInjection;
using Edreams.OutlookMiddleware.Api.Middleware;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
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

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
            services.AddScoped<SecurityContextMiddleware>();

            services.AddCommon();
            services.AddEdreamsSecurity(WindowsIdentity.GetCurrent());
            services.AddConfiguration(_configuration);
            services.AddEdreamsLogging();
            services.AddServiceBus();

            services.AddBusinessLogic();
            services.AddControllers();

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