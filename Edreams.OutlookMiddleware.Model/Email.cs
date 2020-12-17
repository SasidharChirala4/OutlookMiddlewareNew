using Edreams.OutlookMiddleware.Model.Base;
using Edreams.OutlookMiddleware.Model.Enums;

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