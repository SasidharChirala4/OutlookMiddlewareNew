using System;

namespace Edreams.OutlookMiddleware.Common.Configuration.Interfaces
{
    public interface IEdreamsConfiguration
    {
        string OutlookMiddlewareDbConnectionString { get; set; }
        string OutlookMiddlewarePreloadDbConnectionString { get; set; }
        string ServiceName { get; set; }
        string StoragePath { get; set; }
        string EdreamsExtensibilityUrl { get; set; }
        string EdreamsTokenKey { get; set; }
        string EdreamsTokenValue { get; set; }
        int MaxNumberPendingCategories { get; set; }
        string ExchangeAuthority { get; set; }
        string ExchangeClientId { get; set; }
        string ExchangeOnlineServer { get; set; }
        string ExchangeResourceId { get; set; }
        string SharedMailBoxUserNameSecret { get; set; }
        string SharedMailBoxPasswordSecret { get; set; }
        string KeyVaultUri { get; set; }
        string KeyVaultTenantId { get; set; }
        string KeyVaultClientId { get; set; }
        string KeyVaultClientSecret { get; set; }
        string ServiceBusConnectionString { get; set; }
        string ServiceBusQueueName { get; set; }
        int PreloadedFilesExpiryInMinutes { get; set; }
        int TransactionHistoryExpiryInMinutes { get; set; }
        int TransactionSchedulingIntervalInSeconds { get; set; }
        int ExpirationWorkerIntervalInSeconds { get; set; }
        TimeSpan PreloadedFilesWorkerScheduleStartTime { get; set; }
        TimeSpan PreloadedFilesWorkerScheduleStopTime { get; set; }
        int CleanupWorkerIntervalInSeconds { get; set; }
        TimeSpan TransactionsWorkerScheduleStartTime { get; set; }
        TimeSpan TransactionsWorkerScheduleStopTime { get; set; }
        TimeSpan CategorizationWorkerScheduleStartTime { get; set; }
        TimeSpan CategorizationWorkerScheduleStopTime { get; set; }
        int CategorizationExpiryInMinutes { get; set; }
        string SubjectResponse { get; set; }
        string EmailOutgoingSmtpAddress { get; set; }
        string EmailOutgoingFromAddress { get; set; }
        string ErrorMessage { get; set; }
        string EmailErrorSubject { get; set; }
    }
}