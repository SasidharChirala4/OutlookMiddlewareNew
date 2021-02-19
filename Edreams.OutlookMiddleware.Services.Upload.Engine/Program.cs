using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
using Edreams.Common.AzureServiceBus._DependencyInjection;
using Edreams.OutlookMiddleware.Services.Upload.Engine.Interfaces;
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

                    services.AddTransient<IUploadEngineProcessor, UploadEngineProcessor>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<UploadEngineWorker>();
                });
    }
}