using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IFileManager
    {
        Task UpdateFileStatus(Guid fileId, FileStatus status);
    }
}