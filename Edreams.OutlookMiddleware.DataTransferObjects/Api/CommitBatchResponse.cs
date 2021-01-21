using System;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CommitBatchResponse : Response
    {
        public Guid BatchId { get; set; }
        public int NumberOfEmails { get; set; }
        public int NumberOfFiles { get; set; }
    }
}