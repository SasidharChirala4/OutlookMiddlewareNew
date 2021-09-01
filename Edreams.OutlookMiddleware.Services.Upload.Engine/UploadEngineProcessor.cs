using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Common.Exceptions.Factories.Interfaces;
using Edreams.Common.Exchange.Contracts;
using Edreams.Common.Exchange.Interfaces;
using Edreams.Common.KeyVault.Interfaces;
using Edreams.Common.Logging.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Constants;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Services.Upload.Engine.Interfaces;
using ProjectTask = Edreams.Contracts.Data.Extensibility.ProjectTask;
using SharePointFile = Edreams.Contracts.Data.Common.SharePointFile;
namespace Edreams.OutlookMiddleware.Services.Upload.Engine
{
    public class UploadEngineProcessor : IUploadEngineProcessor
    {
        private readonly IBatchManager _batchManager;
        private readonly IEmailManager _emailManager;
        private readonly IFileManager _fileManager;
        private readonly IExtensibilityManager _extensibilityManager;
        private readonly IProjectTaskManager _projectTaskManager;
        private readonly ITransactionQueueManager _transactionQueueManager;
        private readonly IFileHelper _fileHelper;
        private readonly IExchangeAndKeyVaultHelper _exchangeAndKeyVaultHelper;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly IEdreamsLogger<UploadEngineProcessor> _logger;

        public UploadEngineProcessor(
            IBatchManager batchManager, IEmailManager emailManager,
            IFileManager fileManager, IExtensibilityManager extensibilityManager, IProjectTaskManager projectTaskManager,
            ITransactionQueueManager transactionQueueManager, IFileHelper fileHelper,
            IExchangeAndKeyVaultHelper exchangeAndKeyVaultHelper,
            IExceptionFactory exceptionFactory, IEdreamsLogger<UploadEngineProcessor> logger)
        {
            _batchManager = batchManager;
            _emailManager = emailManager;
            _fileManager = fileManager;
            _extensibilityManager = extensibilityManager;
            _projectTaskManager = projectTaskManager;
            _transactionQueueManager = transactionQueueManager;
            _fileHelper = fileHelper;
            _exchangeAndKeyVaultHelper = exchangeAndKeyVaultHelper;
            _exceptionFactory = exceptionFactory;
            _logger = logger;
        }

        public async Task Process(TransactionMessage transactionMessage)
        {
            Guid transactionId = transactionMessage.TransctionId;
            Guid batchId = transactionMessage.BatchId;
            _logger.LogInformation($"Uploading files for batch {batchId} started");
            // Create a client for Azure KeyVault, authenticated using the appsettings.json settings.
            IKeyVaultClient keyVaultClient = _exchangeAndKeyVaultHelper.CreateKeyVaultClient();

            // Create a client for EWS, authenticated using data from Azure KeyVault.
            IExchangeClient exchangeClient = await _exchangeAndKeyVaultHelper.CreateExchangeClient(keyVaultClient);

            try
            {
                // Fetch all details for the batch from the database, into a single DTO.
                BatchDetailsDto batchDetails = await _batchManager.GetBatchDetails(batchId);

                int numberOfSuccessfullyUploadedEmails = 0;

                List<SharePointFile> sharepointFileUploads = new List<SharePointFile>();
                _logger.LogInformation($"{batchDetails.Emails.Count} emails are ready to be uploaded to e-DReaMS!");
                // Loop through all emails that are part of this batch.
                foreach (EmailDetailsDto emailDetails in batchDetails.Emails)
                {
                    int numberOfSuccessfullyUploadedFiles = 0;
                    // Get sent email details from shared mailbox, or null if the email is not of type: "Sent".
                    ExchangeEmail sentEmailDetails = await GetSentEmailDetails(emailDetails, exchangeClient);
                    _logger.LogInformation($"Uploading started for email [{emailDetails.Id}] ");
                    // Loop through all the files that are part of this email.
                    foreach (FileDetailsDto fileDetails in emailDetails.Files)
                    {
                        try
                        {
                            _logger.LogInformation($"Uploading File [{fileDetails.OriginalName}] for mail [{fileDetails.EmailSubject}]");
                            // Skip the files that are not matched with upload option or shouldupload option is set to false.
                            if (!IsFileSkipped(batchDetails.UploadOption, fileDetails))
                            {
                                // Process the file based on the file details.
                                SharePointFile sharepointFile = await ProcessFile(emailDetails, sentEmailDetails, fileDetails, batchDetails.UploadLocationSite, batchDetails.UploadLocationFolder);

                                sharepointFileUploads.Add(sharepointFile);

                                // set metadata for file in sharepoint 
                                await _extensibilityManager.SetFileMetadata(batchDetails.UploadLocationSite, sharepointFile.AbsoluteUrl, fileDetails.Metadata, batchDetails.VersionComment, batchDetails.DeclareAsRecord);

                                // Set the file status to be successfully uploaded and
                                await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.Uploaded);
                                // increase the number of successfully uploaded files.
                                numberOfSuccessfullyUploadedFiles++;
                                _logger.LogInformation($"File [{fileDetails.OriginalName}] for mail [{fileDetails.EmailSubject}] Uploaded successfully");
                            }
                            else
                            {
                                // Set the file status to be skipped if file kind doesnot match with upload option or shouldupload option is set to false.
                                await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.Skipped);
                            }
                        }
                        catch
                        {
                            _logger.LogWarning($"Error occured while uploading File [{fileDetails.OriginalName}] for mail [{fileDetails.EmailSubject}]");
                            // Set the file status to failed to upload.
                            await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.FailedToUpload);
                        }
                    }

