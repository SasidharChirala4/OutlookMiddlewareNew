namespace Edreams.OutlookMiddleware.Common.KeyVault
{
    public class KeyVaultClientOptions
    {
        public string VaultUri { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}