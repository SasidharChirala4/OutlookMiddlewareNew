﻿using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;

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
    }
}