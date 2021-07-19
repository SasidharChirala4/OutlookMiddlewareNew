using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;
using System;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.Mapping
{
    public class ProjectTaskDetailsDtoToProjectTaskMapper : IProjectTaskDetailsDtoToProjectTaskMapper
    {
        public ProjectTask Map(ProjectTaskDto projectTaskDetails, Email email, Guid uploadLocationProjectId)
        {

            ProjectTask projectTask = new ProjectTask
            {
                TaskName = projectTaskDetails.TaskName,
                Priority = projectTaskDetails.Priority,
                DueDate = projectTaskDetails.DueDate,
                EmailId = email.Id,
                Description = projectTaskDetails.Description,
                UploadLocationProjectId = uploadLocationProjectId,
                UserInvolvements = new List<ProjectTaskUserInvolvement>()
            };
            foreach (ProjectTaskUserInvolvementDto userInvolvement in projectTaskDetails.UserInvolvements)
            {
                projectTask.UserInvolvements.Add(new ProjectTaskUserInvolvement()
                {
                    Type = userInvolvement.Type,
                    PrincipalName = userInvolvement.PrincipalName,
                    UserId = userInvolvement.UserId,
                    // ToDo: Need to remove once the johnny configured InsertedBy fileds for child tables in repository.
                    InsertedBy = "BE\\kkaredla"
                });

            }
            return projectTask;
        }
    }
}
