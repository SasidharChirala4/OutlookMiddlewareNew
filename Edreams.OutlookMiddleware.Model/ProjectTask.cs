using System;
using System.Collections.Generic;
using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class ProjectTask : ModelBase, ILongSysId
    {
        public string TaskName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public Guid EmailId { get; set; }
        public virtual Email Email { get; set; }
        public virtual IList<ProjectTaskUserInvolvement> UserInvolvements { get; set; }
        public Guid UploadLocationProjectId { get; set; }
        public string EmailBody { get; set; }
    }
}