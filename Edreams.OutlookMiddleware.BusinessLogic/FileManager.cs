using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class FileManager : IFileManager
    {
        private readonly IRepository<File> _filesRepository;
        private readonly IExceptionFactory _exceptionFactory;

        public FileManager(
            IRepository<File> filesRepository,
            IExceptionFactory exceptionFactory)
        {
            _filesRepository = filesRepository;
            _exceptionFactory = exceptionFactory;
        }

        public async Task UpdateFileStatus(Guid fileId, FileStatus status)
        {
            File file = await _filesRepository.GetSingle(x => x.Id == fileId);

            if (file == null)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.UNKNOWN_FAULT);
            }

            file.Status = status;

            await _filesRepository.Update(file);
        }
    }
}