                    _logger.LogInformation($"Uploading ended for email [{emailDetails.Id}] ");

                    int numberOfShouldUploadFalseFiles = emailDetails.Files.Select(x => x.ShouldUpload).Count();
                    // Determine the email status by comparing the number of successful uploads and the total number of files.
                    EmailStatus emailStatus = CalculateEmailStatus(emailDetails.Files.Count, numberOfSuccessfullyUploadedFiles, numberOfShouldUploadFalseFiles);

                    // Increase the number of successfully uploaded emails if the status is successful.
                    if (emailStatus == EmailStatus.Successful)
                    {
                        numberOfSuccessfullyUploadedEmails++;
                    }

                    //Create Task
                    if (emailDetails.ProjectTaskDto != null)
                    {
                        ProjectTask projectTask = _projectTaskManager.GetEdreamsProjectTask(emailDetails, sharepointFileUploads, batchDetails.UploadLocationSite);
                        ProjectTask newProjectTask = await _extensibilityManager.CreateEdreamsProjectTask(projectTask);
                        _logger.LogInformation("Task created for email with ID: " + emailDetails.Id + " successfully");
                    }
                    // Update email status based on the success rate.
                    await _emailManager.UpdateEmailStatus(emailDetails.Id, emailStatus);
                }

                _logger.LogInformation($"{numberOfSuccessfullyUploadedEmails} out of {batchDetails.Emails.Count} emails are uploaded to e-DReaMS!");
                // Determine the batch status by comparing the number of successful uploads and the total number of emails.
                BatchStatus batchStatus = CalculateBatchStatus(batchDetails.Emails.Count, numberOfSuccessfullyUploadedEmails);
                
                // Update batch status based on the success rate.
                await _batchManager.UpdateBatchStatus(batchDetails.Id, batchStatus);

                // Create a new transaction to schedule the categorization process.
                await _transactionQueueManager.CreateCategorizationTransaction(batchId);
                _logger.LogInformation($"Created Categorization transaction for batch {batchId}");

                // Update the transaction to have a succeeded status.
                await _transactionQueueManager.UpdateTransactionStatusAndArchive(transactionId, TransactionStatus.ProcessingSucceeded);

