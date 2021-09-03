using System;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// Contract that represents a transaction.
    /// </summary>
    public class TransactionDto
    {
        /// <summary>
        /// The unique transaction ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unique batch ID related to this transaction.
        /// </summary>
        public Guid BatchId { get; set; }

        /// <summary>
        /// The unique correlation ID for this transaction.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// The status for this transaction.
        /// </summary>
        public TransactionStatus Status { get; set; }
    }
}