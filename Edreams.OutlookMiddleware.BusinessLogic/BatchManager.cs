using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class BatchManager : IBatchManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<File> _fileRepository;
        private readonly IBatchFactory _batchFactory;
        private readonly IPreloadedFilesToFilesMapper _preloadedFilesToFilesMapper;
        private readonly ITransactionHelper _transactionHelper;

        public BatchManager(
            IRepository<FilePreload> preloadedFilesRepository,
            IRepository<Batch> batchRepository,
            IRepository<File> fileRepository,
            IBatchFactory batchFactory,
            IPreloadedFilesToFilesMapper preloadedFilesToFilesMapper,
            ITransactionHelper transactionHelper)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _batchRepository = batchRepository;
            _fileRepository = fileRepository;
            _batchFactory = batchFactory;
            _preloadedFilesToFilesMapper = preloadedFilesToFilesMapper;
            _transactionHelper = transactionHelper;
        }

        public async Task<CommitBatchResponse> CommitBatch(CommitBatchRequest request)
        {
            // Force a database transaction scope to make sure multiple
            // operations are combined as a single atomic operation.
            using (ITransactionScope transactionScope = _transactionHelper.CreateScope())
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

                // Build a new batch and create it in the database.
                Batch batch = _batchFactory.CreatePendingBatch();
                batch = await _batchRepository.Create(batch);

                // Map the preloaded files to a list of files with relation to email and batch.
                // Afterwards, create the files in the database. EF will automatically create
                // the email references with that.
                IList<File> files = _preloadedFilesToFilesMapper.Map(batch, preloadedFiles);
                await _fileRepository.Create(files);

                // All file records for the specified batch should be marked for cleanup
                // by setting their status to 'Committed'.
                foreach (var preloadedFile in preloadedFiles)
                {
                    preloadedFile.Status = EmailPreloadStatus.Committed;
                }

                // Update all file records in the pre-load database.
                await _preloadedFilesRepository.Update(preloadedFiles);

                transactionScope.Commit();

                // Return a response containing some information about the committed batch.
                return new CommitBatchResponse
                {
                    CorrelationId = request.CorrelationId,
                    BatchId = batch.Id,
                    NumberOfEmails = files.Select(x => x.Email).Distinct().Count(),
                    NumberOfFiles = preloadedFiles.Count
                };
            }
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