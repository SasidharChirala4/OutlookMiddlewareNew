using System;
using System.Collections.Generic;
using Edreams.Common.Web.Contracts;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Specific;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Response for Created Mail
    /// </summary>
    public class CreateMailResponse : Response
    {
        /// <summary>
        /// Constructor for CreateMail response
        /// </summary>
        public CreateMailResponse()
        {
            Attachments = new List<AttachmentResponse>();
        }

        /// <summary>
        /// Unique Id of Mail
        /// </summary>
        public Guid FileId { get; set; }
        /// <summary>
        /// List of attachments in the Mail
        /// </summary>
        public IList<AttachmentResponse> Attachments { get; set; }
    }
}