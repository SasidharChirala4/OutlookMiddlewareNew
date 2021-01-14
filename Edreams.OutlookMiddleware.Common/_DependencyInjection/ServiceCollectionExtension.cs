using Edreams.OutlookMiddleware.Common.Configuration;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Edreams.OutlookMiddleware.Common._DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEdreamsConfiguration>(_ => new EdreamsConfiguration
            {
                StoragePath = configuration.GetValue<string>("StoragePath"),
                EdreamsExtensibilityUrl = configuration.GetValue<string>("EdreamsExtensibilityUrl"),
                EdreamsTokenKey = configuration.GetValue<string>("EdreamsTokenKey"),
                EdreamsTokenValue = configuration.GetValue<string>("EdreamsTokenValue"),
                MaxNumberPendingCategories = configuration.GetValue<int>("MaxNumberPendingCategories"),
                ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString"),
                ServiceBusQueueName = configuration.GetValue<string>("ServiceBusQueueName")
            });
        }
    }
}