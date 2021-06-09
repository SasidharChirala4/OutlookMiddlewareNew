using Edreams.Contracts.Data.Common;
using System.Threading.Tasks;
using ProjectTask = Edreams.Contracts.Data.Extensibility.ProjectTask;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IExtensibilityManager
    {
        /// <summary>
        /// Method to set suggested sites.
        /// </summary>
        /// <param name="from">The user to be added as suggestion.</param>
        /// <param name="siteUrl">Url of the site.</param>
        /// <param name="principalName">PrincipalName of the user.</param>
        /// <returns></returns>
        Task SetSuggestedSites(string from, string siteUrl, string principalName);

        /// <summary>
        /// Method to upload the Email/ Attachment through WebAPI to SharePoint.
        /// </summary>
        /// <param name="itemId">Unique identifier of the Email/ Attachment.</param>
        /// <param name="itemBytes">The binary item data to upload.</param>
        /// <param name="siteUrl">Url of the site where Email/ Attachment should be uploaded.</param>
        /// <param name="folder">Url of the folder where Email/ Attachment should be uploaded.</param>
        /// <param name="fileName">Email/ Attachment Name.</param>
        /// <param name="overwrite">Flag to overwrite the file.</param>
        /// <returns>SharepointFile object of the uploaded file</returns>
        Task<SharePointFile> UploadFile(byte[] itemBytes, string siteUrl, string folder, string fileName, bool overwrite);

        /// <summary>
        /// Creates project task in Edreams.
        /// </summary>
        /// <param name="projectTask">project task object</param>
        /// <returns>Created project Task</returns>
        Task<ProjectTask> CreateEdreamsProjectTask(ProjectTask projectTask);
    }
}