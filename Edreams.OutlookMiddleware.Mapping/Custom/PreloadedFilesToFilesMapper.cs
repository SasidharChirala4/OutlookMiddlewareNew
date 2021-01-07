using System;
using System.Collections.Generic;
using System.Linq;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.Mapping.Custom
{
    public class PreloadedFilesToFilesMapper : IPreloadedFilesToFilesMapper
    {
        public IList<File> Map(Batch batch, IList<FilePreload> preloadedFiles)
        {
            IList<File> files = new List<File>();
            Guid[] emailIds = preloadedFiles.Select(x => x.EmailId).Distinct().ToArray();
            foreach (Guid emailId in emailIds)
            {
                Email email = new Email
                {
                    Batch = batch,
                    Status = EmailStatus.ReadyToUpload
                };

                foreach (var preloadedFile in preloadedFiles)
                {
                    if (preloadedFile.EmailId == emailId)
                    {
                        email.EwsId = preloadedFile.EwsId;
                        email.EntryId = preloadedFile.EntryId;

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