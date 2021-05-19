using System;
using System.Collections.Generic;
using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class Email : ModelBase, ILongSysId
    {
        public string InternetMessageId { get; set; }
        public string EntryId { get; set; }
        public string EwsId { get; set; }
        public EmailStatus Status { get; set; }
        public Guid EdreamsReferenceId { get; set; }
        public EmailKind EmailKind { get; set; }
        public Batch Batch { get; set; }
        public virtual IList<File> Files { get; set; }
        public virtual IList<EmailRecipient> EmailRecipients { get; set; }
        public virtual IList<Task> Tasks { get; set; }
        public EmailUploadOptions UploadOption { get; set; }

    }
}