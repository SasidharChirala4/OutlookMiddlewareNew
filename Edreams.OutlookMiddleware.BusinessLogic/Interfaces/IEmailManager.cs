using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IEmailManager
    {
        Task<CreateMailResponse> CreateMail(CreateMailRequest request);

        Task UpdateEmailStatus(Guid emailId, EmailStatus status);
    }
}