using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;


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