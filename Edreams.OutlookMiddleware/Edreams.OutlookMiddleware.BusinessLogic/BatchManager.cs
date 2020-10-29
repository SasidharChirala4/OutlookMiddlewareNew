using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class BatchManager : IBatchManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<Email> _emailRepository;
        private readonly IRepository<File> _fileRepository;

        public BatchManager(
            IRepository<FilePreload> preloadedFilesRepository,
            IRepository<Batch> batchRepository,
            IRepository<Email> emailRepository,
            IRepository<File> fileRepository)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _batchRepository = batchRepository;
            _emailRepository = emailRepository;
            _fileRepository = fileRepository;
        }

        public async Task<CommitBatchResponse> CommitBatch(CommitBatchRequest request)
        {
            var preloadedFiles = await _preloadedFilesRepository.Find(
                    x => x.BatchId == request.BatchId);

            Batch batch = new Batch
            {
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "CREATED",
                Status = BatchStatus.Pending
            };

            batch = await _batchRepository.Create(batch);


            IList<File> files = new List<File>();
            var emailIds = preloadedFiles.Select(x => x.EmailId).Distinct();
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

            await _fileRepository.Create(files);

            foreach (var preloadedFile in preloadedFiles)
            {
                preloadedFile.Status = EmailPreloadStatus.Committed;
            }

            await _preloadedFilesRepository.Update(preloadedFiles);

            return new CommitBatchResponse();
        }

        public async Task<CancelBatchResponse> CancelBatch(CancelBatchRequest request)
        {
            var preloadedFiles = await _preloadedFilesRepository.Find(
                x => x.BatchId == request.BatchId);

            foreach (var preloadedFile in preloadedFiles)
            {
                preloadedFile.Status = EmailPreloadStatus.Cancelled;
            }

            await _preloadedFilesRepository.Update(preloadedFiles);

            return new CancelBatchResponse();
        }
    }
}