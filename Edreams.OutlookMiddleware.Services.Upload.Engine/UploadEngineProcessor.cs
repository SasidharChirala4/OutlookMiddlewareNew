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
                // Loop through all emails that are part of this batch.
                foreach (EmailDetailsDto emailDetails in batchDetails.Emails)
                {
                    int numberOfSuccessfullyUploadedFiles = 0;
                    // Get sent email details from shared mailbox, or null if the email is not of type: "Sent".
                    ExchangeEmail sentEmailDetails = await GetSentEmailDetails(emailDetails, exchangeClient);

                    // Loop through all the files that are part of this email.
                    foreach (FileDetailsDto fileDetails in emailDetails.Files)
                    {
                        try
                        {
                            // Skip the files that are not matched with upload option or shouldupload option is set to false.
                            if (!IsFileSkipped(batchDetails.UploadOption, fileDetails))
                            {
                                // Process the file based on the file details.
                                SharePointFile sharepointFile = await ProcessFile(emailDetails, sentEmailDetails, fileDetails, batchDetails.UploadLocationSite, batchDetails.UploadLocationFolder);
                                // TODO: Update absolute file URL in database as part of metadata PBI.
                                // This can be handled in pbi #40965
                                sharepointFileUploads.Add(sharepointFile);

                                // ToDo: Need to remove odd coded values for vertioncomment and declaredasrecord 
                                // need to discuss with johnny or sasi

                                // set metadata for file in sharepoint 
                                await _extensibilityManager.SetFileMetaData(batchDetails.UploadLocationSite, sharepointFile.AbsoluteSiteUrl, fileDetails.Metadata, "",false);

                                // Set the file status to be successfully uploaded and
                                await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.Uploaded);
                                // increase the number of successfully uploaded files.
                                numberOfSuccessfullyUploadedFiles++;

                            }
                            else
                            {
                                // Set the file status to be skipped if file kind doesnot match with upload option or shouldupload option is set to false.
                                await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.Skipped);
                            }
                        }
                        catch
                        {
                            // Set the file status to failed to upload.
                            await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.FailedToUpload);
                        }
                    }

                    int numberOfShouldUploadFalseFiles = emailDetails.Files.Select(x => x.ShouldUpload).Count();
                    // Determine the email status by comparing the number of successful uploads and the total number of files.
                    EmailStatus emailStatus = CalculateEmailStatus(emailDetails.Files.Count, numberOfSuccessfullyUploadedFiles, numberOfShouldUploadFalseFiles);

                    // Increase the number of successfully uploaded emails if the status is successful.
                    if (emailStatus == EmailStatus.Successful)
                    {
                        numberOfSuccessfullyUploadedEmails++;
                    }
                    List<MetadataDto> emailMetadata = emailDetails.Files.First(x => x.Kind == Enums.FileKind.Email).Metadata;

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

                // Upload the file to e-DReaMS.
                return await _extensibilityManager.UploadFile(fileData, uploadLocationSite, uploadLocationFolder, fileDetails.NewName, true);
            }
            catch (Exception ex)
            {
                // Throw an exception.
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
                _logger.LogInformation(string.Format("File {0} is skipped because file type is not matched with upload option", fileDetails.Id));
                return fileDetails.Kind != FileKind.Email;
            }

            if (uploadOption == EmailUploadOptions.Attachments)
            {
                _logger.LogInformation(string.Format("File {0} is skipped because file type is not matched with upload option", fileDetails.Id));
                return fileDetails.Kind != FileKind.Attachment;
            }



            return false;
        }

        #endregion
    }
}