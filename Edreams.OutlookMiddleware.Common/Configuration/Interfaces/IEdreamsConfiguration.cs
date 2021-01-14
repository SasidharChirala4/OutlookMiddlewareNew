namespace Edreams.OutlookMiddleware.Common.Configuration.Interfaces
{
    public interface IEdreamsConfiguration
    {
        string StoragePath { get; set; }
        string EdreamsExtensibilityUrl { get; set; }
        string EdreamsTokenKey { get; set; }
        string EdreamsTokenValue { get; set; }
        int MaxNumberPendingCategories { get; set; }
        string ServiceBusConnectionString { get; set; }
        string ServiceBusQueueName { get; set; }
    }
}