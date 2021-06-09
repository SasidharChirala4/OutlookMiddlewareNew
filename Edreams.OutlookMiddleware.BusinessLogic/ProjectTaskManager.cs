using Edreams.Contracts.Data.Common;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ProjectTask = Edreams.Contracts.Data.Extensibility.ProjectTask;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ProjectTaskManager : IProjectTaskManager
    {
        private readonly IEdreamsConfiguration _configuration;

        public ProjectTaskManager(IEdreamsConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Public Methods
        /// <summary>
        /// Method to get the edreams project task object.
        /// </summary>
        /// <param name="emailDetails">Email object</param>
        /// <param name="sharepointFileUploads">file upload details.</param>
        /// <returns>Created ProjectTask Object</returns>
        public ProjectTask GetEdreamsProjectTask(EmailDetailsDto emailDetails, List<SharePointFile> sharepointFileUploads)
        {
            ProjectTaskDto projectTask = emailDetails.ProjectTaskDto;

            //TO DO: Need to check with johnny about Email recipients
            //ToRecipient 
            //EmailRecipientDto toRecipient = emailDetails.EmailRecipients.FirstOrDefault(x => x.Type.Equals("FROM"));

            //Get UserInvolvements
            var userInvolvements = GetUserInvolvements(projectTask.UserInvolvements);
            ProjectTaskUserInvolvement assignedBy = userInvolvements.Where(x => x.InvolvementType == ProjectTaskUserInvolvementType.AssignedBy).FirstOrDefault();
            
            // Create ProjectTaskMail Object
            var mailObject = new ProjectTaskMail
            {
                FromUserPrincipalName = assignedBy.UserPrincipalName,
                FromUserId = assignedBy.UserId,
                // TODO: To,cc,Body details are filled once the metadata details are addded.
                //To = toRecipient.Recipient,
                //Cc = cc,
                //Body = (!string.IsNullOrEmpty(spModel.EmailBody) ? spModel.EmailBody : string.Empty)
                // Email Subject is stored in file entity.
                // Each file related to the email having same subject so taking subject from first object.
                Subject = FormatSubject(emailDetails.Files[0].EmailSubject),
            };

            //Return ProjectTask object to Create.
            return new ProjectTask()
            {
                // TODO: ProjectId,ProjectUrl details are filled once the metadata details are addded.
                //ProjectId = new Guid(projectTask.ProjectId),
                //ProjectUrl = projectTask.SiteUrl,
                Title = projectTask.TaskName,
                Description = projectTask.Description,
                Priority = Enum.Parse<ProjectTaskPriority>(projectTask.Priority.ToString(), true),
                StartDate = DateTime.UtcNow,
                DueDate = projectTask.DueDate.HasValue ? projectTask.DueDate.Value : (DateTime?)null,
                UserInvolvements = userInvolvements,
                Documents = sharepointFileUploads.Select(x => new ProjectTaskDocument()
                {
                    ItemUniqueId = x.Id,
                    DocumentType = ProjectTaskDocumentType.IncomingDocument,
                    FileAbsoluteUrl = x.AbsoluteUrl
                }).ToList(),
                Mail = mailObject
            };
        }
        #endregion

        #region Private Methods
        private List<ProjectTaskUserInvolvement> GetUserInvolvements(List<ProjectTaskUserInvolmentDto> userInvolmentDtos)
        {
            List<ProjectTaskUserInvolvement> userInvolvements = new List<ProjectTaskUserInvolvement>();
            foreach (ProjectTaskUserInvolmentDto userInvolmentDto in userInvolmentDtos)
            {
                userInvolvements.Add(new ProjectTaskUserInvolvement()
                {
                    InvolvementType = Enum.Parse<ProjectTaskUserInvolvementType>(userInvolmentDto.Type.ToString(), true),
                    UserPrincipalName = userInvolmentDto.PrincipalName,
                    UserId = new Guid(userInvolmentDto.UserId)
                });
            }
            return userInvolvements;
        }
        private string FormatSubject(string subject)
        {
            string result = subject;
            if (!String.IsNullOrEmpty(subject) && subject.IndexOf(_configuration.SubjectResponse)<0)
            {
                result = _configuration.SubjectResponse.ToString(CultureInfo.InvariantCulture) + result;
            }
            return result;
        }
        #endregion
    }
}
