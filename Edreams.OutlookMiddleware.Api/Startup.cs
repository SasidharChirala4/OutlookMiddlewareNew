using System.IO;
using System.Security.Principal;
using Edreams.Common.AzureServiceBus._DependencyInjection;
using Edreams.Common.Exceptions._DependencyInjection;
using Edreams.Common.Exchange._DependencyInjection;
using Edreams.Common.KeyVault._DependencyInjection;
using Edreams.Common.Logging._DependencyInjection;
using Edreams.Common.Security._DependencyInjection;
using Edreams.Common.Web.Middleware;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
            services.AddSecurityContextMiddleware();

            services.AddCommon();
            services.AddEdreamsSecurity(WindowsIdentity.GetCurrent());
            services.AddConfiguration(_configuration);
            services.AddEdreamsLogging();
            services.AddServiceBus();
            services.AddEdreamsExceptions();
            services.AddEdreamsKeyVaultIntegration();
            services.AddEdreamsExchangeIntegration();

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

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;
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

            app.UseSecurityContextMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}