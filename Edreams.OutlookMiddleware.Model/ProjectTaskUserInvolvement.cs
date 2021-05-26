using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class ProjectTaskUserInvolvement : ModelBase, ILongSysId
    {
        public string UserId { get; set; }
        public string PrincipalName { get; set; }
        public ProjectTaskUserInvolvementType Type { get; set; }
        public virtual ProjectTask ProjectTask { get; set; }
    }
}