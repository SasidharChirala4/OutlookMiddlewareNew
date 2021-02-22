using Edreams.OutlookMiddleware.Enums;
using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// Contract that represents Email Recipient
    /// </summary>
    public class EmailRecipientDto
    {
        /// <summary>
        /// Recipient
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// EmailRecipient Type
        /// </summary>
        public EmailRecipientType Type { get; set; }

        /// <summary>
        /// EmailId
        /// </summary>
        public Guid EmailId { get; set; }
    }
}
