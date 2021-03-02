using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    /// <summary>
    /// Interface for Configuration Manager
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the Outlook Middleware shared mailbox.
        /// </summary>
        /// <returns>The Outlook Middleware shared mailbox, if available. <see cref="System.String.Empty"/> otherwise.</returns>
        Task<GetSharedMailBoxResponse> GetSharedMailBox();

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

    }
}
