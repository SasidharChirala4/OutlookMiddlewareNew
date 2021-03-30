using System.Security.Principal;
using Edreams.Common.AzureServiceBus._DependencyInjection;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
using Edreams.OutlookMiddleware.Common.Security;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Services.Categorization.Engine.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edreams.OutlookMiddleware.Services.Categorization.Engine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddEnvironmentVariables();
                })
                .ConfigureServices((hostBuilder, services) =>
                {
                    ISecurityContext securityContext = new SecurityContext();
                    securityContext.RefreshCorrelationId();
                    securityContext.SetUserIdentity(WindowsIdentity.GetCurrent());
                    services.AddSingleton(_ => securityContext);

                    services.AddCommon();
                    services.AddConfiguration(hostBuilder.Configuration);
                    services.AddServiceBus();
                    services.AddBusinessLogic();

                    services.AddTransient<ICategorizationEngineProcessor, CategorizationEngineProcessor>();
                    services.AddHostedService<CategorizationEngineWorker>();
                });
    }
}