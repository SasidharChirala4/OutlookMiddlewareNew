using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.KeyVault.Interfaces
{
    public interface IKeyVaultClientFactory
    {
        IKeyVaultClient AuthenticateAndCreateClient(KeyVaultClientOptions clientOptions);
    }
}