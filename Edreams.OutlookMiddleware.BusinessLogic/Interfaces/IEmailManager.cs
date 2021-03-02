using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IEmailManager
    {
        Task<IList<Email>> GetEmails(Guid batchId);

        Task<IList<EmailRecipientDto>> GetEmailRecipients(Guid emailId);

        Task<CreateMailResponse> CreateMail(CreateMailRequest request);

        Task UpdateEmailStatus(Guid emailId, EmailStatus status);
    }
}