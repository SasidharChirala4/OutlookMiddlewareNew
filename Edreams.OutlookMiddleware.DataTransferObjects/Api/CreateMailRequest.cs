using System;
using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Specific;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class CreateMailRequest : Request
    {
        public CreateMailRequest()
        {
            Attachments = new List<Attachment>();
        }

        public Guid BatchId { get; set; }
        public string MailEntryId { get; set; }
        public string MailEwsId { get; set; }
        public string MailSubject { get; set; }
        public Guid EmailReferenceId { get; set; }
        public EmailKind EmailKind { get; set; }
        public IList<Attachment> Attachments { get; set; }
    }
}