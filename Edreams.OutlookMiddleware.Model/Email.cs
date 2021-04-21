using System.Collections.Generic;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model.Base;

namespace Edreams.OutlookMiddleware.Model
{
    public class Email : ModelBase, ILongSysId
    {
        public string InternetMessageId { get; set; }
        public string EntryId { get; set; }
        public string EwsId { get; set; }
        public EmailStatus Status { get; set; }
        public Batch Batch { get; set; }
        public virtual IList<File> Files { get; set; }
        public virtual IList<EmailRecipient> EmailRecipients { get; set; }
        public EmailUploadOptions UploadOption { get; set; }

    }
}