                _logger.LogInformation($"Uploading files for batch {batchId} finished. Batch status set to {batchStatus}.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Uploading files for batch {batchId}");
                throw;
            }

        }

        #region <| Helper Methods |>

        private async Task<ExchangeEmail> GetSentEmailDetails(EmailDetailsDto emailDetails, IExchangeClient exchangeClient)
        {
            // Check if the email is sent email and EdreamsReferenceId is not empty
            if (emailDetails.EmailKind == EmailKind.Sent && emailDetails.EdreamsReferenceId != Guid.Empty)
            {
                // Get the sent emails details from the exchange service.
                ExchangeEmail sentEmailDetails = await exchangeClient.FindEmailByExtendedProperty("EdreamsReferenceId", $"{emailDetails.EdreamsReferenceId}");

                if (sentEmailDetails != null)
                {
                    emailDetails.InternetMessageId = sentEmailDetails.InternetMessageId;

                    // Update internetMessageId, EwsId for email
                    await _emailManager.UpdateEmailInternetMessageId(emailDetails.Id, sentEmailDetails.InternetMessageId, sentEmailDetails.EwsId);
                    _logger.LogInformation($"Email '{sentEmailDetails.Subject}' successfully found in the SharedMailBox!");
                    return sentEmailDetails;
                }

                // Set email status to failed.
                await _emailManager.UpdateEmailStatus(emailDetails.Id, EmailStatus.Failed);

                // Throw an exception if mail not found in SharedMailBox
                _logger.LogError(new FileNotFoundException(), string.Format("Email '{0}' not found in SharedMailBox!", emailDetails.EdreamsReferenceId));
            }

            return null;
        }

        private byte[] GetFileData(ExchangeEmail sentEmail, FileDetailsDto fileDetails)
        {
            if (fileDetails.Kind == FileKind.Email)
            {
                fileDetails.OriginalName = sentEmail.Subject;
                return sentEmail.Data;
            }

            if (fileDetails.Kind == FileKind.Attachment)
            {
                ExchangeAttachement attachment = sentEmail.Attachments.SingleOrDefault(x => x.Name == fileDetails.OriginalName);
                if (attachment != null)
                {
                    fileDetails.OriginalName = attachment.Name;
                    return attachment.Data;
                }
            }

            return null;
        }

        private async Task<SharePointFile> ProcessFile(EmailDetailsDto emailDetails, ExchangeEmail sentEmail, FileDetailsDto fileDetails, string uploadLocationSite, string uploadLocationFolder)
        {
            try
            {
                byte[] fileData;
                // Check if the Email is sent email.
                if (emailDetails.EmailKind == EmailKind.Sent && sentEmail != null)
                {
                    // Load the file into memory from its temporary path.
                    fileData = GetFileData(sentEmail, fileDetails);
                }
                else
                {
                    // Load the file into memory from its temporary path.
                    fileData = await _fileHelper.LoadFileInMemory(fileDetails.Path);
                }

                _logger.LogInformation($"Uploading File [{fileDetails.OriginalName}] to e-DReaMS");
                return await _extensibilityManager.UploadFile(fileData, uploadLocationSite, uploadLocationFolder, fileDetails.NewName, true);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Uploading File [{fileDetails.OriginalName}] to e-DReaMS failed");
                throw _exceptionFactory.CreateEdreamsExceptionFromCode(EdreamsOutlookMiddlewareExceptionCode.OutlookMiddlewareUploadToEdreamsFailed, ex);
            }
        }

        private EmailStatus CalculateEmailStatus(int totalNumberOfFiles, int numberOfSuccessfullyUploadedFiles, int numberOfShouldUploadFalseFiles)
        {
            if (numberOfSuccessfullyUploadedFiles == 0)
            {
                return EmailStatus.Failed;
            }

            if (numberOfSuccessfullyUploadedFiles + numberOfShouldUploadFalseFiles == totalNumberOfFiles)
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

        /// <summary>
        /// Skips the file if file kind is matched with upload option or shouldupload is set to false.
        /// </summary>
        /// <param name="uploadOption">Email Upload Option</param>
        /// <param name="fileDetails">File Details</param>
        /// <returns>Boolen value specifies file can be skipped or not</returns>
        private bool IsFileSkipped(EmailUploadOptions uploadOption, FileDetailsDto fileDetails)
        {
            // checks the file should upload option.
            if (!fileDetails.ShouldUpload)
            {
                _logger.LogInformation(string.Format("File {0} is skipped because ShouldUpload option set to false", fileDetails.Id));
                return true;
            }

            if (uploadOption == EmailUploadOptions.Emails)
            {
                if (fileDetails.Kind != FileKind.Email)
                {
                    _logger.LogInformation(string.Format("File {0} is skipped because file type is not matched with upload option", fileDetails.Id));
                    return true;
                }
                return false;
            }

            if (uploadOption == EmailUploadOptions.Attachments)
            {
                if (fileDetails.Kind != FileKind.Attachment)
                {
                    _logger.LogInformation(string.Format("File {0} is skipped because file type is not matched with upload option", fileDetails.Id));
                    return true;
                }
                return false;
            }

            return false;
        }

        #endregion
    }
}