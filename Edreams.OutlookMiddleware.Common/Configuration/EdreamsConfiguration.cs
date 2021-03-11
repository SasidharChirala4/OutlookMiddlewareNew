using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using System;

namespace Edreams.OutlookMiddleware.Common.Configuration
{
    public class EdreamsConfiguration : IEdreamsConfiguration
    {
        public string OutlookMiddlewareDbConnectionString { get; set; }
        public string OutlookMiddlewarePreloadDbConnectionString { get; set; }
        public string ServiceName { get; set; }
        public string StoragePath { get; set; }
        public string EdreamsExtensibilityUrl { get; set; }
        public string EdreamsTokenKey { get; set; }
        public string EdreamsTokenValue { get; set; }
        public int MaxNumberPendingCategories { get; set; }
        public string ExchangeAuthority { get; set; }
        public string ExchangeClientId { get; set; }
        public string ExchangeOnlineServer { get; set; }
        public string ExchangeResourceId { get; set; }
        public string SharedMailBoxUserNameSecret { get; set; }
        public string SharedMailBoxPasswordSecret { get; set; }
        public string KeyVaultUri { get; set; }
        public string KeyVaultTenantId { get; set; }
        public string KeyVaultClientId { get; set; }
        public string KeyVaultClientSecret { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string ServiceBusQueueName { get; set; }
        public int PreloadedFilesExpiryInMinutes { get; set; }
        public int TransactionHistoryExpiryInMinutes { get; set; }
        public int TransactionSchedulingIntervalInSeconds { get; set; }
        public int ExpirationWorkerIntervalInSeconds { get; set; }
        public TimeSpan PreloadedFilesWorkerScheduleStartTime { get; set; }
        public TimeSpan PreloadedFilesWorkerScheduleStopTime { get; set; }
        public int CleanupWorkerIntervalInSeconds { get; set; }
        public TimeSpan TransactionsWorkerScheduleStartTime { get; set; }
        public TimeSpan TransactionsWorkerScheduleStopTime { get; set; }
        public TimeSpan CategorizationWorkerScheduleStartTime { get; set; }
        public TimeSpan CategorizationWorkerScheduleStopTime { get; set; }
        public int CategorizationExpiryInMinutes { get; set; }
    }
}