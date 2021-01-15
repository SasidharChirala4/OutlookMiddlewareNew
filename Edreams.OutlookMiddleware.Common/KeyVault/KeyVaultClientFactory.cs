using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;

namespace Edreams.OutlookMiddleware.Common.KeyVault
{
    public class KeyVaultClientFactory : IKeyVaultClientFactory
    {
        public IKeyVaultClient AuthenticateAndCreateClient(KeyVaultClientOptions clientOptions)
        {
            Uri vaultUri = new Uri(clientOptions.VaultUri);
            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(clientOptions.TenantId, clientOptions.ClientId, clientOptions.ClientSecret);

            SecretClient secretClient = new SecretClient(vaultUri, clientSecretCredential);

            return new KeyVaultClient(secretClient, clientOptions);
        }
    }
}