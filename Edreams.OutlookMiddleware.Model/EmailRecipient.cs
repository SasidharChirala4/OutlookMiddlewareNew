using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class EmailRecipient : ModelBase, ILongSysId
    {
        public string Recipient { get; set; }
        public EmailRecipientType Type { get; set; }
        public Email Email { get; set; }
    }
}