using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IStatusManager
    {
        Task<GetStatusResponse> GetStatus();
    }
}