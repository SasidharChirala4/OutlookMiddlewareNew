using System;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Edreams.Common.Exceptions.Constants;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.Common.Exchange;
using Edreams.Common.Exchange.Interfaces;
using Edreams.Common.KeyVault;
using Edreams.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;

namespace Edreams.OutlookMiddleware.BusinessLogic.Helpers
{
    public class ExchangeAndKeyVaultHelper : IExchangeAndKeyVaultHelper
    {
        #region <| Dependencies |>

        private readonly IExchangeClientFactory _exchangeClientFactory;
        private readonly IKeyVaultClientFactory _keyVaultClientFactory;
        private readonly IEdreamsConfiguration _configuration;
        private readonly IExceptionFactory _exceptionFactory;

        #endregion

        #region <| Construction |>

        public ExchangeAndKeyVaultHelper(
            IExchangeClientFactory exchangeClientFactory,
            IKeyVaultClientFactory keyVaultClientFactory,
            IEdreamsConfiguration configuration,
            IExceptionFactory exceptionFactory)
        {
            _exchangeClientFactory = exchangeClientFactory;
            _keyVaultClientFactory = keyVaultClientFactory;
            _configuration = configuration;
            _exceptionFactory = exceptionFactory;
        }

        #endregion

        #region <| IExchangeAndKeyVaultHelper Implementation |>

        public IKeyVaultClient CreateKeyVaultClient()
        {
            try
            {
                // Prepare the Azure KeyVault client options from the application configuration.
                KeyVaultClientOptions keyVaultClientOptions = new KeyVaultClientOptions
                {
                    VaultUri = _configuration.KeyVaultUri,
                    TenantId = _configuration.KeyVaultTenantId,
                    ClientId = _configuration.KeyVaultClientId,
                    ClientSecret = _configuration.KeyVaultClientSecret
                };

                // Use the client options to authenticate and create the Azure KeyVault client.
                return _keyVaultClientFactory.AuthenticateAndCreateClient(keyVaultClientOptions);
            }
            catch (Exception ex)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.UnknownFault, ex);
            }
        }

        public async Task<IExchangeClient> CreateExchangeClient(IKeyVaultClient keyVaultClient)
        {
            try
            {
                ExchangeClientOptions exchangeClientOptions = new ExchangeClientOptions
                {
                    Authority = _configuration.ExchangeAuthority,
                    Resource = _configuration.ExchangeResourceId,
                    ClientId = _configuration.ExchangeClientId,
                    WebUri = _configuration.ExchangeOnlineServer,
                    UserName = await keyVaultClient.GetSecret(_configuration.SharedMailBoxUserNameSecret),
                    Password = await keyVaultClient.GetSecret(_configuration.SharedMailBoxPasswordSecret)
                };

                return await _exchangeClientFactory.AuthenticateAndCreateClient(exchangeClientOptions);
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.Handle(ex =>
                {
                    if (ex is RequestFailedException)
                    {
                        throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.KeyVaultRequestFault, ex);
                    }

                    return false;
                });

                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.UnknownFault, aggregateException);
            }
            catch (AuthenticationFailedException ex)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.KeyVaultAuthenticationFault, ex);
            }
            catch (Exception ex)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.UnknownFault, ex);
            }
        }

        #endregion
    }
}