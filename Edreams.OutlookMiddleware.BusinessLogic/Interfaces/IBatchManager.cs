using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IBatchManager
    {
        /// <summary>
        /// Gets all the details, including emails and files, for a specified batch.
        /// </summary>
        /// <param name="batchId">The unique batch identifier.</param>
        /// <returns>A DTO containing all email details and file details.</returns>
        Task<BatchDetailsDto> GetBatchDetails(Guid batchId);

        /// <summary>
        /// Updates the status for a specified batch.
        /// </summary>
        /// <param name="batchId">The unique batch identifier.</param>
        /// <param name="status">The new batch status.</param>
        Task UpdateBatchStatus(Guid batchId, BatchStatus status);

        /// <summary>
        /// Commits the batch specified by the request object.
        /// </summary>
        /// <param name="batchId">The unique batch identifier.</param>
        /// <param name="request">A request object containing the unique batch identifier and the metadata needed to commit the specified batch.</param>
        /// <returns>A response object containing the number of emails and files that are part of the batch.</returns>
        Task<CommitBatchResponse> CommitBatch(Guid batchId, CommitBatchRequest request);

        /// <summary>
        /// Cancels the batch specified by the request object.
        /// </summary>
        /// <param name="batchId">The unique batch identifier.</param>
        /// <param name="request">A request object containing the unique batch identifier.</param>
        /// <returns>A response object containing the number of files that are cancelled as part of the batch.</returns>
        Task<CancelBatchResponse> CancelBatch(Guid batchId, CancelBatchRequest request);
    }
}