using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IBatchManager
    {
        Task<BatchDetailsDto> GetBatchDetails(Guid batchId);

        Task UpdateBatchStatus(Guid batchId, BatchStatus status);

        Task<CommitBatchResponse> CommitBatch(CommitBatchRequest request);

        Task<CancelBatchResponse> CancelBatch(CancelBatchRequest request);
    }
}