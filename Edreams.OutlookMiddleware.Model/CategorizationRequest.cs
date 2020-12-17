using Edreams.OutlookMiddleware.Model.Base;
using Edreams.OutlookMiddleware.Model.Enums;


namespace Edreams.OutlookMiddleware.Model
{
   public class CategorizationRequest : ModelBase ,ILongSysId
    {
        public string UserPrincipalName { get; set; }
        public string InternetMessageId { get; set; }
        public bool IsCompose { get; set; }
        public bool Sent { get; set; }
        public CategorizationRequestType CategorizationRequestType { get; set; }
    }
}
