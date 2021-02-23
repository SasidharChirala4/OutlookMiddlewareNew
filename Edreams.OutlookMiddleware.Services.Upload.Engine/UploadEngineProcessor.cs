using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Services.Upload.Engine.Interfaces;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Services.Upload.Engine
{
    public class UploadEngineProcessor : IUploadEngineProcessor
    {
        private readonly IBatchManager _batchManager;
        private readonly IEmailManager _emailManager;
        private readonly IFileManager _fileManager;
        private readonly IExtensibilityManager _extensibilityManager;
        private readonly ITransactionQueueManager _transactionQueueManager;
        private readonly IFileHelper _fileHelper;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly ILogger<UploadEngineProcessor> _logger;

        public UploadEngineProcessor(
            IBatchManager batchManager, IEmailManager emailManager,
            IFileManager fileManager, IExtensibilityManager extensibilityManager,
            ITransactionQueueManager transactionQueueManager, IFileHelper fileHelper,
            IExceptionFactory exceptionFactory, ILogger<UploadEngineProcessor> logger)
        {
            _batchManager = batchManager;
            _emailManager = emailManager;
            _fileManager = fileManager;
            _extensibilityManager = extensibilityManager;
            _transactionQueueManager = transactionQueueManager;
            _fileHelper = fileHelper;
            _exceptionFactory = exceptionFactory;
            _logger = logger;
        }

        public async Task Process(TransactionMessage transactionMessage)
        {
            Guid transactionId = transactionMessage.TransctionId;
            Guid batchId = transactionMessage.BatchId;

            try
            {
                // Fetch all details for the batch from the database, into a single DTO.
                BatchDetailsDto batchDetails = await _batchManager.GetBatchDetails(batchId);

                int numberOfSuccessfullyUploadedEmails = 0;

                // Loop through all emails that are part of this batch.
                foreach (EmailDetailsDto emailDetails in batchDetails.Emails)
                {
                    int numberOfSuccessfullyUploadedFiles = 0;

                    // Loop through all the files that are part of this email.
                    foreach (FileDetailsDto fileDetails in emailDetails.Files)
                    {
                        try
                        {
                            // Process the file based on the file details.
                            string absoluteFileUrl = await ProcessFile(fileDetails);

                            // TODO: Update absolute file URL in database as part of metadata PBI.

                            // Set the file status to be successfully uploaded and
                            // increase the number of successfully uploaded files.
                            await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.Uploaded);
                            numberOfSuccessfullyUploadedFiles++;
                        }
                        catch
                        {
                            // Set the file status to failed to upload.
                            await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.FailedToUpload);
                        }
                    }

                    // Determine the email status by comparing the number of successful uploads and the total number of files.
                    EmailStatus emailStatus = CalculateEmailStatus(emailDetails.Files.Count, numberOfSuccessfullyUploadedFiles);

                    // Increase the number of successfully uploaded emails if the status is successful.
                    if (emailStatus == EmailStatus.Successful)
                    {
                        numberOfSuccessfullyUploadedEmails++;
                    }

                    // Update email status based on the success rate.
                    await _emailManager.UpdateEmailStatus(emailDetails.Id, emailStatus);
                }

                // Determine the batch status by comparing the number of successful uploads and the total number of emails.
                BatchStatus batchStatus = CalculateBatchStatus(batchDetails.Emails.Count, numberOfSuccessfullyUploadedEmails);

                // Update batch status based on the success rate.
                await _batchManager.UpdateBatchStatus(batchDetails.Id, batchStatus);

                // Create a new transaction to schedule the categorization process.
                await _transactionQueueManager.CreateCategorizationTransaction(batchId);

                // Update the transaction to have a succeeded status.
                await _transactionQueueManager.UpdateTransactionStatusAndArchive(transactionId, TransactionStatus.ProcessingSucceeded);
            }
            catch (Exception ex)
            {
                // TODO: Do better logging.
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        #region <| Helper Methods |>

        private async Task<string> ProcessFile(FileDetailsDto fileDetails)
        {
            try
            {
                // Load the file into memory from its temporary path.
                byte[] fileData = await _fileHelper.LoadFileInMemory(fileDetails.Path);

                // Upload the file to e-DReaMS.
                // TODO: Get the site URL and folder from metadata.
                string absoluteFileUrl = await _extensibilityManager.UploadFile(fileData, null, null, fileDetails.Name, true);

                return absoluteFileUrl;
            }
            catch (Exception ex)
            {
                // Throw an exception.
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.OUTLOOKMIDDLEWARE_UPLOAD_TO_EDREAMS_FAILED, ex);
            }
        }

        private EmailStatus CalculateEmailStatus(int totalNumberOfFiles, int numberOfSuccessfullyUploadedFiles)
        {
            if (numberOfSuccessfullyUploadedFiles == 0)
            {
                return EmailStatus.Failed;
            }

            if (numberOfSuccessfullyUploadedFiles == totalNumberOfFiles)
            {
                return EmailStatus.Successful;
            }

            return EmailStatus.Partially;
        }

        private BatchStatus CalculateBatchStatus(int totalNumberOfEmails, int numberOfSuccessfullyUploadedEmails)
        {
            if (numberOfSuccessfullyUploadedEmails == 0)
            {
                return BatchStatus.Failed;
            }

            if (numberOfSuccessfullyUploadedEmails == totalNumberOfEmails)
            {
                return BatchStatus.Successful;
            }

            return BatchStatus.Partially;
        }

        #endregion
    }
}