using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories.Interfaces;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class CleanupManager : ICleanupManager
    {
        private readonly IRepository<FilePreload> _preloadedFilesRepository;
        private readonly IRepository<HistoricTransaction> _transactionHistoryRepository;
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<Email> _emailRepository;
        private readonly IRepository<File> _fileRepository;
        private readonly ITransactionHelper _transactionHelper;
        private readonly IEdreamsConfiguration _configuration;

        public CleanupManager(
            IRepository<FilePreload> preloadedFilesRepository,
            IRepository<HistoricTransaction> transactionHistoryRepository,
            IRepository<Batch> batchRepository,
            IRepository<Email> emailRepository,
            IRepository<File> fileRepository,
            ITransactionHelper transactionHelper,
            IEdreamsConfiguration configuration)
        {
            _preloadedFilesRepository = preloadedFilesRepository;
            _transactionHistoryRepository = transactionHistoryRepository;
            _batchRepository = batchRepository;
            _emailRepository = emailRepository;
            _fileRepository = fileRepository;
            _transactionHelper = transactionHelper;
            _configuration = configuration;
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
                x => x.Status == EmailPreloadStatus.Expired || x.Status == EmailPreloadStatus.Cancelled, o => o.PreloadedOn);

            // If an expired preloaded file was found...
            if (preloadedFile != null)
            {
                // Get a list of all related preloaded files based on the batch ID.
                IList<Guid> preloadedFileIds = await _preloadedFilesRepository.FindAndProject(
                    x => x.BatchId == preloadedFile.BatchId, proj => proj.Id);

                // Remove the expired preloaded files for the one specific batch.
                await _preloadedFilesRepository.Delete(preloadedFileIds);

                // Return the number of preloaded files that have been removed.
                return preloadedFileIds.Count;
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
                IList<Guid> fileIds = await _fileRepository.FindAndProject(
                    x => emailIds.Contains(x.Email.Id), proj => proj.Id);

                // Remove the expired transaction and the related
                // batch, emails and files from the database
                await _fileRepository.Delete(fileIds);
                await _emailRepository.Delete(emailIds);
                await _batchRepository.Delete(batchId);
                await _transactionHistoryRepository.Delete(historicTransaction);

                // Commit the transaction.
                transactionScope.Commit();

                // Return the total count of records removed from the database.
                // Number of files + number of emails + batch + transaction.
                return fileIds.Count + emailIds.Count + 2;
            }

            // Return zero if no expired preloaded files were found.
            return 0;
        }
    }
}