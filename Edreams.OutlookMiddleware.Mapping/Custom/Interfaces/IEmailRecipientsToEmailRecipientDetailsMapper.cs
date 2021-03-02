using System.Collections.Generic;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.Mapping.Custom.Interfaces
{
    public interface IEmailRecipientsToEmailRecipientDetailsMapper
    {
        IList<EmailRecipientDto> Map(IList<EmailRecipient> emails);
    }
}