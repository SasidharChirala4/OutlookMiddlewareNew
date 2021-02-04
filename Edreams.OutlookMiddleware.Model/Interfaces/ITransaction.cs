using System;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.Model.Interfaces
{
    public interface ITransaction
    {
        TransactionStatus Status { get; set; }
        DateTime? Scheduled { get; set; }
        DateTime? ProcessingStarted { get; set; }
        DateTime? ProcessingFinished { get; set; }
    }
}