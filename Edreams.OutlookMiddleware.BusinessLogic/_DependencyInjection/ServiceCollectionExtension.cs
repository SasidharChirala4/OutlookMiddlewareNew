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

            services.AddTransient<IStatusManager, StatusManager>();
            services.AddTransient<IBatchManager, BatchManager>();
            services.AddTransient<IEmailManager, EmailManager>();
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<ILoggingManager, LoggingManager>();
            services.AddTransient<IConfigurationManager, ConfigurationManager>();


            services.AddTransient<ICleanupManager, CleanupManager>();
        }
    }
}