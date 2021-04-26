using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class File : ModelBase, ILongSysId
    {
        public FileKind Kind { get; set; }
        public string EmailSubject { get; set; }
        public string AttachmentId { get; set; }
        public string TempPath { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public FileStatus Status { get; set; }

        public Email Email { get; set; }
    }
}