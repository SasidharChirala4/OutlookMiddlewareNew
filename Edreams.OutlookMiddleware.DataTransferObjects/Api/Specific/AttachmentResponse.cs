using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api.Specific
{
    public class AttachmentResponse
    {
        public string AttachmentId { get; set; }
        public Guid FileId { get; set; }
    }
}