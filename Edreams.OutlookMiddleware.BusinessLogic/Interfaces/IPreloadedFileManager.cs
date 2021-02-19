using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IPreloadedFileManager
    {
        /// <summary>
        /// Method to update the file details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="storagePath"></param>
        /// <returns></returns>
        Task<UpdateFileResponse> UpdateFile(UpdateFileRequest request, string storagePath);
    }
}