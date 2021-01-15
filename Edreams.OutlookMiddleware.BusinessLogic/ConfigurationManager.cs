using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Exchange;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Common.KeyVault;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ConfigurationManager : IConfigurationManager
    {
        #region <| Dependencies |>

        private readonly IExchangeClientFactory _exchangeClientFactory;
        private readonly IKeyVaultClientFactory _keyVaultClientFactory;
        private readonly IEdreamsConfiguration _configuration;

        #endregion

        /// <summary>Initializes a new instance of the <see cref="ConfigurationManager" /> class.</summary>
        /// <param name="exchangeClientFactory">The exchange client factory.</param>
        /// <param name="keyVaultClientFactory">The key vault client factory.</param>
        /// <param name="configuration">The configuration.</param>
        public ConfigurationManager(
            IExchangeClientFactory exchangeClientFactory,
            IKeyVaultClientFactory keyVaultClientFactory,
            IEdreamsConfiguration configuration)
        {
            _exchangeClientFactory = exchangeClientFactory;
            _keyVaultClientFactory = keyVaultClientFactory;
            _configuration = configuration;
        }

        public async Task<GetSharedMailBoxResponse> GetSharedMailBox()
        {
            KeyVaultClientOptions keyVaultClientOptions = new KeyVaultClientOptions
            {
                VaultUri = _configuration.KeyVaultUri,
                TenantId = _configuration.KeyVaultTenantId,
                ClientId = _configuration.KeyVaultClientId,
                ClientSecret = _configuration.KeyVaultClientSecret
            };

            IKeyVaultClient keyVaultClient = _keyVaultClientFactory.AuthenticateAndCreateClient(keyVaultClientOptions);

            ExchangeClientOptions exchangeClientOptions = new ExchangeClientOptions
            {
                Authority = _configuration.ExchangeAuthority,
                Resource = _configuration.ExchangeResourceId,
                ClientId = _configuration.ExchangeClientId,
                WebUri = _configuration.ExchangeOnlineServer,
                UserName = await keyVaultClient.GetSecret(_configuration.SharedMailBoxUserNameSecret),
                Password = await keyVaultClient.GetSecret(_configuration.SharedMailBoxPasswordSecret)
            };

            IExchangeClient exchangeClient = await _exchangeClientFactory.AuthenticateAndCreateClient(exchangeClientOptions);
            string emailAddress = await exchangeClient.ResolveEmailAddress();

            return new GetSharedMailBoxResponse
            {
                EmailAddress = emailAddress,
                CorrelationId = Guid.NewGuid()
            };
        }
    }
}