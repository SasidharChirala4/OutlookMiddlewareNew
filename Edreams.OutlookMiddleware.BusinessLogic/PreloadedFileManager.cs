using System;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Exceptions;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    /// <summary>
    /// Manager containing different method to handle file operations
    /// </summary>
    public class PreloadedFileManager : IPreloadedFileManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly ISecurityContext _securityContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreloadedFileManager" /> class.
        /// </summary>
        public PreloadedFileManager(
            IRepository<FilePreload> preloadedFilesRepository,
            ISecurityContext securityContext)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _securityContext = securityContext;
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

            if (string.IsNullOrEmpty(storagePath))
                throw new EdreamsException("StoragePath for saving file is required!");

            if (Guid.Empty.Equals(request.FileId))
                throw new EdreamsException("A valid fileId is required!");

            #endregion

            //Update the File details in the database
            FilePreload preloadedFile = await _preloadedFilesRepository.GetSingle(x => x.Id == request.FileId);
            if (preloadedFile != null)
            {
                preloadedFile.TempPath = request.TempPath;
                preloadedFile.FileName = request.FileName;
                preloadedFile.Size = request.FileSize;
                preloadedFile.PreloadedOn = DateTime.UtcNow;
                preloadedFile.FileStatus = FilePreloadStatus.Ready;
                await _preloadedFilesRepository.Update(preloadedFile);

                return new UpdateFileResponse
                {
                    CorrelationId = _securityContext.CorrelationId,
                    FileId = preloadedFile.Id,
                    FileName = preloadedFile.FileName,
                    TempPath = preloadedFile.TempPath
                };
            }

            throw new EdreamsException("Preloaded file not found!");
        }
    }
}