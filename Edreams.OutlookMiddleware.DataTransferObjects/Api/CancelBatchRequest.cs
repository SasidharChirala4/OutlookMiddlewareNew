using System;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CancelBatchRequest : Request
    {
        public Guid BatchId { get; set; }
    }
}