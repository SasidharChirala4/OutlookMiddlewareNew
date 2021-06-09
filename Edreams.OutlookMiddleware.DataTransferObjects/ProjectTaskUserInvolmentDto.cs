using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class ProjectTaskUserInvolvementDto
    {
        public string UserId { get; set; }
        public string PrincipalName { get; set; }
        public ProjectTaskUserInvolvementType Type { get; set; }
    }
}