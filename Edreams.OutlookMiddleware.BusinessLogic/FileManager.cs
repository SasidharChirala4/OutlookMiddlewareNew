using System;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Exceptions.Constants;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
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
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsExceptionCode.UnknownFault);
            }

            file.Status = status;

            await _filesRepository.Update(file);
        }
    }
}