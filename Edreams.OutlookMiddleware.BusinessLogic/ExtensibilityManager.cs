using Edreams.Contracts.Data.Enums;
using Edreams.Contracts.Data.Extensibility;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Edreams.OutlookMiddleware.Common.Exceptions;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ExtensibilityManager : IExtensibilityManager
    {
        #region <| Private Members |>

        private readonly IRestHelper<SuggestedSite> _suggestedSiteRestHelper;
        private readonly IValidator _validator;
        private readonly ILogger _logger;

        #endregion

        #region <| Construction |>
        public ExtensibilityManager(IRestHelper<SuggestedSite> suggestedSiteRestHelper, IValidator validator, ILogger logger)
        {
            _suggestedSiteRestHelper = suggestedSiteRestHelper;
            _validator = validator;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Method to set suggested sites.
        /// </summary>
        /// <param name="from">The user to be added as suggestion.</param>
        /// <param name="siteUrl">Url of the site.</param>
        /// <param name="principalName">PrincipalName of the user.</param>
        /// <returns></returns>
        public async Task SetSuggestedSites(string from, string siteUrl, string principalName)
        {
            // Validations
            _validator.ValidateString(from, ValidationMessages.WebApi.FromRequired);
            _validator.ValidateString(siteUrl, ValidationMessages.WebApi.SiteUrlRequired);
            _validator.ValidateString(principalName, ValidationMessages.WebApi.PrincipalNameRequired);

            // Suggestedsite object values entry 
            SuggestedSite suggestedSite = new SuggestedSite
            {
                Suggestion = from,
                SiteUrl = siteUrl,
                Type = SiteType.ProjectSite,
                PrincipalName = principalName
            };
            try
            {
                // Set SuggestedSites using rest helper 
                _ = await _suggestedSiteRestHelper.CreateNew("sites/suggested", suggestedSite);
            }
            catch (EdreamsException ex)
            {
                // TODO : Need to check with johnny/sasi about proper log message 
                _logger.LogError("Error at setting suggested sites.", ex);
            }
            catch (Exception ex)
            {
                // TODO : Need to check with johnny/sasi about proper log message
                // This handles all remaining exceptions.
                _logger.LogError("Unexpected error occured while setting suggested sites.", ex);
            }
        }
    }
}
