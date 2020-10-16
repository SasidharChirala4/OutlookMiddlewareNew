using System.Security.Principal;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
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
                .ConfigureServices((hostContext, services) =>
                {
                    ISecurityContext securityContext = new SecurityContext();
                    securityContext.RefreshCorrelationId();
                    securityContext.SetUserIdentity(WindowsIdentity.GetCurrent());

                    services.AddSingleton(_ => securityContext);
                    services.AddBusinessLogic();
                    services.AddHostedService<ExpirationWorker>();
                    services.AddHostedService<CleanupWorker>();
                });
    }
}