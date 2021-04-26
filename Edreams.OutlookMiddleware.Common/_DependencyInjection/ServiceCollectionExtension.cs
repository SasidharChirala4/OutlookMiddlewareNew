using System;
using Edreams.Common.Exceptions.Factories;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
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
                OutlookMiddlewareDbConnectionString = configuration.GetConnectionString("OutlookMiddlewareDbConnectionString"),
                OutlookMiddlewarePreloadDbConnectionString = configuration.GetConnectionString("OutlookMiddlewarePreloadDbConnectionString"),
                ServiceName = configuration.GetValue<string>("ServiceName"),
                StoragePath = configuration.GetValue<string>("StoragePath"),
                EdreamsExtensibilityUrl = configuration.GetValue<string>("EdreamsExtensibilityUrl"),
                EdreamsTokenKey = configuration.GetValue<string>("EdreamsTokenKey"),
                EdreamsTokenValue = configuration.GetValue<string>("EdreamsTokenValue"),
                MaxNumberPendingCategories = configuration.GetValue<int>("MaxNumberPendingCategories"),
                ExchangeAuthority = configuration.GetValue<string>("ExchangeAuthority"),
                ExchangeClientId = configuration.GetValue<string>("ExchangeClientId"),
                ExchangeOnlineServer = configuration.GetValue<string>("ExchangeOnlineServer"),
                ExchangeResourceId = configuration.GetValue<string>("ExchangeResourceId"),
                SharedMailBoxUserNameSecret = configuration.GetValue<string>("SharedMailBoxUserNameSecret"),
                SharedMailBoxPasswordSecret = configuration.GetValue<string>("SharedMailBoxPasswordSecret"),
                KeyVaultUri = configuration.GetValue<string>("KeyVaultUri"),
                KeyVaultTenantId = configuration.GetValue<string>("KeyVaultTenantId"),
                KeyVaultClientId = configuration.GetValue<string>("KeyVaultClientId"),
                KeyVaultClientSecret = configuration.GetValue<string>("KeyVaultClientSecret"),
                ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString"),
                ServiceBusQueueName = configuration.GetValue<string>("ServiceBusQueueName"),
                PreloadedFilesExpiryInMinutes = configuration.GetValue<int>("PreloadedFilesExpiryInMinutes"),
                TransactionHistoryExpiryInMinutes = configuration.GetValue<int>("TransactionHistoryExpiryInMinutes"),
                TransactionSchedulingIntervalInSeconds = configuration.GetValue<int>("TransactionSchedulingIntervalInSeconds"),
                ExpirationWorkerIntervalInSeconds = configuration.GetValue<int>("ExpirationWorkerIntervalInSeconds"),
                PreloadedFilesWorkerScheduleStartTime = configuration.GetValue<TimeSpan>("PreloadedFilesWorkerScheduleStartTime"),
                PreloadedFilesWorkerScheduleStopTime = configuration.GetValue<TimeSpan>("PreloadedFilesWorkerScheduleStopTime"),
                CleanupWorkerIntervalInSeconds = configuration.GetValue<int>("CleanupWorkerIntervalInSeconds"),
                TransactionsWorkerScheduleStartTime = configuration.GetValue<TimeSpan>("TransactionsWorkerScheduleStartTime"),
                TransactionsWorkerScheduleStopTime = configuration.GetValue<TimeSpan>("TransactionsWorkerScheduleStopTime"),
                CategorizationWorkerScheduleStartTime = configuration.GetValue<TimeSpan>("CategorizationWorkerScheduleStartTime"),
                CategorizationWorkerScheduleStopTime = configuration.GetValue<TimeSpan>("CategorizationWorkerScheduleStopTime"),
                CategorizationExpiryInMinutes = configuration.GetValue<int>("CategorizationExpiryInMinutes")
            });
        }

        public static void AddCommon(this IServiceCollection services)
        {
            services.AddTransient(typeof(IRestHelper<>), typeof(RestHelper<>));
            services.AddSingleton<ITimeHelper, TimeHelper>();
            services.AddSingleton<IFileHelper, FileHelper>();
            services.AddTransient(typeof(IRestHelper<>), typeof(RestHelper<>));
            services.AddSingleton<IValidator, Validator>();
            services.AddSingleton<IExceptionFactory, ExceptionFactory>();
        }
    }
}