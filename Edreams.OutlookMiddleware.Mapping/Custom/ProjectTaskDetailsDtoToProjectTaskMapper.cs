using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping
{
    public class ProjectTaskDetailsDtoToProjectTaskMapper : IProjectTaskDetailsDtoToProjectTaskMapper
    {
        public ProjectTask Map(ProjectTaskDto projectTaskDetails, Email email)
        {
            ProjectTask projectTask = new ProjectTask
            {
                TaskName = projectTaskDetails.TaskName,
                Priority = projectTaskDetails.Priority,
                DueDate = projectTaskDetails.DueDate,
                EmailId = email.Id,
                EmailBody = projectTaskDetails.EmailBody,
                Description = projectTaskDetails.Description,
                UploadLocationProjectId = projectTaskDetails.UploadLocationProjectId,
                UserInvolvements = new List<ProjectTaskUserInvolvement>()
            };

            if (projectTaskDetails.UserInvolvements != null)
            {
                foreach (ProjectTaskUserInvolvementDto userInvolvement in projectTaskDetails.UserInvolvements)
                {
                    projectTask.UserInvolvements.Add(new ProjectTaskUserInvolvement()
                    {
                        Type = userInvolvement.Type,
                        PrincipalName = userInvolvement.PrincipalName,
                        UserId = userInvolvement.UserId
                    });

                }
            }

            return projectTask;
        }
    }
}