using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IBatchManager
    {
        Task<CommitBatchResponse> CommitBatch(CommitBatchRequest request);

        Task<CancelBatchResponse> CancelBatch(CancelBatchRequest request);
    }
}