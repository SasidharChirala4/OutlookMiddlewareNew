using System;
using Edreams.Common.Web.Contracts;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CancelBatchRequest : Request
    {
        public Guid BatchId { get; set; }
    }
}