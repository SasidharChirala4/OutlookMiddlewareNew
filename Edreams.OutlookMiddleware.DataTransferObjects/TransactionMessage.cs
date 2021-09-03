using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// Contract that represents a transaction, packaged within a ServiceBus message.
    /// </summary>
    public class TransactionMessage
    {
        /// <summary>
        /// The unique identifier for the transaction.
        /// </summary>
        public Guid TransctionId { get; set; }

        /// <summary>
        /// The unique identifier for the batch.
        /// </summary>
        public Guid BatchId { get; set; }

        /// <summary>
        /// The unique correlation identifier.
        /// </summary>
        public Guid CorrelationId { get; set; }
    }
}