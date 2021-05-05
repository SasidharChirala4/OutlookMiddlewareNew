using System;
using System.Threading.Tasks;
using Edreams.Common.Exchange.Interfaces;
using Edreams.Common.KeyVault.Interfaces;
using Edreams.Common.Logging.Interfaces;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Helpers.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.Exchange.WebServices.Data;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class ConfigurationManager : IConfigurationManager
    {
        #region <| Dependencies |>

        private readonly IExchangeAndKeyVaultHelper _exchangeAndKeyVaultHelper;
        private readonly ISecurityContext _securityContext;
        private readonly IEdreamsLogger<ConfigurationManager> _logger;

        #endregion

        #region <| Construction |>

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationManager" /> class.
        /// </summary>
        /// <param name="exchangeAndKeyVaultHelper">The exchange and key vault helper.</param>
        /// <param name="securityContext">The security context.</param>
        /// <param name="logger">The logger.</param>
        public ConfigurationManager(
            IExchangeAndKeyVaultHelper exchangeAndKeyVaultHelper,
            ISecurityContext securityContext,
            IEdreamsLogger<ConfigurationManager> logger)
        {
            _exchangeAndKeyVaultHelper = exchangeAndKeyVaultHelper;
            _securityContext = securityContext;
            _logger = logger;
        }

        #endregion

        #region <| IConfigurationManager Implementation |>

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
        public async Task<SentEmailDto> GetSharedMailBoxEmail(Guid edreamsReferenceId)
        {
            try
            {
                // Create mail box view .
                ItemView view = new ItemView(1, 0, OffsetBasePoint.Beginning);
                // Create a client for Azure KeyVault, authenticated using the appsettings.json settings.
                IKeyVaultClient keyVaultClient = _exchangeAndKeyVaultHelper.CreateKeyVaultClient();
                // Create exchange service,  authenticated using data from Azure KeyVault.
                //ExchangeService exchangeService = await _exchangeAndKeyVaultHelper.CreateExchangeService(keyVaultClient);
                ExchangeService exchangeService = null;
                // Crete propery definition for EdreamsReferenceId property.
                ExtendedPropertyDefinition emailExtendedPropDef =
                    new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, "EdreamsReferenceId", MapiPropertyType.String);
                // Create object for PropertySet for EdreamsReferenceId Properydefinition
                view.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties, emailExtendedPropDef);

                // Find EdreamsReferencedEmail from exhange service.
                FindItemsResults<Item> findResults = await exchangeService.FindItems(WellKnownFolderName.Inbox,
                    new SearchFilter.IsEqualTo(emailExtendedPropDef, $"{edreamsReferenceId}"), view);

                if (findResults.Items.Count == 1)
                {
                    // Load the Email with MimeContent and AttachMents properties.
                    _ = findResults.Items[0].Load(new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.MimeContent, ItemSchema.Attachments));
                    EmailMessage emailMessage = (EmailMessage)findResults.Items[0];
                    PropertySet propSet = new PropertySet(BasePropertySet.FirstClassProperties);
                    propSet.Add(ItemSchema.MimeContent);

                    // Bind the EmailMessage with MimeContent property.
                    emailMessage = await EmailMessage.Bind(exchangeService, findResults.Items[0].Id, propSet);

                    // Prepare response object of type SharedMailBoxDto .
                    SentEmailDto sharedMailBoxMail = new SentEmailDto
                    {
                        IsFound = true,
                        Subject = emailMessage.Subject,
                        InternetMessageId = emailMessage.InternetMessageId,
                        EwsId = emailMessage.Id.ToString(),
                        Data = emailMessage.MimeContent.Content
                    };

                    // Add Email attachments to response object.
                    foreach (Attachment attachment in emailMessage.Attachments)
                    {
                        FileAttachment fileAttachment = attachment as FileAttachment;
                        if (fileAttachment != null)
                        {
                            await attachment.Load();
                            sharedMailBoxMail.Attachments.Add(new SentEmailAttachmentDto
                            {
                                Name = fileAttachment.Name,
                                Data = fileAttachment.Content
                            });
                        }
                    }

                    return sharedMailBoxMail;
                }

                _logger.LogInformation(string.Format("EdreamsReferenced email '{0}' was not found!", edreamsReferenceId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during EWS communication!");
            }

            return SentEmailDto.NotFound;
        }

        #endregion
    }
}