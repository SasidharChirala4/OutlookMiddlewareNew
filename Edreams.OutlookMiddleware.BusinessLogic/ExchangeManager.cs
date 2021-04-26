using Edreams.Common.Logging.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Exchange.Interfaces;
using Edreams.OutlookMiddleware.Common.KeyVault.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ExchangeManager : IExchangeManager
    {
        #region <| Dependencies |>

        private readonly IExchangeAndKeyVaultHelper _exchangeAndKeyVaultHelper;
        private readonly ISecurityContext _securityContext;
        private readonly IEdreamsLogger<ExchangeManager> logger;
        private readonly ExchangeService _exchangeService;
        #endregion

        public ExchangeManager(IEdreamsLogger<ExchangeManager> _logger, IExchangeAndKeyVaultHelper exchangeAndKeyVaultHelper,
            ISecurityContext securityContext, ExchangeService exchangeService)
        {
            logger = _logger;
            _exchangeAndKeyVaultHelper = exchangeAndKeyVaultHelper;
            _securityContext = securityContext;
            _exchangeService = exchangeService;
        }
        public async Task<GetSharedMailBoxResponse> GetSharedMailBox()
        {
            // Create a client for Azure KeyVault, authenticated using the appsettings.json settings.
            IKeyVaultClient keyVaultClient = _exchangeAndKeyVaultHelper.CreateKeyVaultClient();

            // Create a client for EWS, authenticated using data from Azure KeyVault.
            IExchangeClient exchangeClient = await _exchangeAndKeyVaultHelper.CreateExchangeClient(keyVaultClient);

            // Use the client for EWS to resolve the email address for the current 
            string emailAddress = await exchangeClient.ResolveEmailAddress();

            // Return a response containing the resolved email address and a correlation ID.
            return new GetSharedMailBoxResponse
            {
                EmailAddress = emailAddress,
                CorrelationId = _securityContext.CorrelationId
            };
        }

        /// <summary>
        /// Find the email in SharedMailBox by edreamsReferenceId
        /// </summary>
        /// <param name="edreamsReferenceId">unique id of sent email</param>
        /// <returns></returns>
        public async Task<SharedMailBoxDto> FindSharedMailBoxEmail(Guid edreamsReferenceId)
        {
            try
            {
                ItemView view = new ItemView(1, 0, OffsetBasePoint.Beginning);

                ExtendedPropertyDefinition emailExtendedPropDef =
                    new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, "SharedMailBoxMailId", MapiPropertyType.String);
                
                view.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties, emailExtendedPropDef);
                
                FindItemsResults<Item> findResults = await _exchangeService.FindItems(WellKnownFolderName.Inbox,
                    new SearchFilter.IsEqualTo(emailExtendedPropDef, $"{edreamsReferenceId}"), view);

                if (findResults.Items.Count == 1)
                {
                    findResults.Items[0].Load(new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.MimeContent, ItemSchema.Attachments));

                    EmailMessage emailMessage = (EmailMessage)findResults.Items[0];

                    SharedMailBoxDto sharedMailBoxMail = new SharedMailBoxDto
                    {
                        IsFound = true,
                        Subject = emailMessage.Subject,
                        InternetMessageId = emailMessage.InternetMessageId,
                        EwsId = emailMessage.Id.ToString(),
                        Data = emailMessage.MimeContent.Content
                    };

                    foreach (Attachment attachment in emailMessage.Attachments)
                    {
                        FileAttachment fileAttachment = attachment as FileAttachment;
                        if (fileAttachment != null)
                        {
                            attachment.Load();
                            sharedMailBoxMail.Attachments.Add(new SharedMailBoxAttachmentDto
                            {
                                Name = fileAttachment.Name,
                                Data = fileAttachment.Content
                            });
                        }
                    }

                    return sharedMailBoxMail;
                }

                logger.LogInformation(string.Format("Shared mailbox email '{0}' was not found!", edreamsReferenceId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"An error occured during EWS communication!");
            }

            return SharedMailBoxDto.NotFound;
        }

        /// <summary>
        /// Delete shared mail box emails.
        /// </summary>
        /// <param name="sharedMailBoxMailIds"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task DeleteSharedMailBoxMails(List<Guid> sharedMailBoxMailIds)
        {
            try
            {
                ItemView view = new ItemView(sharedMailBoxMailIds.Count, 0, OffsetBasePoint.Beginning);

                //LogicalOperation.Or" is performed, which indicates that a search must satisfy at least one of the search filters
                SearchFilter.SearchFilterCollection searchFilterCollection = new SearchFilter.SearchFilterCollection(LogicalOperator.Or);
                ExtendedPropertyDefinition emailExtendedPropDef =
                       new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, "SharedMailBoxMailId", MapiPropertyType.String);
                view.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties, emailExtendedPropDef);

                foreach (Guid sharedMailBoxMailId in sharedMailBoxMailIds)
                {
                    // Building Collection filter with all SharedMailBoxMailIds
                    searchFilterCollection.Add(new SearchFilter.IsEqualTo(emailExtendedPropDef, $"{sharedMailBoxMailId}"));
                }
                FindItemsResults<Item> findResults = await _exchangeService.FindItems(WellKnownFolderName.Inbox, searchFilterCollection, view);

                if (findResults.Items.Count > 0)
                {
                    IEnumerable<ItemId> itemIds = from p in findResults.Items select p.Id;
                    // deleting sharedMailbox items by group
                    _exchangeService.DeleteItems(itemIds, DeleteMode.HardDelete, null, null);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"An error occured during deleting SharedMailBoxMail!");
            }
        }

    }
}
