using Edreams.Contracts.Data.Common;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        /// <param name="sharepointFileUploads">fie upload details.</param>
        /// <param name="siteUrl">site url</param>
        /// <returns>Project Task object to create</returns>
        public ProjectTask GetEdreamsProjectTask(EmailDetailsDto emailDetails, List<SharePointFile> sharepointFileUploads, string siteUrl)
        {
            ProjectTaskDto projectTask = emailDetails.ProjectTaskDto;
            List<MetadataDto> emailMetadata = emailDetails.Files.First(x => x.Kind == Enums.FileKind.Email).Metadata;
            //ToRecipient
            EmailRecipientDto toRecipient = emailDetails.EmailRecipients.FirstOrDefault(x => x.Kind == Enums.EmailRecipientKind.From);

            //Get UserInvolvements
            var userInvolvements = GetUserInvolvements(projectTask.UserInvolvements);
            ProjectTaskUserInvolvement assignedBy = userInvolvements.Where(x => x.InvolvementType == ProjectTaskUserInvolvementType.AssignedBy).FirstOrDefault();

            string cc = string.Empty;
            if (emailMetadata.Any())
            {
                MetadataDto mailCcMetadata = emailMetadata.SingleOrDefault(t =>
                    t.PropertyName.Equals(Constants.EdrMailCc, StringComparison.OrdinalIgnoreCase));
                cc = mailCcMetadata != null ? mailCcMetadata.PropertyValue : string.Empty;
            }

            // Create ProjectTaskMail Object
            var mailObject = new ProjectTaskMail
            {
                FromUserPrincipalName = assignedBy.UserPrincipalName,
                FromUserId = assignedBy.UserId,
                Cc = cc,
                To = toRecipient?.Recipient,
                Body = BuildMailBody(emailMetadata, projectTask.EmailBody),
                // Email Subject is stored in file entity.
                // Each file related to the email having same subject so taking subject from first object.
                Subject = FormatSubject(emailDetails.Files[0].EmailSubject),
            };

            //Returns ProjectTask object to Create.
            return new ProjectTask()
            {
                ProjectId = projectTask.UploadLocationProjectId,
                ProjectUrl = siteUrl,
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

        private List<ProjectTaskUserInvolvement> GetUserInvolvements(List<ProjectTaskUserInvolvementDto> userInvolmentDtos)
        {
            List<ProjectTaskUserInvolvement> userInvolvements = new List<ProjectTaskUserInvolvement>();
            foreach (ProjectTaskUserInvolvementDto userInvolmentDto in userInvolmentDtos)
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
            if (!String.IsNullOrEmpty(subject) && subject.IndexOf(_configuration.SubjectResponse, StringComparison.OrdinalIgnoreCase) != 0)
            {
                result = _configuration.SubjectResponse + result;
            }
            return result;
        }

        /// <summary>
        /// Method to build the Mail body with the header added
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="emailBody"></param>
        /// <returns></returns>
        public string BuildMailBody(List<MetadataDto> metadata, string emailBody)
        {
            string mailHeaderRow = string.Empty;
            string mailHeaderTemplateRow = Constants.MailHeaderTemplateRow;
            string mailHeaderTemplatePara = Constants.MailHeaderTemplatePara;

            //Adding the From Address            
            mailHeaderRow = string.Format(mailHeaderTemplateRow, Constants.MailFrom,
                            metadata.SingleOrDefault(t => t.PropertyName.Equals(Constants.EdrMailFrom, StringComparison.OrdinalIgnoreCase)).PropertyValue);

            //Adding the Sent on details
            //Converting the MailReceived date to format - For example: Monday, 9 September, 2019 14:13
            string mailReceivedDate = metadata.SingleOrDefault(t => t.PropertyName.Equals(Constants.EdrMailSent, StringComparison.OrdinalIgnoreCase)).PropertyValue;
            var dateValue = DateTime.ParseExact(mailReceivedDate, Constants.EdrDateTimeFormat, CultureInfo.InvariantCulture);
            mailHeaderRow += string.Format(mailHeaderTemplateRow, Constants.MailSent, dateValue.ToString(Constants.MailHeaderSendFormat));

            //Adding the To Address
            string toAddresses = string.Empty;
            var property = metadata.SingleOrDefault(t =>
                      t.PropertyName.Equals(Constants.EdrMailTo, StringComparison.OrdinalIgnoreCase));
            toAddresses = property.PropertyValue.Replace(',', ';');
            mailHeaderRow += string.Format(mailHeaderTemplateRow, Constants.MailTo, toAddresses);

            //Adding the Cc Address            
            string ccAddresses = string.Empty;
            MetadataDto mailCcMetadata = metadata.SingleOrDefault(t =>
                    t.PropertyName.Equals(Constants.EdrMailCc, StringComparison.OrdinalIgnoreCase));
            ccAddresses = mailCcMetadata != null ? mailCcMetadata.PropertyValue.Replace(',', ';') : string.Empty;
            mailHeaderRow += string.Format(mailHeaderTemplateRow, Constants.MailCc, ccAddresses);

            //Adding the Subject
            string subject = metadata.SingleOrDefault(t =>
                     t.PropertyName.Equals(Constants.EdrMailSubject, StringComparison.OrdinalIgnoreCase)).PropertyValue;
            mailHeaderRow += string.Format(mailHeaderTemplateRow, Constants.MailSubject, subject);

            //Returning the combined string having the Header of the mail and the actual mail body
            return (string.Format(mailHeaderTemplatePara, mailHeaderRow) + emailBody);
        }

        #endregion
    }
}
