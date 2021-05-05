using System;
using Edreams.Common.Web.Contracts;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CommitBatchResponse : Response
    {
        public Guid BatchId { get; set; }
        public int NumberOfEmails { get; set; }
        public int NumberOfFiles { get; set; }
    }
}