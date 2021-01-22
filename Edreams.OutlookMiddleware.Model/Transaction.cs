using System;
using Edreams.OutlookMiddleware.Model.Base;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class Transaction : ModelBase, ILongSysId
    {
        public Guid BatchId { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public TransactionStatus Status { get; set; }

        public DateTime? Scheduled { get; set; }

        public DateTime? ProcessingStarted { get; set; }

        public DateTime? ProcessingFinished { get; set; }

        public string ProcessingEngine { get; set; }

        public Guid CorrelationId { get; set; }
    }
}