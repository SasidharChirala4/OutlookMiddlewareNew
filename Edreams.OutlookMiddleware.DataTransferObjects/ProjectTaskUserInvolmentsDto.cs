using Edreams.OutlookMiddleware.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class ProjectTaskUserInvolmentsDto
    {
        public string UserId { get; set; }
        public string PrincipalName { get; set; }
        public ProjectTaskUserInvolvementType Type { get; set; }
    }
}
