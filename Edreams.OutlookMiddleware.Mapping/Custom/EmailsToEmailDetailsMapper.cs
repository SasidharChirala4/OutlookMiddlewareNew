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
                        Name = file.FileName,
                        Path = file.TempPath,
                        Kind = file.Kind,
                        EmailSubject = file.EmailSubject,
                    };

                    listOfFileDetails.Add(fileDetails);
                }
                
                EmailDetailsDto emailDetails = new EmailDetailsDto
                {
                    Id = email.Id,
                    Files = listOfFileDetails,
                    UploadOption = email.UploadOption,
                    EdreamsReferenceId = email.EdreamsReferenceId,
                    EmailKind = email.EmailKind,
                    
                };

                foreach(EmailRecipient emailRecipient in email.EmailRecipients)
                {
                    emailDetails.EmailRecipients.Add(new EmailRecipientDto()
                    {
                        EmailId= emailRecipient.Email.Id ,
                        Recipient =emailRecipient.Recipient,
                        Type= emailRecipient.Type
                    });
                }

                if (email.ProjectTask != null)
                {
                    List<ProjectTaskUserInvolmentDto> userInvolvements = new List<ProjectTaskUserInvolmentDto>();
                    foreach (ProjectTaskUserInvolvement userInvlovement in email.ProjectTask.UserInvolvements)
                    {
                        userInvolvements.Add(
                        new ProjectTaskUserInvolmentDto()
                        {
                            PrincipalName = userInvlovement.PrincipalName,
                            Type = userInvlovement.Type,
                            UserId = userInvlovement.UserId
                        });
                    }
                    ProjectTaskDto projectTaskDto = new ProjectTaskDto()
                    {
                        Description = email.ProjectTask.Description,
                        DueDate = email.ProjectTask.DueDate,
                        Priority = email.ProjectTask.Priority,
                        TaskName = email.ProjectTask.TaskName,
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