using System;
using System.Collections.Generic;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// Project Task Dto
    /// </summary>
    public class ProjectTaskDto
    {
        public string TaskName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public Guid UploadLocationProjectId { get; set; }
        public List<ProjectTaskUserInvolvementDto> UserInvolvements { get; set; }
        public string EmailBody { get; set; }

    }
}