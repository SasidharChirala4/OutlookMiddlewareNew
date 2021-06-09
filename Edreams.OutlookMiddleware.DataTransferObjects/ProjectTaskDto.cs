using System;
using System.Collections.Generic;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class ProjectTaskDto
    {
        public string TaskName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public List<ProjectTaskUserInvolvementDto> UserInvolvements { get; set; }

    }
}