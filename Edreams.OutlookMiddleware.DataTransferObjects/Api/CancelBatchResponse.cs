using System;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CancelBatchResponse : Response
    {
        public Guid BatchId { get; set; }
        public int NumberOfCancelledFiles { get; set; }
    }
}