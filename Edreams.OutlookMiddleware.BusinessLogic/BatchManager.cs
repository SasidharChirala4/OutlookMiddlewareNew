using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.Common.Exceptions;
using Edreams.Common.Exceptions.Constants;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class BatchManager : IBatchManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<Email> _emailRepository;
        private readonly IRepository<File> _fileRepository;
        private readonly IRepository<ProjectTask> _projectTaskRepository;
        private readonly IBatchFactory _batchFactory;
        private readonly IEmailsToEmailDetailsMapper _emailsToEmailDetailsMapper;
        private readonly IPreloadedFilesToFilesMapper _preloadedFilesToFilesMapper;
        private readonly IProjectTaskDetailsDtoToProjectTaskMapper _projectTaskDetailsDtoToProjectTaskMapper;
        private readonly ITransactionHelper _transactionHelper;
        private readonly IValidator _validator;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly ISecurityContext _securityContext;

        public BatchManager(
            IRepository<FilePreload> preloadedFilesRepository,
            IRepository<Batch> batchRepository,
            IRepository<Email> emailRepository,
            IRepository<File> fileRepository,
            IRepository<ProjectTask> projectTaskRepository,
            IBatchFactory batchFactory,
            IEmailsToEmailDetailsMapper emailsToEmailDetailsMapper,
            IPreloadedFilesToFilesMapper preloadedFilesToFilesMapper,
            IProjectTaskDetailsDtoToProjectTaskMapper projectTaskDetailsDtoToProjectTaskMapper,
            ITransactionHelper transactionHelper,
            IValidator validator,
            IExceptionFactory exceptionFactory,
            ISecurityContext securityContext)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _batchRepository = batchRepository;
            _emailRepository = emailRepository;
            _fileRepository = fileRepository;
            _batchFactory = batchFactory;
            _projectTaskRepository = projectTaskRepository;
            _emailsToEmailDetailsMapper = emailsToEmailDetailsMapper;
            _preloadedFilesToFilesMapper = preloadedFilesToFilesMapper;
            _transactionHelper = transactionHelper;
            _projectTaskDetailsDtoToProjectTaskMapper = projectTaskDetailsDtoToProjectTaskMapper;
            _validator = validator;
            _exceptionFactory = exceptionFactory;
            _securityContext = securityContext;
        }

        public async Task<BatchDetailsDto> GetBatchDetails(Guid batchId)
        {
            // Fetch the batch with specified unique ID.
            Batch batch = await _batchRepository.GetSingle(x => x.Id == batchId);

            // Throw an exception if a batch with specified unique ID cannot be found.
            if (batch == null)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsOutlookMiddlewareExceptionCode.OutlookMiddlewareBatchNotFound);
            }

            // Fetch all emails that are related to the specified batch and include the referenced files.
            IList<Email> emails = await _emailRepository.Find(x => x.Batch.Id == batchId, inc => inc.Files.Select(x => x.Metadata), incl => incl.EmailRecipients);

            // Map the database emails and files to email details and file details.
            IList<EmailDetailsDto> emailDetails = _emailsToEmailDetailsMapper.Map(emails);

            // Return the batch details that contains the email details and file details.
            return new BatchDetailsDto
            {
                Id = batchId,
                Emails = emailDetails.ToList(),
                UploadOption = batch.UploadOption,
                UploadLocationFolder = batch.UploadLocationFolder,
                UploadLocationSite = batch.UploadLocationSite,
                DeclareAsRecord = batch.DeclareAsRecord,
                VersionComment = batch.VersionComment
            };
        }

        public async Task UpdateBatchStatus(Guid batchId, BatchStatus status)
        {
            Batch batch = await _batchRepository.GetSingle(x => x.Id == batchId);

            if (batch == null)
            {
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsOutlookMiddlewareExceptionCode.OutlookMiddlewareBatchNotFound);
            }

            batch.Status = status;

            await _batchRepository.Update(batch);
        }

        public async Task<CommitBatchResponse> CommitBatch(Guid batchId, CommitBatchRequest request)
        {
            // Validate the batch identifier.
            _validator.Validate<EdreamsValidationException>(() => batchId == request.BatchId,
                "There is a 'BatchId' mismatch for route and request.");

            // Create response object contains information about the committed batch
            CommitBatchResponse response = new CommitBatchResponse();

            // Find all the file records that have been preloaded for the specified batch.
            var preloadedFiles = await _preloadedFilesRepository.Find(
            x => x.BatchId == request.BatchId);

            // Force a database transaction scope to make sure multiple
            // operations are combined as a single atomic operation.
            using (ITransactionScope transactionScope = _transactionHelper.CreateScope())
            {
                // If there were no file records found for the specified batch, that batch
                // is not found and 'null' should be returned to force an HTTP 404.
                if (preloadedFiles.Count == 0)
                {
                    return null;
                }

                // Build a new batch and create it in the database.
                Batch batch = _batchFactory.CreatePendingBatch();
                batch.UploadOption = request.UploadOption;
                batch.VersionComment = request.VersionComment;
                batch.DeclareAsRecord = request.DeclareAsRecord;
                batch.UploadLocationSite = request.UploadLocationSite;
                batch.UploadLocationFolder = request.UploadLocationFolder;
                batch = await _batchRepository.Create(batch);

                // Map the preloaded files to a list of files with relation to email and batch.
                // Afterwards, create the files in the database. EF will automatically create
                // the email references with that.
                IList<File> files = _preloadedFilesToFilesMapper.Map(batch, preloadedFiles, request);
                await _fileRepository.Create(files);

                if (request.ProjectTaskDetails != null)
                {
                    IList<Email> emails = await _emailRepository.Find(x => x.Batch.Id == batch.Id);
                    List<ProjectTask> taskDetails = new List<ProjectTask>();
                    foreach (Email email in emails)
                    {
                        ProjectTask task = _projectTaskDetailsDtoToProjectTaskMapper.Map(request.ProjectTaskDetails, email);
                        taskDetails.Add(task);
                    }

                    await _projectTaskRepository.Create(taskDetails);
                }


                // All file records for the specified batch should be marked for cleanup
                // by setting their status to 'Committed'.
                foreach (var preloadedFile in preloadedFiles)
                {
                    preloadedFile.Status = EmailPreloadStatus.Committed;
                }

                transactionScope.Commit();

                // Fill response object.
                response.CorrelationId = _securityContext.CorrelationId;
                response.BatchId = batch.Id;
                response.NumberOfEmails = files.Select(x => x.Email).Distinct().Count();
                response.NumberOfFiles = preloadedFiles.Count;
            }

            // Update all file records in the pre-load database.
            await _preloadedFilesRepository.Update(preloadedFiles);

            return response;
        }

        public async Task<CancelBatchResponse> CancelBatch(Guid batchId, CancelBatchRequest request)
        {
            // Validate the batch identifier.
            _validator.Validate<EdreamsValidationException>(() => batchId == request.BatchId,
                "There is a 'BatchId' mismatch for route and request.");

            // Find all the file records that have been preloaded for the specified batch.
            var preloadedFiles = await _preloadedFilesRepository.Find(
                x => x.BatchId == request.BatchId);

            // If there were no file records found for the specified batch, that batch
            // is not found and 'null' should be returned to force an HTTP 404.
            if (preloadedFiles.Count == 0)
            {
                return null;
            }

            // All file records for the specified batch should be marked for cleanup
            // by setting their status to 'Cancelled'.
            foreach (var preloadedFile in preloadedFiles)
            {
                preloadedFile.Status = EmailPreloadStatus.Cancelled;
            }

            // Update all file records in the pre-load database.
            await _preloadedFilesRepository.Update(preloadedFiles);

            // Return a response containing some information about the cancelled batch.
            return new CancelBatchResponse
            {
                CorrelationId = _securityContext.CorrelationId,
                BatchId = request.BatchId,
                NumberOfCancelledFiles = preloadedFiles.Count,
            };
        }
    }
}