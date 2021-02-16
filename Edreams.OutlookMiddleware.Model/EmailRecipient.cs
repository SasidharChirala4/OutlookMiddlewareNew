using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.Model
{
    public class EmailRecipient  : ModelBase, ILongSysId
    {
        public string Recipient { get; set; }
        public EmailRecipientType Type { get; set; }
    }
}
