using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Common.DataAccess.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class CleanupManager : ICleanupManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IRepository<HistoricTransaction> _transactionHistoryRepository;
        private readonly IRepository<CategorizationRequest> _categorizationRequestRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<Email> _emailRepository;
        private readonly IRepository<EmailRecipient> _emailRecipientRepository;
        private readonly IRepository<File> _fileRepository;
        private readonly IRepository<Metadata> _metadataRepository;
        private readonly IFileHelper _fileHelper;
        private readonly ITransactionHelper _transactionHelper;
        private readonly IEdreamsConfiguration _configuration;

        public CleanupManager(
            IRepository<FilePreload> preloadedFilesRepository,
            IRepository<HistoricTransaction> transactionHistoryRepository,
            IRepository<CategorizationRequest> categorizationRequestRepository,
            IRepository<Batch> batchRepository,
            IRepository<Email> emailRepository,
            IRepository<EmailRecipient> emailRecipientRepository,
            IRepository<File> fileRepository,
            IRepository<Metadata> metadataRepository,
            IFileHelper fileHelper,
            ITransactionHelper transactionHelper,
            IEdreamsConfiguration configuration)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _transactionHistoryRepository = transactionHistoryRepository;
            _categorizationRequestRepository = categorizationRequestRepository;
            _batchRepository = batchRepository;
            _emailRepository = emailRepository;
            _emailRecipientRepository = emailRecipientRepository;
            _fileRepository = fileRepository;
            _fileHelper = fileHelper;
            _transactionHelper = transactionHelper;
            _configuration = configuration;
            _metadataRepository = metadataRepository;
        }

        public async Task<int> ExpirePreloadedFiles()
        {
            // Read the time in minutes for expiry from configuration and calculate the datetime offset.
            int expiry = _configuration.PreloadedFilesExpiryInMinutes;
            DateTime expirationDateTime = DateTime.UtcNow.AddMinutes(-expiry);

            // Search for pending preloaded files that are older than the expiry offset.
            IList<FilePreload> preloadedFiles = await _preloadedFilesRepository.Find(
                x => x.Status == EmailPreloadStatus.Pending && x.PreloadedOn < expirationDateTime);

            // Change the status for all eligible preloaded files to expired.
            foreach (FilePreload file in preloadedFiles)
            {
                file.Status = EmailPreloadStatus.Expired;
            }

            // Update all expired preloaded files in the database.
            await _preloadedFilesRepository.Update(preloadedFiles);

            // Return the number of preloaded files that have been marked as expired.
            return preloadedFiles.Count;
        }

        public async Task<int> ExpireTransactions()
        {
            // Read the time in minutes for expiry from configuration and calculate the datetime offset.
            int expiry = _configuration.TransactionHistoryExpiryInMinutes;
            DateTime expirationDateTime = DateTime.UtcNow.AddMinutes(-expiry);

            // Search for succeeded transactions that are older than the expiry offset.
            IList<HistoricTransaction> transactionHistory = await _transactionHistoryRepository.Find(
                x => x.Status == TransactionStatus.ProcessingSucceeded && x.ProcessingFinished.HasValue &&
                     x.ProcessingFinished < expirationDateTime);

            // Change the status for all eligible transactions to expired.
            foreach (HistoricTransaction file in transactionHistory)
            {
                file.Status = TransactionStatus.Expired;
            }

            // Update all expired transactions in the database.
            await _transactionHistoryRepository.Update(transactionHistory);

            // Return the number of transactions that have been marked as expired.
            return transactionHistory.Count;
        }

        public async Task<int> CleanupPreloadedFiles()
        {
            // Find the oldest expired or cancelled preloaded file in the database.
            FilePreload preloadedFile = await _preloadedFilesRepository.GetFirstAscending(
                x => x.Status == EmailPreloadStatus.Committed || x.Status == EmailPreloadStatus.Expired || x.Status == EmailPreloadStatus.Cancelled, o => o.PreloadedOn);

            // If an expired preloaded file was found...
            if (preloadedFile != null)
            {
                // Get a list of all related preloaded files based on the batch ID.
                var preloadedFiles = await _preloadedFilesRepository.FindAndProject(
                    x => x.BatchId == preloadedFile.BatchId, file => new { file.Id, file.TempPath });

                // Remove the expired preloaded files from temporary path for the one specific batch.
                await _fileHelper.DeleteFile(preloadedFiles.Select(f => f.TempPath).ToList());

                // Remove the expired preloaded files for the one specific batch.
                _ = await _preloadedFilesRepository.Delete(preloadedFiles.Select(f => f.Id).ToList());

                // Return the number of preloaded files that have been removed.
                return preloadedFiles.Count;
            }

            // Return zero if no expired preloaded files were found.
            return 0;
        }

        public async Task<int> CleanupTransactions()
        {
            // Find the oldest expired transaction in the database.
            HistoricTransaction historicTransaction = await _transactionHistoryRepository.GetFirstAscending(
                x => x.Status == TransactionStatus.Expired, o => o.ProcessingFinished);

            // If an expired transaction was found...
            if (historicTransaction != null)
            {
                // Force a database transaction scope to make sure multiple
                // operations are combined as a single atomic operation.
                using ITransactionScope transactionScope = _transactionHelper.CreateScope();

                // Get the batch ID from the expired transaction.
                Guid batchId = historicTransaction.BatchId;

                // Get a list of all related email ID's.
                IList<Guid> emailIds = await _emailRepository.FindAndProject(
                    x => x.Batch.Id == batchId, proj => proj.Id);

                // Get a list of all related file ID's.
                var files = await _fileRepository.FindAndProject(
                    x => emailIds.Contains(x.Email.Id), file => new { file.Id, file.TempPath });

                // Remove the expired transaction related files from temporary path                
                await _fileHelper.DeleteFile(files.Select(f => f.TempPath).ToList());

                List<Guid> fileIds = files.Select(f => f.Id).ToList();
                // Get list of  metadata related to fileIds
                var metadataIds = await _metadataRepository.FindAndProject(x => fileIds.Contains(x.FileId), x => x.Id);

                // Remove the expired transaction and the related
                // batch, emails , files and metadata from the database

                //Remove files metadata
                _ = await _metadataRepository.Delete(metadataIds);

                _ = await _fileRepository.Delete(files.Select(f => f.Id).ToList());

                // Get list of all Email Recipients
                var emailRecipients = await _emailRecipientRepository.FindAndProject(
                    x => emailIds.Contains(x.Email.Id), emailRecipient => emailRecipient.Id);
                // Remove email recipients
                _ = await _emailRecipientRepository.Delete(emailRecipients);
                // Remove emails
                _ = await _emailRepository.Delete(emailIds);
                _ = await _batchRepository.Delete(batchId);
                _ = await _transactionHistoryRepository.Delete(historicTransaction);

                // Commit the transaction.
                transactionScope.Commit();

                // Return the total count of records removed from the database.
                // Number of files + number of emails + batch + transaction.
                return files.Count + emailIds.Count + 2;
            }

            // Return zero if no expired preloaded files were found.
            return 0;
        }

        /// <summary>
        /// Removing records from database which are Processed and Expired for longer time 
        /// </summary>
        /// <returns>Total count of records removed from the database.</returns>
        public async Task<int> CleanupCategorizations()
        {
            // Find the processed/expired categories in the database.
            IList<Guid> categorizationRequest = await _categorizationRequestRepository.FindAndProject(
                x => x.Status == CategorizationRequestStatus.Processed || x.Status == CategorizationRequestStatus.Expired, category => category.Id);

            // If an processed/expired categories was found...
            if (categorizationRequest.Count > 0)
            {
                await _categorizationRequestRepository.Delete(categorizationRequest);

                // Return the total count of records removed from the database.
                return categorizationRequest.Count;
            }
            return 0;

        }
        /// <summary>
        /// Updating categorization status type as expired.
        /// </summary>
        /// <returns>Total count of records are set to expired status.</returns>
        public async Task<int> ExpireCategorizations()
        {
            // Read the time in minutes for expiry from configuration and calculate the datetime offset.
            int expiry = _configuration.CategorizationExpiryInMinutes;
            DateTime expirationDateTime = DateTime.UtcNow.AddMinutes(-expiry);

            // Search for stuck categories that are older than the expiry offset.
            IList<CategorizationRequest> categorizationRequests = await _categorizationRequestRepository.Find(
                x => x.Status == CategorizationRequestStatus.Pending && x.InsertedOn < expirationDateTime);

            // Change the status for all eligible categories to expired.
            foreach (CategorizationRequest categorization in categorizationRequests)
            {
                categorization.Status = CategorizationRequestStatus.Expired;
            }

            // Update all expired categories in the database.
            await _categorizationRequestRepository.Update(categorizationRequests);

            // Return the number of categories that have been marked as expired.
            return categorizationRequests.Count;

        }
    }
}