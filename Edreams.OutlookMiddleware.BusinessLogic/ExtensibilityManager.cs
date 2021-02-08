using Edreams.Contracts.Data.Enums;
using Edreams.Contracts.Data.Extensibility;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Net;
using Edreams.OutlookMiddleware.Common.Exceptions;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ExtensibilityManager : IExtensibilityManager
    {
        #region <| Private Members |>

        private readonly IRestHelper<SuggestedSite> _restHelper;
        private readonly IValidator _validator;
        private readonly ILogger _logger;

        #endregion

        #region <| Construction |>
        public ExtensibilityManager(IRestHelper<SuggestedSite> restHelper, IValidator validator, ILogger logger)
        {
            _restHelper = restHelper;
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
                // Set SuggestedSites from rest helper 
                _ = await _restHelper.CreateNew("sites/suggested", suggestedSite);
            }
            catch (EdreamsException ex)
            {
                _logger.LogError("error at setting suggested sites.", ex);
            }
            catch (Exception ex)
            {
                // This handles all remaining exceptions.
                _logger.LogError("Unexpected error occured while setting suggested sites.", ex);
            }
        }
    }
}
