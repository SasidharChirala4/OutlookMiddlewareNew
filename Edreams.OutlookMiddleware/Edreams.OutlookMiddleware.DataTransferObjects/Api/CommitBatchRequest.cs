using System;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CommitBatchRequest : Request
    {
        public Guid BatchId { get; set; }
    }
}