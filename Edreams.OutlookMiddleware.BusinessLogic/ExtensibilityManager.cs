using System;
using System.Threading.Tasks;
using Edreams.Common.Exceptions;
using Edreams.Common.Logging.Interfaces;
using Edreams.Contracts.Data.Common;
using Edreams.Contracts.Data.Enums;
using Edreams.Contracts.Data.Extensibility;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using RestSharp;
using ProjectTask = Edreams.Contracts.Data.Extensibility.ProjectTask;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ExtensibilityManager : IExtensibilityManager
    {
        #region <| Private Members |>

        private readonly IRestHelper<SuggestedSite> _suggestedSiteRestHelper;
        private readonly IRestHelper<SharePointFile> _sharePointFileRestHelper;
        private readonly IRestHelper<ProjectTask> _projectTaskRestHelpler;
        private readonly IValidator _validator;
        private readonly IEdreamsLogger<ExtensibilityManager> _logger;

        #endregion

        #region <| Construction |>

        public ExtensibilityManager(
            IRestHelper<SuggestedSite> suggestedSiteRestHelper,
            IRestHelper<SharePointFile> sharePointFileRestHelper,
            IRestHelper<ProjectTask> projectTaskRestHelpler,
            IValidator validator, IEdreamsLogger<ExtensibilityManager> logger)
        {
            _suggestedSiteRestHelper = suggestedSiteRestHelper;
            _sharePointFileRestHelper = sharePointFileRestHelper;
            _projectTaskRestHelpler = projectTaskRestHelpler;
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
                _logger.LogError(ex, "Error at setting suggested sites.");
            }
            catch (Exception ex)
            {
                // TODO : Need to check with johnny/sasi about proper log message
                // This handles all remaining exceptions.
                _logger.LogError(ex, "Unexpected error occured while setting suggested sites.");
            }
        }

        /// <summary>
        /// Method to upload the Email/ Attachment through WebAPI to SharePoint.
        /// </summary>
        /// <param name="itemBytes">The binary item data to upload.</param>
        /// <param name="siteUrl">Url of the site where Email/ Attachment should be uploaded.</param>
        /// <param name="folder">Url of the folder where Email/ Attachment should be uploaded.</param>
        /// <param name="fileName">Email/ Attachment Name.</param>
        /// <param name="overwrite">Flag to overwrite the file.</param>
        /// <returns>Uploaded file url</returns>
        public async Task<SharePointFile> UploadFile(byte[] itemBytes, string siteUrl, string folder, string fileName, bool overwrite)
        {
            // Validations
            _validator.ValidateString(siteUrl, ValidationMessages.WebApi.SiteUrlRequired);
            _validator.ValidateString(folder, ValidationMessages.WebApi.FolderRequired);
            _validator.ValidateString(fileName, ValidationMessages.WebApi.FileNameRequired);

            try
            {
                // Assign values to file parameter header
                FileParameter fileParameter = new FileParameter()
                {
                    Name = "file",
                    Writer = s => s.Write(itemBytes, 0, itemBytes.Length),
                    FileName = fileName,
                    ContentLength = itemBytes.Length,
                    ContentType = "multipart/form-data"
                };

                // upload files from rest helper .
                var response = await _sharePointFileRestHelper.CreateFile($"/file/content?siteUrl={siteUrl}&folderUrl={folder}&overWrite={overwrite}", fileParameter);
                if (response.Content != null)
                {
                    _logger.LogInformation($"File [{fileParameter.FileName}] uploaded to site [{siteUrl}] successfully.");
                    return response.Content;
                }

                // this scenario won't occur mostly, because we are handling all kind of exceptions in rest helper
                _logger.LogInformation($"File [{fileParameter.FileName}] uploaded failed to site [{siteUrl}] with out any error.");
                return null;
            }
            catch (EdreamsException ex)
            {
                // TODO : Need to check with johnny/sasi about proper log message and return value
                _logger.LogError(ex, "Error at upload file.");
                return null;
            }
            catch (Exception ex)
            {
                // TODO : Need to check with johnny/sasi about proper log message and return value
                _logger.LogError(ex, "Unexpected error occured while uploading file.");
                return null;
            }
        }

        /// <summary>
        /// Creates project task in Edreams.
        /// </summary>
        /// <param name="projectTask">project task object</param>
        /// <returns>Created project Task</returns>
        public async Task<ProjectTask> CreateEdreamsProjectTask(ProjectTask projectTask)
        {
            try
            {
                // create project task from rest helper .
                var response = await _projectTaskRestHelpler.CreateNew($"/projects/{projectTask.ProjectId}/tasks", projectTask,false);
                if (response.Content != null)
                {
                    _logger.LogInformation($"Project Task [{projectTask.Title}] Created successfully.");
                    return response.Content;
                }
                return null;
            }
            catch (EdreamsException ex)
            {
                _logger.LogError(ex, "Error while creating Project Task.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured while creating Project Task.");
                return null;
            }
        }

        
    }
}