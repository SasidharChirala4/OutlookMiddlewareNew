﻿using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Services.Categorization.Engine.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces;

namespace Edreams.OutlookMiddleware.Services.Categorization.Engine
{
    public class CategorizationEngineProcessor : ICategorizationEngineProcessor
    {
        private readonly IEmailManager _emailManager;
        private readonly ITransactionQueueManager _transactionQueueManager;
        private readonly ILogger<CategorizationEngineProcessor> _logger;
        private readonly ICategorizationManager _categorizationManager;
        private readonly IExchangeAndKeyVaultHelper _exchangeAndKeyVaultHelper;

        public CategorizationEngineProcessor(
            IEmailManager emailManager,
            ITransactionQueueManager transactionQueueManager, 
            ILogger<CategorizationEngineProcessor> logger,
            ICategorizationManager categorizationManager,
            IExchangeAndKeyVaultHelper exchangeAndKeyVaultHelper)
        {
            _emailManager = emailManager;
            _transactionQueueManager = transactionQueueManager;
            _logger = logger;
            _categorizationManager = categorizationManager;
            _exchangeAndKeyVaultHelper = exchangeAndKeyVaultHelper;
        }

        public async Task Process(TransactionMessage transactionMessage)
        {
            Guid transactionId = transactionMessage.TransctionId;
            Guid batchId = transactionMessage.BatchId;

            try
            {
                // Fetch all email details for the batch from the database.
                IList<Email> emails = await _emailManager.GetEmails(batchId);

                // Create a client for Azure KeyVault, authenticated using the appsettings.json settings.
                IKeyVaultClient keyVaultClient = _exchangeAndKeyVaultHelper.CreateKeyVaultClient();

                // Create a client for EWS, authenticated using data from Azure KeyVault.
                IExchangeClient exchangeClient = await _exchangeAndKeyVaultHelper.CreateExchangeClient(keyVaultClient);

                foreach (var email in emails)
                {
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

                    // Adding Categorizations for the emailId
                    // TODO : Need to check with Johnny/Sasi about setting isUploaded status.
                    // TODO : Passing isUploaded value as true as of now  and need to adjust logic in categorizationManager 
                    await _categorizationManager.AddCategorizationRequest(email.InternetMessageId, orginalRecipients, true);

                    // Update the transaction to have a succeeded status.
                    await _transactionQueueManager.UpdateTransactionStatusAndArchive(transactionId, TransactionStatus.ProcessingSucceeded);
                }
            }
            catch (Exception ex)
            {
                // TODO: Do better logging.
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}