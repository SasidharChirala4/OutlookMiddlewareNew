using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Services.Categorization.Engine.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Services.Categorization.Engine
{
    public class CategorizationEngineProcessor : ICategorizationEngineProcessor
    {
        private readonly IEmailManager _emailManager;
        private readonly ITransactionQueueManager _transactionQueueManager;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly ILogger<CategorizationEngineProcessor> _logger;
        private readonly ICategorizationManager _categorizationManager;
        private readonly IExchangeClient _exchangeClient;

        public CategorizationEngineProcessor(IEmailManager emailManager,
            ITransactionQueueManager transactionQueueManager,
            IExceptionFactory exceptionFactory, ILogger<CategorizationEngineProcessor> logger,
            ICategorizationManager categorizationManager,
            IExchangeClient exchangeClient)
        {
            _emailManager = emailManager;
            _transactionQueueManager = transactionQueueManager;
            _exceptionFactory = exceptionFactory;
            _logger = logger;
            _categorizationManager = categorizationManager;
            _exchangeClient = exchangeClient;
        }

        public async Task Process(TransactionMessage transactionMessage)
        {
            Guid transactionId = transactionMessage.TransctionId;
            Guid batchId = transactionMessage.BatchId;

            try
            {
                // Fetch all email details for the batch from the database.
                IList<Email> emails = await _emailManager.GetEmails(batchId);

                foreach (var email in emails)
                {
                    // Fetch details of the emailRecipients by emailId from the database, into a List of DTO
                    IList<EmailRecipientDto> emailRecipients = await _emailManager.GetEmailRecipients(email.Id);
                    List<string> recipients = new List<string>();
                    if (emailRecipients.Count > 0)
                    {
                        foreach (EmailRecipientDto emailRecipient in emailRecipients)
                        {
                            // cheking emailRecipientType as distribution list or not
                            // if it's a distribution list we will expand recipients from that group.
                            if (emailRecipient.Type == EmailRecipientType.DistributionList)
                            {
                                // expanding recipients from distributionList group
                                IList<string> expandedRecipientLists = await _exchangeClient.ExpandDistributionLists(emailRecipient.Recipient);
                                if (expandedRecipientLists.Count > 0)
                                {
                                    // adding recipients into list
                                    recipients.AddRange(expandedRecipientLists);
                                }
                            }
                            else
                            {
                                // adding recipients into list
                                recipients.Add(emailRecipient.Recipient);
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
            }
            catch (Exception ex)
            {
                // TODO: Do better logging.
                _logger.LogError(ex, ex.Message);
                throw;
            }

            throw new NotImplementedException();
        }
    }
}
