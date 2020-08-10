using System;
using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Specific;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CreateMailResponse : Response
    {
        public CreateMailResponse()
        {
            Attachments = new List<AttachmentResponse>();
        }

        public Guid FileId { get; set; }

        public IList<AttachmentResponse> Attachments { get; set; }
    }
}