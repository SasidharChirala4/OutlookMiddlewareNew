using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping.Custom
{
    public class EmailRecipientsToEmailRecipientDetailsMapper : IEmailRecipientsToEmailRecipientDetailsMapper
    {
        public IList<EmailRecipientDto> Map(IList<EmailRecipient> emailRecipients)
        {
            List<EmailRecipientDto> listOfEmailRecipientDetails = new List<EmailRecipientDto>(emailRecipients.Count);

            foreach (EmailRecipient emailRecipient in emailRecipients)
            {
                EmailRecipientDto emailRecipientDto = new EmailRecipientDto
                {
                    Recipient = emailRecipient.Recipient,
                    Type = emailRecipient.Type,
                    EmailId = emailRecipient.Email.Id
                };

                listOfEmailRecipientDetails.Add(emailRecipientDto);
            }

            return listOfEmailRecipientDetails;
        }
    }
}