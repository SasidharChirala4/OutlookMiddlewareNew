using System;
using Edreams.Common.Web.Contracts;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CancelBatchResponse : Response
    {
        public Guid BatchId { get; set; }
        public int NumberOfCancelledFiles { get; set; }
    }
}