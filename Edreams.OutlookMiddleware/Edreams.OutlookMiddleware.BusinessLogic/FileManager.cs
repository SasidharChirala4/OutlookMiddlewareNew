using System;
using System.IO;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    /// <summary>
    /// Manager containing different method to handle file oeprations
    /// </summary>
    public class FileManager : IFileManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;

        /// <summary>Initializes a new instance of the <see cref="FileManager" /> class.
        /// </summary>
        public FileManager(
            IRepository<FilePreload> preloadedFilesRepository)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
        }

        /// <summary>
        /// Method to upload the file to temporary storage and update details in DB.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="storagePath"></param>
        /// <returns></returns>
        public async Task<UpdateFileResponse> UpdateFile(UpdateFileRequest request, string storagePath)
        {
            #region Validation
            if(string.IsNullOrEmpty(storagePath))            
                throw new EdreamsException("StoragePath for saving file is required!");            

            if (Guid.Empty.Equals(request.FileId))            
                throw new EdreamsException("A valid fileId is required!");            
            #endregion

            //Update the File details in the database
            FilePreload preloadedFile = await _preloadedFilesRepository.GetSingle(x => x.Id == request.FileId);

            preloadedFile.TempPath = request.TempPath;
            preloadedFile.FileName = request.FileId.ToString();
            preloadedFile.PreloadedOn = DateTime.UtcNow;
            preloadedFile.FileStatus = FilePreloadStatus.Ready;
            await _preloadedFilesRepository.Update(preloadedFile);

            return new UpdateFileResponse()
            {
                FileId = preloadedFile.Id,
                FileName = preloadedFile.FileName,
                TempPath = preloadedFile.TempPath                
            };
        }
    }
}