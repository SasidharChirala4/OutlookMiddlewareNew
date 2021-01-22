using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
using Edreams.OutlookMiddleware.Common.ServiceBus._DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edreams.OutlookMiddleware.Services.Upload.Engine
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
                    services.AddConfiguration(hostBuilder.Configuration);
                    services.AddServiceBus();
                    services.AddBusinessLogic();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<UploadEngineWorker>();
                });
    }
}