using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model.Base;

namespace Edreams.OutlookMiddleware.Model
{
    public class Email : ModelBase, ILongSysId
    {
        public string EntryId { get; set; }
        public string EwsId { get; set; }
        public EmailStatus Status { get; set; }
        public Batch Batch { get; set; }
    }
}