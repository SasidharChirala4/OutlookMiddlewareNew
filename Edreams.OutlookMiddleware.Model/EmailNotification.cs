using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;

namespace Edreams.OutlookMiddleware.Model
{
    public class EmailNotification : ModelBase, ILongSysId
    {
        public bool? NotificationSent { get; set; }
        public Batch Batch { get; set; }
    }
}
