using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;

namespace Edreams.OutlookMiddleware.Common.KeyVault
{
    public class KeyVaultClient : IKeyVaultClient
    {
        private readonly SecretClient _secretClient;
        private readonly KeyVaultClientOptions _clientOptions;

        public KeyVaultClient(
            SecretClient secretClient,
            KeyVaultClientOptions clientOptions)
        {
            _secretClient = secretClient;
            _clientOptions = clientOptions;
        }

        public async Task<string> GetSecret(string key)
        {
            KeyVaultSecret secret = await _secretClient.GetSecretAsync(key);
            return secret.Value;
        }
    }
}