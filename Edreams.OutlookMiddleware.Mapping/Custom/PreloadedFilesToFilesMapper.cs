using System;
using System.Collections.Generic;
using System.Linq;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping.Custom
{
    public class PreloadedFilesToFilesMapper : IPreloadedFilesToFilesMapper
    {
        public IList<File> Map(Batch batch, IList<FilePreload> preloadedFiles, CommitBatchRequest request)
        {
            IList<File> files = new List<File>();
            Guid[] emailIds = preloadedFiles.Select(x => x.EmailId).Distinct().ToArray();
            foreach (Guid emailId in emailIds)
            {
                Email email = new Email
                {
                    Batch = batch,
                    Status = EmailStatus.ReadyToUpload,
                };

                foreach (var preloadedFile in preloadedFiles)
                {
                    if (preloadedFile.EmailId == emailId)
                    {
                        email.EwsId = preloadedFile.EwsId;
                        email.EntryId = preloadedFile.EntryId;
                        email.EmailKind = preloadedFile.EmailKind;
                        email.EdreamsReferenceId = preloadedFile.EdreamsReferenceId;
                        email.InternetMessageId = preloadedFile.InternetMessageId;
                        email.EmailRecipients = new List<EmailRecipient>();
                        if (request.EmailRecipients != null)
                        {
                            IEnumerable<EmailRecipientDto> emailRecipientList = request.EmailRecipients.Where(x => x.EmailId == emailId);
                            foreach (EmailRecipientDto emailRecipient in emailRecipientList)
                            {
                                email.EmailRecipients.Add(new EmailRecipient()
                                {
                                    Email = email,
                                    Recipient = emailRecipient.Recipient,
                                    Type = emailRecipient.Type,
                                    Kind = emailRecipient.Kind
                                });
                            }
                        }

                        File file = new File
                        {
                            Email = email,
                            EmailSubject = preloadedFile.EmailSubject,
                            AttachmentId = preloadedFile.AttachmentId,
                            OriginalName = preloadedFile.FileName,
                            Size = preloadedFile.Size,
                            TempPath = preloadedFile.TempPath,
                            Kind = preloadedFile.Kind,
                            Status = FileStatus.ReadyToUpload
                        };
                        FileDetailsDto fileDetails = request.Files.FirstOrDefault(x => x.Id == preloadedFile.Id);
                        if (fileDetails != null)
                        {
                            file.NewName = fileDetails.NewName;
                            file.ShouldUpload = fileDetails.ShouldUpload;

                            foreach (MetadataDto metadataDto in fileDetails.Metadata)
                            {
                                file.Metadata.Add(new Metadata()
                                {
                                    PropertyName = metadataDto.PropertyName,
                                    PropertyValue = metadataDto.PropertyValue
                                });
                            }
                        }

                        files.Add(file);
                    }
                }
            }

            return files;
        }
    }
}