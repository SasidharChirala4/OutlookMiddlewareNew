using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class BatchManager : IBatchManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<Email> _emailRepository;
        private readonly IRepository<File> _fileRepository;
        private readonly IMapper<EmailRecipientDto, EmailRecipient> _emailRecipientDtoToEmailRecipientMapper;
        private readonly IRepository<EmailRecipient> _emailRecipientRepository;
        private readonly IBatchFactory _batchFactory;
        private readonly IEmailsToEmailDetailsMapper _emailsToEmailDetailsMapper;
        private readonly IPreloadedFilesToFilesMapper _preloadedFilesToFilesMapper;
        private readonly ITransactionHelper _transactionHelper;
        private readonly IExceptionFactory _exceptionFactory;

        public BatchManager(
            IRepository<FilePreload> preloadedFilesRepository,
            IRepository<Batch> batchRepository,
            IRepository<Email> emailRepository,
            IRepository<File> fileRepository,
            IRepository<EmailRecipient> emailRecipientRepository,
            IBatchFactory batchFactory,
            IEmailsToEmailDetailsMapper emailsToEmailDetailsMapper,
            IPreloadedFilesToFilesMapper preloadedFilesToFilesMapper,
            IMapper<EmailRecipientDto, EmailRecipient> emailRecipientDtoToEmailRecipientMapper,
            ITransactionHelper transactionHelper,
            IExceptionFactory exceptionFactory)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _batchRepository = batchRepository;
            _emailRepository = emailRepository;
            _fileRepository = fileRepository;
            _emailRecipientRepository = emailRecipientRepository;
            _batchFactory = batchFactory;
            _emailsToEmailDetailsMapper = emailsToEmailDetailsMapper;
            _preloadedFilesToFilesMapper = preloadedFilesToFilesMapper;
            _emailRecipientDtoToEmailRecipientMapper = emailRecipientDtoToEmailRecipientMapper;
            _transactionHelper = transactionHelper;
            _exceptionFactory = exceptionFactory;
        }

        public async Task<BatchDetailsDto> GetBatchDetails(Guid batchId)
        {
            // Fetch the batch with specified unique ID.
            Batch batch = await _batchRepository.GetSingle(x => x.Id == batchId);

            // Throw an exception if a batch with specified unique ID cannot be found.
            if (batch == null)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.OUTLOOKMIDDLEWARE_BATCH_NOT_FOUND);
            }

            // Fetch all emails that are related to the specified batch and include the referenced files.
            // TODO: Need to Remove Upload Option/adjust logic
            IList<Email> emails = await _emailRepository.Find(x => x.Batch.Id == batchId, inc => inc.Files);

            // Map the database emails and files to email details and file details.
            IList<EmailDetailsDto> emailDetails = _emailsToEmailDetailsMapper.Map(emails);

            // Return the batch details that contains the email details and file details.
            return new BatchDetailsDto
            {
                Id = batchId,
                Emails = emailDetails.ToList()
            };
        }

        public async Task UpdateBatchStatus(Guid batchId, BatchStatus status)
        {
            Batch batch = await _batchRepository.GetSingle(x => x.Id == batchId);

            if (batch == null)
            {
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.OUTLOOKMIDDLEWARE_BATCH_NOT_FOUND);
            }

            batch.Status = status;

            await _batchRepository.Update(batch);
        }

        public async Task<CommitBatchResponse> CommitBatch(CommitBatchRequest request)
        {
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
                batch = await _batchRepository.Create(batch);

                // Map the preloaded files to a list of files with relation to email and batch.
                // Afterwards, create the files in the database. EF will automatically create
                // the email references with that.
                IList<File> files = _preloadedFilesToFilesMapper.Map(batch, preloadedFiles, request.UploadOption);
                await _fileRepository.Create(files);

                // Add email recipients for the current batch.
                if (request.EmailRecipients != null && request.EmailRecipients.Any())
                {
                    // Find all the email records for the specified batch.
                    IEnumerable<Email> batchEmails = await _emailRepository.Find(
                        x => x.Batch.Id == request.BatchId);

                    // Creates Email Recipients for each email of batch
                    foreach (Email batchemail in batchEmails)
                    {
                        // Get the requested email recipients for the specified batch email.
                        var emailRecipientsDto = request.EmailRecipients.Where(x => x.EmailId == batchemail.Id).ToList();

                        // Map the list of EmailRecipientDto to list of EmailRecipients.
                        var emailRecipients = _emailRecipientDtoToEmailRecipientMapper.Map(emailRecipientsDto);

                        //ToDo: will the requested recipients contain emailid.
                        if (emailRecipients.Any())
                        {
                            // Creates Email Recipients for each batch email  
                            await _emailRecipientRepository.Create(emailRecipients);
                        }
                    }
                }

                // All file records for the specified batch should be marked for cleanup
                // by setting their status to 'Committed'.
                foreach (var preloadedFile in preloadedFiles)
                {
                    preloadedFile.Status = EmailPreloadStatus.Committed;
                }

                transactionScope.Commit();

                // Fill response object.
                response.CorrelationId = request.CorrelationId;
                response.BatchId = batch.Id;
                response.NumberOfEmails = files.Select(x => x.Email).Distinct().Count();
                response.NumberOfFiles = preloadedFiles.Count;
            }
            // Update all file records in the pre-load database.
            await _preloadedFilesRepository.Update(preloadedFiles);

            return response;
        }

        public async Task<CancelBatchResponse> CancelBatch(CancelBatchRequest request)
        {
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
                CorrelationId = request.CorrelationId,
                BatchId = request.BatchId,
                NumberOfCancelledFiles = preloadedFiles.Count
            };
        }
    }
}