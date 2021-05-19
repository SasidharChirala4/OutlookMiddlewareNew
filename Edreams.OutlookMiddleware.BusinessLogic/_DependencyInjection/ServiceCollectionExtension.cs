using Edreams.Common.Exceptions.Factories;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.Common.Exchange;
using Edreams.Common.Exchange.Interfaces;
using Edreams.Common.KeyVault;
using Edreams.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Factories;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
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

            services.AddSingleton<ITransactionHelper, TransactionHelper>();

            services.AddTransient<IStatusManager, StatusManager>();
            services.AddTransient<IBatchManager, BatchManager>();
            services.AddTransient<IEmailManager, EmailManager>();
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<IPreloadedFileManager, PreloadedFileManager>();
            services.AddTransient<ILoggingManager, LoggingManager>();
            services.AddTransient<IConfigurationManager, ConfigurationManager>();
            services.AddTransient<ITransactionQueueManager, TransactionQueueManager>();
            services.AddTransient<ICleanupManager, CleanupManager>();
            services.AddTransient<IFileHelper, FileHelper>();
            services.AddTransient<ICategorizationManager, CategorizationManager>();
            services.AddTransient<IExchangeAndKeyVaultHelper, ExchangeAndKeyVaultHelper>();
            services.AddTransient<IBatchFactory, BatchFactory>();
        }
    }
}