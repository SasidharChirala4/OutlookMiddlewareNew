using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface IExchangeManager
    {
        Task<GetSharedMailBoxResponse> GetSharedMailBox();
        Task<SharedMailBoxDto> FindSharedMailBoxEmail(Guid sharedMailBoxMailId);
        Task DeleteSharedMailBoxMails(List<Guid> sharedMailBoxMailIds);
    }
}
