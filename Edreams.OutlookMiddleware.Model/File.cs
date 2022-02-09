using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.Model
{
    public class File : ModelBase, ILongSysId
    {
        public FileKind Kind { get; set; }
        public string EmailSubject { get; set; }
        public string AttachmentId { get; set; }
        public string TempPath { get; set; }
        public long Size { get; set; }
        public bool ShouldUpload { get; set; }
        public string OriginalName { get; set; }
        public string NewName { get; set; }
        public string Extension { get; set; }
        public FileStatus Status { get; set; }
        public Email Email { get; set; }
        public List<Metadata> Metadata { get; set; }
    }
}