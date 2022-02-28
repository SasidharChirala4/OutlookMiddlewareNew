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

        /// <summary>
        /// Project Task Details
        /// </summary>
        public ProjectTaskDto ProjectTaskDetails { get; set; }

        /// <summary>
        /// Upload location site url
        /// </summary>
        public string UploadLocationSite { get; set; }

        /// <summary>
        /// Upload location folder url
        /// </summary>
        public string UploadLocationFolder { get; set; }

        /// <summary>
        /// List of Files
        /// </summary>
        public List<FileDetailsDto> Files { get; set; }

        /// <summary>
        /// Version Comment
        /// </summary>
        public string VersionComment { get; set; }

        /// <summary>
        /// Declare as Record
        /// </summary>
        public bool DeclareAsRecord { get; set; }

        /// <summary>
        /// PrincipalName
        /// </summary>
        public string PrincipalName { get; set; }

    }
}