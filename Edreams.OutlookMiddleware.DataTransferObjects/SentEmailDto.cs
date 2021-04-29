using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// SentEmailDto
    /// </summary>
    public class SentEmailDto
    {
        /// <summary>
        /// NotFound
        /// </summary>
        public static SentEmailDto NotFound { get; } = new SentEmailDto { IsFound = false };

        /// <summary>
        /// IsFound
        /// </summary>
        public bool IsFound { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// InternetMessageId
        /// </summary>
        public string InternetMessageId { get; set; }

        /// <summary>
        /// EwsId
        /// </summary>
        public string EwsId { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Attachments
        /// </summary>

        public List<SentEmailAttachmentDto> Attachments { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SentEmailDto()
        {
            Attachments = new List<SentEmailAttachmentDto>();
        }

    }
}
