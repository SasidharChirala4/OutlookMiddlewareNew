using System.Security.Principal;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
using Edreams.OutlookMiddleware.Common.Security;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Services.Cleanup.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edreams.OutlookMiddleware.Services.Cleanup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostBuilder, services) =>
                {
                    ISecurityContext securityContext = new SecurityContext();
                    securityContext.RefreshCorrelationId();
                    securityContext.SetUserIdentity(WindowsIdentity.GetCurrent());

                    services.AddSingleton(_ => securityContext);
                    services.AddConfiguration(hostBuilder.Configuration);
                    services.AddCommon();
                    services.AddBusinessLogic();
                    services.AddHostedService<PreloadedFilesCleanupWorker>();
                    services.AddHostedService<PreloadedFilesExpirationWorker>();
                    services.AddHostedService<TransactionsCleanupWorker>();
                    services.AddHostedService<TransactionsExpirationWorker>();
                });
    }
}