namespace Edreams.OutlookMiddleware.Common.Configuration.Interfaces
{
    public interface IEdreamsConfiguration
    {
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
    }
}