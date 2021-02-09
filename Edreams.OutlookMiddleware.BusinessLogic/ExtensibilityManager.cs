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
using RestSharp;
using Edreams.Contracts.Data.Common;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ExtensibilityManager : IExtensibilityManager
    {
        #region <| Private Members |>

        private readonly IRestHelper<SuggestedSite> _suggestSiteRestHelper;
        private readonly IRestHelper<SharePointFile> _sharePointFileRestHelper;
        private readonly IValidator _validator;
        private readonly ILogger _logger;

        #endregion

        #region <| Construction |>
        public ExtensibilityManager(IRestHelper<SuggestedSite> suggestSiteRestHelper,IRestHelper<SharePointFile> sharePointFileRestHelper, IValidator validator, ILogger logger)
        {
            _suggestSiteRestHelper = suggestSiteRestHelper;
            _sharePointFileRestHelper = sharePointFileRestHelper;
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
                _ = await _suggestSiteRestHelper.CreateNew("sites/suggested", suggestedSite);
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

        /// <summary>
        /// Method to upload the Email/ Attachment through WebAPI to SharePoint.
        /// </summary>
        /// <param name="itemBytes">The binary item data to upload.</param>
        /// <param name="siteUrl">Url of the site where Email/ Attachment should be uploaded.</param>
        /// <param name="folder">Url of the folder where Email/ Attachment should be uploaded.</param>
        /// <param name="itemName">Email/ Attachment Name.</param>
        /// <param name="ext">Email/ Attachment extension.</param>
        /// <param name="overwrite">Flag to overwrite the file.</param>
        /// <returns></returns>
        public async Task<string> UploadFile(byte[] itemBytes, string siteUrl, string folder, string itemName, string ext, bool overwrite)
        {
            // Validations
            _validator.ValidateString(siteUrl, ValidationMessages.WebApi.SiteUrlRequired);
            _validator.ValidateString(folder, ValidationMessages.WebApi.FolderRequired);
            _validator.ValidateString(itemName, ValidationMessages.WebApi.ItemNameRequired);

            try
            {
                // Assign values to file parameter header
                FileParameter fileParameter = new FileParameter()
                {
                    Name = "file",
                    Writer = s => s.Write(itemBytes, 0, itemBytes.Length),
                    FileName = itemName + (string.IsNullOrEmpty(ext) ? "" : "." + ext),
                    ContentLength = itemBytes.Length,
                    ContentType = "multipart/form-data"
                };

                // upload files from rest helper .
                var response = await _sharePointFileRestHelper.CreateNew($"/file/content?siteUrl={siteUrl}&folderUrl={folder}&overWrite={overwrite}", fileParameter);
                return response.Content.AbsoluteUrl;
            }
            catch (EdreamsException ex)
            {
                // TODO : Need to check with johnny/sasi about proper log message and return value
                _logger.LogError("Error at upload file.", ex);
                return null;
            }
            catch (Exception ex)
            {
                // TODO : Need to check with johnny/sasi about proper log message and return value
                _logger.LogError("Unexpected error occured while uploading file.", ex);
                return null;
            }
        }
    }
}
