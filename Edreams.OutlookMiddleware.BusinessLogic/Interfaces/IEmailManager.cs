using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;
using Task = System.Threading.Tasks.Task;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IEmailManager
    {
        Task<IList<Email>> GetEmails(Guid batchId);

        Task<IList<EmailRecipient>> GetEmailRecipients(Guid emailId);

        Task<CreateMailResponse> CreateMail(CreateMailRequest request);

        Task UpdateEmailStatus(Guid emailId, EmailStatus status);

        Task UpdateEmailInternetMessageId(Guid emailId, string internetMessageId, string ewsId);
    }
}