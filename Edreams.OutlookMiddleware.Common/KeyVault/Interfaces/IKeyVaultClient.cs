using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.KeyVault.Interfaces
{
    public interface IKeyVaultClient
    {
        Task<string> GetSecret(string key);
    }
}