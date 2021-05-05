using System;
using System.Collections.Generic;
using Edreams.Common.Web.Contracts;
using Edreams.OutlookMiddleware.Enums;

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

        /// <summary>
        /// Email upload option
        /// </summary>
        public EmailUploadOptions UploadOption { get; set; }
    }
}