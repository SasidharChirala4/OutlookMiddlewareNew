using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface ITransactionQueueManager
    {
        /// <summary>
        /// Determines whether the specified transaction is available.
        /// </summary>
        /// <param name="transactionId">The unique identifier for the transaction.</param>
        /// <returns>True if the specified transaction is available, false otherwise.</returns>
        Task<bool> IsTransactionAvailable(Guid transactionId);

        /// <summary>
        /// Gets transactions based on object id
        /// </summary>
        /// <param name="transactionId">The object id (Customer/Project/Permission/ProjectTask).</param>
        /// <returns>The requested transaction.</returns>
        Task<TransactionDto> GetTransaction(Guid transactionId);

        /// <summary>
        /// Gets the status of the specified transaction.
        /// </summary>
        /// <param name="transactionId">The unique identifier of the transaction to get the status for.</param>
        /// <returns>The status of the specified transaction, or null if the transaction does not exist.</returns>
        Task<TransactionStatus?> GetTransactionStatus(Guid transactionId);

        /// <summary>
        /// Gets the next upload transaction to process.
        /// </summary>
        /// <returns>The requested upload transaction.</returns>
        Task<TransactionDto> GetNextUploadTransaction();

        /// <summary>
        /// Gets the next categorization transaction to process.
        /// </summary>
        /// <returns>The requested categorization transaction.</returns>
        Task<TransactionDto> GetNextCategorizationTransaction();

        /// <summary>
        /// Creates a new upload transaction for a specific batch.
        /// </summary>
        /// <param name="batchId">The unique identifier for the batch that needs to be uploaded.</param>
        Task CreateUploadTransaction(Guid batchId);

        /// <summary>
        /// Creates a new categorization transaction for a specific batch.
        /// </summary>
        /// <param name="batchId">The unique identifier for the batch that needs to be categorized.</param>
        Task CreateCategorizationTransaction(Guid batchId);

        /// <summary>
        /// Updates the status of an existing transaction.
        /// </summary>
        /// <param name="transactionId">The unique identifier for the transaction.</param>
        /// <param name="status">The new transaction status.</param>
        /// <returns>The updated transaction.</returns>
        Task<TransactionDto> UpdateTransactionStatus(Guid transactionId, TransactionStatus status);

        /// <summary>
        /// Updates the status of an existing transaction.
        /// </summary>
        /// <param name="transactionId">The unique identifier for the transaction.</param>
        /// <param name="status">The new transaction status.</param>
        /// <param name="engine">The name of the engine changing the status.</param>
        /// <returns>The updated transaction.</returns>
        Task<TransactionDto> UpdateTransactionStatus(Guid transactionId, TransactionStatus status, string engine);

        /// <summary>
        /// Updates the status of an existing transaction and archive it.
        /// </summary>
        /// <param name="transactionId">The unique identifier for the transaction.</param>
        /// <param name="status">The new transaction status.</param>
        /// <returns>The archived transaction.</returns>
        Task<TransactionDto> UpdateTransactionStatusAndArchive(Guid transactionId, TransactionStatus status);

        /// <summary>
        /// Updates the status of an existing transaction and archive it.
        /// </summary>
        /// <param name="transactionId">The unique identifier for the transaction.</param>
        /// <param name="status">The new transaction status.</param>
        /// <param name="engine">The name of the engine changing the status.</param>
        /// <returns>The archived transaction.</returns>
        Task<TransactionDto> UpdateTransactionStatusAndArchive(Guid transactionId, TransactionStatus status, string engine);

        /// <summary>
        /// Gets a number of message statistics for the provisioning queues.
        /// </summary>
        /// <returns>A statistics object for provisioning containing a number of message counts.</returns>
        Task<GetTransactionQueueStatisticsResponse> GetTransactionQueueStatistics();
    }
}