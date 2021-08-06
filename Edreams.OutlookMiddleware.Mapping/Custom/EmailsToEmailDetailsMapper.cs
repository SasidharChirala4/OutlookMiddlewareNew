using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping.Custom
{
    public class EmailsToEmailDetailsMapper : IEmailsToEmailDetailsMapper
    {
        public IList<EmailDetailsDto> Map(IList<Email> emails)
        {
            List<EmailDetailsDto> listOfEmailDetails = new List<EmailDetailsDto>(emails.Count);

            foreach (Email email in emails)
            {
                List<FileDetailsDto> listOfFileDetails = new List<FileDetailsDto>(email.Files.Count);

                foreach (File file in email.Files)
                {

                    FileDetailsDto fileDetails = new FileDetailsDto
                    {
                        Id = file.Id,
                        OriginalName = file.OriginalName,
                        Path = file.TempPath,
                        Kind = file.Kind,
                        EmailSubject = file.EmailSubject,
                        NewName = file.NewName,
                        ShouldUpload = file.ShouldUpload
                    };

                    foreach (Metadata metaData in file.Metadata)
                    {
                        fileDetails.Metadata.Add(new MetadataDto()
                        {
                            PropertyName = metaData.PropertyName,
                            PropertyValue = metaData.PropertyValue,
                            FileId = metaData.FileId
                        });
                    }

                    listOfFileDetails.Add(fileDetails);
                }

                EmailDetailsDto emailDetails = new EmailDetailsDto
                {
                    Id = email.Id,
                    Files = listOfFileDetails,
                    EdreamsReferenceId = email.EdreamsReferenceId,
                    EmailKind = email.EmailKind,
                    InternetMessageId = email.InternetMessageId,
                    Status = email.Status,
                    //initializing new EmailRecipientDto to avoid object reference not set to an instance of an object in the below step line no :50
                    EmailRecipients = new List<EmailRecipientDto>()

                };

                foreach (EmailRecipient emailRecipient in email.EmailRecipients)
                {
                    emailDetails.EmailRecipients.Add(new EmailRecipientDto()
                    {
                        EmailId = emailRecipient.Email.Id,
                        Recipient = emailRecipient.Recipient,
                        Type = emailRecipient.Type,
                        Kind = emailRecipient.Kind
                    });
                }

                if (email.ProjectTask != null)
                {
                    List<ProjectTaskUserInvolvementDto> userInvolvements = new List<ProjectTaskUserInvolvementDto>();
                    foreach (ProjectTaskUserInvolvement userInvolvement in email.ProjectTask.UserInvolvements)
                    {
                        userInvolvements.Add(
                        new ProjectTaskUserInvolvementDto()
                        {
                            PrincipalName = userInvolvement.PrincipalName,
                            Type = userInvolvement.Type,
                            UserId = userInvolvement.UserId
                        });
                    }
                    ProjectTaskDto projectTaskDto = new ProjectTaskDto()
                    {
                        Description = email.ProjectTask.Description,
                        DueDate = email.ProjectTask.DueDate,
                        Priority = email.ProjectTask.Priority,
                        TaskName = email.ProjectTask.TaskName,
                        UploadLocationProjectId= email.ProjectTask.UploadLocationProjectId,
                        UserInvolvements = userInvolvements
                    };
                    emailDetails.ProjectTaskDto = projectTaskDto;
                }

                listOfEmailDetails.Add(emailDetails);
            }

            return listOfEmailDetails;
        }
    }
}