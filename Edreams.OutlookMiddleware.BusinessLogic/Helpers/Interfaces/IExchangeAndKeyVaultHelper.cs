using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;
using Microsoft.Exchange.WebServices.Data;
namespace Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces
{
    public interface IExchangeAndKeyVaultHelper
    {
        /// <summary>
        /// Prepare the Azure KeyVault client 
        /// </summary>
        /// <returns></returns>
        IKeyVaultClient CreateKeyVaultClient();

        /// <summary>
        /// Prepare the ExchangeClient
        /// </summary>
        /// <param name="keyVaultClient"></param>
        /// <returns>IExchangeClient</returns>
        Task<IExchangeClient> CreateExchangeClient(IKeyVaultClient keyVaultClient);

        /// <summary>
        /// Create exchange service, authenticate using data from Azure KeyVault 
        /// </summary>
        /// <param name="keyVaultClient"></param>
        /// <returns>ExchangeService</returns>
        Task<ExchangeService> CreateExchangeService(IKeyVaultClient keyVaultClient);
    }
}