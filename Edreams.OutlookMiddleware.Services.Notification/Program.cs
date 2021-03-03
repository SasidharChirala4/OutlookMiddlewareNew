using System.Security.Principal;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
using Edreams.OutlookMiddleware.Common.Security;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edreams.OutlookMiddleware.Services.Notification
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

                    services.AddCommon();
                    services.AddConfiguration(hostBuilder.Configuration);
                    services.AddBusinessLogic();

                    services.AddHostedService<Worker>();
                });
    }
}