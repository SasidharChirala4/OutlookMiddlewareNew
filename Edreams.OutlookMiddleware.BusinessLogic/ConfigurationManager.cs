using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Azure;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.Common.Exchange;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Common.KeyVault;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ConfigurationManager : IConfigurationManager
    {
        #region <| Dependencies |>

        private readonly IExchangeClientFactory _exchangeClientFactory;
        private readonly IKeyVaultClientFactory _keyVaultClientFactory;
        private readonly IEdreamsConfiguration _configuration;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly ISecurityContext _securityContext;

        #endregion

        #region <| Construction |>

        /// <summary>Initializes a new instance of the <see cref="ConfigurationManager" /> class.</summary>
        /// <param name="exchangeClientFactory">The exchange client factory.</param>
        /// <param name="keyVaultClientFactory">The key vault client factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="exceptionFactory">The exception factory.</param>
        /// <param name="securityContext">The security context.</param>
        public ConfigurationManager(
            IExchangeClientFactory exchangeClientFactory,
            IKeyVaultClientFactory keyVaultClientFactory,
            IEdreamsConfiguration configuration,
            IExceptionFactory exceptionFactory,
            ISecurityContext securityContext)
        {
            _exchangeClientFactory = exchangeClientFactory;
            _keyVaultClientFactory = keyVaultClientFactory;
            _configuration = configuration;
            _exceptionFactory = exceptionFactory;
            _securityContext = securityContext;
        }

        #endregion

        #region <| IConfigurationManager Implementation |>

        public async Task<GetSharedMailBoxResponse> GetSharedMailBox()
        {
            // Create a client for Azure KeyVault, authenticated using the appsettings.json settings.
            IKeyVaultClient keyVaultClient = CreateKeyVaultClient();

            // Create a client for EWS, authenticated using data from Azure KeyVault.
            IExchangeClient exchangeClient = await CreateExchangeClient(keyVaultClient);

            // Use the client for EWS to resolve the email address for the current 
            string emailAddress = await exchangeClient.ResolveEmailAddress();

            // Return a response containing the resolved email address and a correlation ID.
            return new GetSharedMailBoxResponse
            {
                EmailAddress = emailAddress,
                CorrelationId = _securityContext.CorrelationId
            };
        }

        #endregion

        #region <| Helper Method |>

        private IKeyVaultClient CreateKeyVaultClient()
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
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.UNKNOWN_FAULT, ex);
            }
        }

        private async Task<IExchangeClient> CreateExchangeClient(IKeyVaultClient keyVaultClient)
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
                        return true;
                    }

                    return false;
                });
            }
            catch (Exception ex)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.UNKNOWN_FAULT, ex);
            }
        }

        #endregion
    }
}