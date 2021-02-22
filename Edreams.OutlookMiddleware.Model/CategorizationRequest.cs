using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model.Base;


namespace Edreams.OutlookMiddleware.Model
{
    public class CategorizationRequest : ModelBase, ILongSysId
    {
        public string EmailAddress { get; set; }
        public string InternetMessageId { get; set; }
        public CategorizationRequestStatus Status { get; set; }
        public CategorizationRequestType Type { get; set; }
    }
}