using System;
using System.Collections.Generic;
using System.Text;
using Edreams.Contracts.Data.Common;
using ProjectTask = Edreams.Contracts.Data.Extensibility.ProjectTask;
using Edreams.OutlookMiddleware.DataTransferObjects;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IProjectTaskManager
    {
        /// <summary>
        /// Method to get the edreams project task object.
        /// </summary>
        /// <param name="emailDetails">Email object</param>
        /// <param name="sharepointFileUploads">fie upload details.</param>
        /// <returns></returns>
        ProjectTask GetEdreamsProjectTask(EmailDetailsDto emailDetails, List<SharePointFile> sharepointFileUploads);
    }
}
