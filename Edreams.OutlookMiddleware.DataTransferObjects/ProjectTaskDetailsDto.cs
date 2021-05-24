using Edreams.OutlookMiddleware.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class ProjectTaskDetailsDto
    {
        public string TaskName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public  List<ProjectTaskUserInvolmentsDto> UserInvolvements { get; set; }
       
    }
}
