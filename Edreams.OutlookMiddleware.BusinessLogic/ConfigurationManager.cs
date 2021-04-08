using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ConfigurationManager : IConfigurationManager
    {
        #region <| Dependencies |>

        private readonly IExchangeAndKeyVaultHelper _exchangeAndKeyVaultHelper;
        private readonly ISecurityContext _securityContext;

        #endregion

        #region <| Construction |>

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationManager" /> class.
        /// </summary>
        /// <param name="exchangeAndKeyVaultHelper">The exchange and key vault helper.</param>
        /// <param name="securityContext">The security context.</param>
        public ConfigurationManager(
            IExchangeAndKeyVaultHelper exchangeAndKeyVaultHelper,
            ISecurityContext securityContext)
        {
            _exchangeAndKeyVaultHelper = exchangeAndKeyVaultHelper;
            _securityContext = securityContext;
        }

        #endregion

        #region <| IConfigurationManager Implementation |>

        public async Task<GetSharedMailBoxResponse> GetSharedMailBox()
        {
            // Create a client for Azure KeyVault, authenticated using the appsettings.json settings.
            IKeyVaultClient keyVaultClient = _exchangeAndKeyVaultHelper.CreateKeyVaultClient();

            // Create a client for EWS, authenticated using data from Azure KeyVault.
            IExchangeClient exchangeClient = await _exchangeAndKeyVaultHelper.CreateExchangeClient(keyVaultClient);

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
    }
}