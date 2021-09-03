namespace Edreams.OutlookMiddleware.Enums
{
    /// <summary>
    /// Possible statuses for a transaction.
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// The transaction is queued.
        /// </summary>
        Queued,

        /// <summary>
        /// The transaction is scheduled for processing.
        /// </summary>
        Scheduled,

        /// <summary>
        /// Scheduling of the transaction has failed.
        /// </summary>
        ScheduleFailed,

        /// <summary>
        /// Processing of the transaction has started.
        /// </summary>
        ProcessingStarted,

        /// <summary>
        /// Processing of the transaction has succeeded.
        /// </summary>
        ProcessingSucceeded,

        /// <summary>
        /// Processing of the transaction has failed.
        /// </summary>
        ProcessingFailed,

        /// <summary>
        /// Processing of the transaction has partially succeeded.
        /// </summary>
        ProcessingPartiallySucceeded,

        /// <summary>
        /// Retried the transactions
        /// </summary>
        Retried,

        /// <summary>
        /// Skipped the transactions
        /// </summary>
        Skipped,

        /// <summary>
        /// The transaction has expired and is marked for cleanup.
        /// </summary>
        Expired
    }
}