using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.DependencyInjection;
using Edreams.OutlookMiddleware.Mapping.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddBusinessLogic(this IServiceCollection services)
        {
            services.AddMapping();
            services.AddDataAccess();

            services.AddTransient<IBatchLogic, BatchLogic>();
            services.AddTransient<IEmailLogic, EmailLogic>();
            services.AddTransient<IFileLogic, FileLogic>();

            services.AddTransient<ICleanupLogic, CleanupLogic>();
        }
    }
}