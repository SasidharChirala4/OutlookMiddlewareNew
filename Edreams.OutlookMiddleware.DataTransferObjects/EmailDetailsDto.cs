using Edreams.OutlookMiddleware.Enums;
using System;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class EmailDetailsDto
    {
        public Guid Id { get; set; }

        public List<FileDetailsDto> Files { get; set; }
        public Guid EdreamsReferenceId { get; set; }
        public EmailKind EmailKind { get; set; }
        public string InternetMessageId { get; set; }
        public EmailUploadOptions UploadOption { get; set; }
        public ProjectTaskDto ProjectTaskDto { get; set; }
        public List<EmailRecipientDto> EmailRecipients { get; set; }
    }
}