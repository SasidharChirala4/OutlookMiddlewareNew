using System;
using System.Collections.Generic;
using System.Linq;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping.Custom
{
    public class PreloadedFilesToFilesMapper : IPreloadedFilesToFilesMapper
    {
        public IList<File> Map(Batch batch, IList<FilePreload> preloadedFiles, EmailUploadOptions uploadOption, List<EmailRecipientDto> emailRecipients)
        {
            IList<File> files = new List<File>();
            Guid[] emailIds = preloadedFiles.Select(x => x.EmailId).Distinct().ToArray();
            foreach (Guid emailId in emailIds)
            { 
                Email email = new Email
                {
                    Batch = batch,
                    Status = EmailStatus.ReadyToUpload,
                    UploadOption = uploadOption,
                };

                foreach (var preloadedFile in preloadedFiles)
                {
                    if (preloadedFile.EmailId == emailId)
                    {
                        email.EwsId = preloadedFile.EwsId;
                        email.EntryId = preloadedFile.EntryId;
                        email.InternetMessageId = preloadedFile.InternetMessageId;
                        email.EmailRecipients = new List<EmailRecipient>();
                        IEnumerable<EmailRecipientDto> emailRecipientList = emailRecipients.Where(x => x.EmailId == emailId);
                        foreach(EmailRecipientDto emailRecipient in emailRecipientList)
                        {
                            EmailRecipient newEmailRecipient = new EmailRecipient();
                            newEmailRecipient.Email = email;
                            newEmailRecipient.Recipient = emailRecipient.Recipient;
                            newEmailRecipient.Type = emailRecipient.Type;
                            // ToDo: Need to remove and configure in repository.
                            newEmailRecipient.InsertedBy = "BE\\kkaredla";
                            email.EmailRecipients.Add(newEmailRecipient);
                        }
                        files.Add(new File
                        {
                            Email = email,
                            EmailSubject = preloadedFile.EmailSubject,
                            AttachmentId = preloadedFile.AttachmentId,
                            FileName = preloadedFile.FileName,
                            Size = preloadedFile.Size,
                            TempPath = preloadedFile.TempPath,
                            Kind = preloadedFile.Kind,
                            Status = FileStatus.ReadyToUpload
                        });
                    }
                }
            }

            return files;
        }
    }
}