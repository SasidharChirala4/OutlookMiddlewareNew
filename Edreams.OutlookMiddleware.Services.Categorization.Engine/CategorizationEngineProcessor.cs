using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.Common.Exchange.Interfaces;
using Edreams.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Services.Categorization.Engine.Interfaces;

namespace Edreams.OutlookMiddleware.Services.Categorization.Engine
{
    public class CategorizationEngineProcessor : ICategorizationEngineProcessor
    {
        private readonly IEmailManager _emailManager;
        private readonly IBatchManager _batchManager;
        private readonly ITransactionQueueManager _transactionQueueManager;
        // TODO : Commented temporarily which is causing dependency issue. 
        // private readonly ILogger<CategorizationEngineProcessor> _logger;
        private readonly ICategorizationManager _categorizationManager;
        private readonly IExchangeAndKeyVaultHelper _exchangeAndKeyVaultHelper;

        public CategorizationEngineProcessor(
            IEmailManager emailManager,
            IBatchManager batchManager,
            ITransactionQueueManager transactionQueueManager,
            //ILogger<CategorizationEngineProcessor> logger,
            ICategorizationManager categorizationManager,
            IExchangeAndKeyVaultHelper exchangeAndKeyVaultHelper)
        {
            _emailManager = emailManager;
            _batchManager = batchManager;
            _transactionQueueManager = transactionQueueManager;
            // _logger = logger;
            _categorizationManager = categorizationManager;
            _exchangeAndKeyVaultHelper = exchangeAndKeyVaultHelper;
        }

        public async Task Process(TransactionMessage transactionMessage)
        {
            Guid transactionId = transactionMessage.TransctionId;
            Guid batchId = transactionMessage.BatchId;

            try
            {
                // Fetch all details for the batch from the database, into a single DTO.
                BatchDetailsDto batchDetails = await _batchManager.GetBatchDetails(batchId);

                // Create a client for Azure KeyVault, authenticated using the appsettings.json settings.
                IKeyVaultClient keyVaultClient = _exchangeAndKeyVaultHelper.CreateKeyVaultClient();

                // Create a client for EWS, authenticated using data from Azure KeyVault.
                IExchangeClient exchangeClient = await _exchangeAndKeyVaultHelper.CreateExchangeClient(keyVaultClient);

                foreach (var email in batchDetails.Emails)
                {
                    // skipping the categorization when batch upload option set to only Attachments and email doesnot have atleast one shouldupload file.
                    if (batchDetails.UploadOption == EmailUploadOptions.Attachments && email.Files.Select(x => x.ShouldUpload).Count() == 0)
                    {
                        continue;
                    }

                    // Fetch details of the emailRecipients by emailId from the database
                    IList<EmailRecipient> emailRecipients = await _emailManager.GetEmailRecipients(email.Id);
                    List<string> recipients = new List<string>();

                    // The condition will not occur regularly, which it will be applicable for rare scenario's 
                    if (emailRecipients.Count == 0)
                    {
                        throw new Exception($"EmailRecipients are null for the email {email.Id}");
                    }

                    // fetching recipient list which are not type as distribution
                    List<string> individualRecipientsList = emailRecipients.Where(x => x.Type != EmailRecipientType.DistributionList).Select(x => x.Recipient).ToList();
                    // adding individual recipients
                    recipients.AddRange(individualRecipientsList);

                    // fetching distribution recipients
                    List<string> distributionRecipientsList = emailRecipients.Where(x => x.Type == EmailRecipientType.DistributionList).Select(x => x.Recipient).ToList();

                    foreach (var emailRecipient in distributionRecipientsList)
                    {
                        // expanding recipients from distributionList group
                        IList<string> expandedRecipientLists = await exchangeClient.ExpandDistributionLists(emailRecipient);
                        if (expandedRecipientLists.Count > 0)
                        {
                            // adding distribution recipients into list
                            recipients.AddRange(expandedRecipientLists);
                        }
                    }
                    // Removing duplicate recipients from expandDistributionlist and emailRecipients
                    List<string> orginalRecipients = recipients.Distinct().ToList();

                    //Set the categorization request type based on upload option and email status.
                    CategorizationRequestType categorizationRequestType = GetCategorizationRequestType(batchDetails.UploadOption, email.Status);

                    // Adding Categorizations for the emailId
                    await _categorizationManager.AddCategorizationRequest(email.InternetMessageId, orginalRecipients, categorizationRequestType);

                }
                // Update the transaction to have a succeeded status.
                await _transactionQueueManager.UpdateTransactionStatusAndArchive(transactionId, TransactionStatus.ProcessingSucceeded);
            }
            catch (Exception ex)
            {
                // TODO: Do better logging.
                // _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        #region <| Helper Methods |>

        /// <summary>
        /// Get the categorization request type based on upload option and email status.
        /// </summary>
        /// <param name="uploadOption">Upload Option</param>
        /// <param name="status">Email Status</param>
        /// <returns>Get the CategorizationRequestType based on upload option and email status.</returns>
        private CategorizationRequestType GetCategorizationRequestType(EmailUploadOptions uploadOption, EmailStatus status)
        {
            if (status == EmailStatus.Failed || status == EmailStatus.Partially)
            {
                if (uploadOption == EmailUploadOptions.Emails)
                {
                    return CategorizationRequestType.EmailUploadFailed;
                }
                else if (uploadOption == EmailUploadOptions.Attachments)
                {
                    return CategorizationRequestType.AtachmentUploadFailed;
                }
                else
                {
                    return CategorizationRequestType.EmailAndAttachmentUploadFailed;
                }
            }
            else if (status == EmailStatus.Successful)
            {
                if (uploadOption == EmailUploadOptions.Emails)
                {
                    return CategorizationRequestType.EmailUploaded;
                }
                else if (uploadOption == EmailUploadOptions.Attachments)
                {
                    return CategorizationRequestType.AttachmentUploaded;
                }
                else
                {
                    return CategorizationRequestType.EmailAndAttachmentUploaded;
                }
            }
            return CategorizationRequestType.EmailUploadFailed;
        }
        #endregion
    }
}