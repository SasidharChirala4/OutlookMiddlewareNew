using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.Mapping
{
    public class ProjectTaskDetailsDtoToProjectTaskMapper : IProjectTaskDetailsDtoToProjectTaskMapper
    {
        public ProjectTask Map(ProjectTaskDto projectTaskDetails, Email email)
        {

            ProjectTask projectTask = new ProjectTask();
            projectTask.TaskName = projectTaskDetails.TaskName;
            projectTask.Priority = projectTaskDetails.Priority;
            projectTask.DueDate = projectTaskDetails.DueDate;
            projectTask.EmailId = email.Id;
            projectTask.Description = projectTaskDetails.Description;
            projectTask.UserInvolvements = new List<ProjectTaskUserInvolvement>();
            foreach (ProjectTaskUserInvolmentDto userInvolvement in projectTaskDetails.UserInvolvements)
            {
                projectTask.UserInvolvements.Add(new ProjectTaskUserInvolvement()
                {
                    Type = userInvolvement.Type,
                    PrincipalName = userInvolvement.PrincipalName,
                    UserId = userInvolvement.UserId,
                });
            }
            return projectTask;
        }
    }
}
