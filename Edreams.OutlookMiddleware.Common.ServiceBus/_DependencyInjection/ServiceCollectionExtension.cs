using Edreams.OutlookMiddleware.Common.ServiceBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Edreams.OutlookMiddleware.Common.ServiceBus._DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddServiceBus(this IServiceCollection services)
        {
            services.AddTransient<IServiceBusHandler, ServiceBusHandler>();
        }
    }
}