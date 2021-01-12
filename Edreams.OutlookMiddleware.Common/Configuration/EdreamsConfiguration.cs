using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;

namespace Edreams.OutlookMiddleware.Common.Configuration
{
    public class EdreamsConfiguration : IEdreamsConfiguration
    {
        public string StoragePath { get; set; }
        public string EdreamsExtensibilityUrl { get; set; }
        public string EdreamsTokenKey { get; set; }
        public string EdreamsTokenValue { get; set; }
        public int MaxNumberPendingCategories { get; set; }
        public string ExchangeAuthority { get; set; }
        public string ExchangeClientId { get; set; }
        public string ExchangeOnlineServer { get; set; }
        public string ExchangeResourceId { get; set; }
        public string SharedMailBoxCredentials { get; set; }
    }
}