using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IFileLogic
    {
        Task<UpdateFileResponse> UpdateFile(UpdateFileRequest request);
    }
}