using System;
using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Request class of CommitBatch
    /// </summary>
    public class CommitBatchRequest : Request
    {
        /// <summary>
        /// BatchId
        /// </summary>
        public Guid BatchId { get; set; }

        /// <summary>
        /// List of Email Recipients
        /// </summary>
        public List<EmailRecipientDto> EmailRecipients { get; set; }
    }
}