using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.Common.Helpers.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Services.Upload.Engine.Interfaces;
using Edreams.Common.Logging.Interfaces;
using System.IO;
using System.Linq;

namespace Edreams.OutlookMiddleware.Services.Upload.Engine
{
    public class UploadEngineProcessor : IUploadEngineProcessor
    {
        private readonly IBatchManager _batchManager;
        private readonly IEmailManager _emailManager;
        private readonly IFileManager _fileManager;
        private readonly IExtensibilityManager _extensibilityManager;
        private readonly ITransactionQueueManager _transactionQueueManager;
        private readonly IConfigurationManager _configurationManager;
        private readonly IFileHelper _fileHelper;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly IEdreamsLogger<UploadEngineProcessor> _logger;

        public UploadEngineProcessor(
            IBatchManager batchManager, IEmailManager emailManager,
            IFileManager fileManager, IExtensibilityManager extensibilityManager,
            ITransactionQueueManager transactionQueueManager, IFileHelper fileHelper,
            IConfigurationManager configurationManager,
            IExceptionFactory exceptionFactory, IEdreamsLogger<UploadEngineProcessor> logger)
        {
            _batchManager = batchManager;
            _emailManager = emailManager;
            _fileManager = fileManager;
            _extensibilityManager = extensibilityManager;
            _transactionQueueManager = transactionQueueManager;
            _configurationManager = configurationManager;
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
                    // Get sent email details from shared mail box
                    SentEmailDto sentEmailDetails = await GetSentEmailDetails(emailDetails);

                    // Loop through all the files that are part of this email.
                    foreach (FileDetailsDto fileDetails in emailDetails.Files)
                    {
                        try
                        {
                            //Skip the files that are not matched with upload option.
                            if (!IsFileSkipped(emailDetails.UploadOption, fileDetails.Kind))
                            {
                                // check if the Email is sent email.
                                if (emailDetails.EmailKind == EmailKind.Sent)
                                {
                                    byte[] itemBytes = GetFileData(emailDetails, fileDetails, sentEmailDetails);
                                    if (itemBytes != null)
                                    {
                                        // Process the file based on downloaded sharedmailbox email.
                                        string absoluteFileUrl = await ProcessShredMailBoxFile(itemBytes, fileDetails.Name);
                                    }
                                }
                                else
                                {
                                    // Process the file based on the file details.
                                    string absoluteFileUrl = await ProcessFile(fileDetails);
                                }
                                // TODO: Update absolute file URL in database as part of metadata PBI.

                                // Set the file status to be successfully uploaded and
                                // increase the number of successfully uploaded files.
                                await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.Uploaded);
                                numberOfSuccessfullyUploadedFiles++;
                            }
                            else
                            {
                                // Set the file status to be skipped if file kind doesnot match with upload option.
                                await _fileManager.UpdateFileStatus(fileDetails.Id, FileStatus.Skipped);
                            }
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
        private async Task<SentEmailDto> GetSentEmailDetails(EmailDetailsDto emailDetails)
        {
            // Check if the email is sent email and EdreamsReferenceId is not empty
            if (emailDetails.EmailKind == EmailKind.Sent && emailDetails.EdreamsReferenceId != Guid.Empty)
            {
                // Get the sent emails details from the exchange service.
                SentEmailDto sentEmailDetails = await _configurationManager.GetSharedMailBoxEmail(emailDetails.EdreamsReferenceId);
                if (sentEmailDetails != null)
                {
                    emailDetails.InternetMessageId = sentEmailDetails.InternetMessageId;
                    // Update internetMessageId, EwsId for email
                    await _emailManager.UpdateEmailInternetMessageId(emailDetails.Id,
                        sentEmailDetails.InternetMessageId, sentEmailDetails.EwsId);
                    return sentEmailDetails;
                }
                else // throw an exception if mail not found in SharedMailBox
                {
                    _logger.LogError(new FileNotFoundException(), string.Format("Email '{0}' not found in SharedMailBox!", emailDetails.EdreamsReferenceId));
                    // set Email status to failed.
                    await _emailManager.UpdateEmailStatus(emailDetails.Id, EmailStatus.Failed);
                }
            }
            return new SentEmailDto();
        }

        private byte[] GetFileData(EmailDetailsDto emailDetails, FileDetailsDto fileDetails, SentEmailDto sentEmailDetails)
        {
            if (fileDetails.Kind == FileKind.Attachment)
            {
                // If sentemail does not have any attchments then throw exception.
                if (sentEmailDetails.Attachments?.Any() != true)
                {
                    _logger.LogWarning($"Unable to read attachments for mail [{emailDetails.Id}] from SharedMailBox!");
                }
                else
                {
                    string attachmentName = $"{fileDetails.Name}";
                    // Find attachment related to file details
                    var sharedMailBoxAttachment = sentEmailDetails.Attachments.SingleOrDefault(x => x.Name.Equals(attachmentName, StringComparison.OrdinalIgnoreCase));
                    if (sharedMailBoxAttachment?.Data != null)
                    {
                        fileDetails.Name = sharedMailBoxAttachment.Name;
                        return sharedMailBoxAttachment.Data;
                    }
                    else
                    {
                        _logger.LogWarning($"Unable to find attachments [{fileDetails.Name}] for mail [{emailDetails.Id}] from SharedMailBox!");
                    }
                }
            }
            else  // if file kind is Email
            {
                if (sentEmailDetails.Data != null)
                {
                    fileDetails.Name = sentEmailDetails.Subject;
                    return sentEmailDetails.Data;
                }
                else
                {
                    _logger.LogWarning($"Unable to download file [{fileDetails.Name}] for mail [{emailDetails.Id}] from SharedMailBox!");
                }
            }
            return null;
        }
        private async Task<string> ProcessFile(FileDetailsDto fileDetails)
        {
            try
            {
                // Load the file into memory from its temporary path.
                byte[] fileData = await _fileHelper.LoadFileInMemory(fileDetails.Path);

                // Upload the file to e-DReaMS.
                // TODO: Get the site URL and folder from metadata.
                string absoluteFileUrl = await _extensibilityManager.UploadFile(fileData, "https://edreams4-t.be.deloitte.com/Sites/42k21cf7", "https://edreams4-t.be.deloitte.com/Sites/42k21cf7/42k240mr/AllDocuments/Correspondence", fileDetails.Name, true);

                return absoluteFileUrl;
            }
            catch (Exception ex)
            {
                // Throw an exception.
                throw _exceptionFactory.CreateFromCode(EdreamsExceptionCode.OUTLOOKMIDDLEWARE_UPLOAD_TO_EDREAMS_FAILED, ex);
            }
        }

        private async Task<string> ProcessShredMailBoxFile(byte[] fileData, string fileName)
        {
            try
            {
                // Upload the file to e-DReaMS.
                // TODO: Get the site URL and folder from metadata.
                string absoluteFileUrl = await _extensibilityManager.UploadFile(fileData, "https://edreams4-t.be.deloitte.com/Sites/42k21cf7", "https://edreams4-t.be.deloitte.com/Sites/42k21cf7/42k240mr/AllDocuments/Correspondence", fileName, true);

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

        /// <summary>
        /// Specifies whether the file kind is matched with upload option.
        /// </summary>
        /// <param name="uploadOption">Email Upload Option</param>
        /// <param name="fileKind">File Kind</param>
        /// <returns>boolen value specifies is file type is matched with upload option </returns>
        private bool IsFileSkipped(EmailUploadOptions uploadOption, FileKind fileKind)
        {
            if (uploadOption == EmailUploadOptions.Emails)
            {
                return fileKind != FileKind.Email;
            }
            else if (uploadOption == EmailUploadOptions.Attachments)
            {
                return fileKind != FileKind.Attachment;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}