using System;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model.Base;
using Edreams.OutlookMiddleware.Model.Interfaces;

namespace Edreams.OutlookMiddleware.Model
{
    public class Transaction : ModelBase, ILongSysId, ITransaction
    {
        public Guid BatchId { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }

        public DateTime? Scheduled { get; set; }

        public DateTime? ProcessingStarted { get; set; }

        public DateTime? ProcessingFinished { get; set; }

        public string ProcessingEngine { get; set; }

        public Guid CorrelationId { get; set; }
    }
}