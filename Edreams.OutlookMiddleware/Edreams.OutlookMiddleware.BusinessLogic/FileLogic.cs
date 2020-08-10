using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class FileLogic : IFileLogic
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;

        public FileLogic(
            IRepository<FilePreload> preloadedFilesRepository)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
        }

        public async Task<UpdateFileResponse> UpdateFile(UpdateFileRequest request)
        {
            FilePreload preloadedFile = await _preloadedFilesRepository.GetSingle(x => x.Id == request.FileId);

            preloadedFile.TempPath = request.TempPath;
            preloadedFile.FileName = request.FileName;
            preloadedFile.Size = request.FileSize;
            preloadedFile.FileStatus = FilePreloadStatus.Ready;

            await _preloadedFilesRepository.Update(preloadedFile);

            return new UpdateFileResponse();
        }
    }
}