using System;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        public Guid BatchId { get; set; }

        public TransactionStatus Status { get; set; }
    }
}