using System;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model.Base;

namespace Edreams.OutlookMiddleware.Model
{
    public class FilePreload : ModelBase, ILongSysId
    {
        public Guid BatchId { get; set; }
        public Guid EmailId { get; set; }
        public string EntryId { get; set; }
        public string EwsId { get; set; }
        public string EmailSubject { get; set; }
        public string AttachmentId { get; set; }
        public string TempPath { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime PreloadedOn { get; set; }
        public Guid EmailReferenceId { get; set; }
        public FileKind Kind { get; set; }
        public EmailKind EmailKind { get; set; }
        public EmailPreloadStatus Status { get; set; }
        public FilePreloadStatus FileStatus { get; set; }
    }
}