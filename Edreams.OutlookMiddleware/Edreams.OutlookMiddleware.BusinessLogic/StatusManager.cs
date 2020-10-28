using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class StatusManager : IStatusManager
    {
        public Task<GetStatusResponse> GetStatus()
        {
            return Task.FromResult(new GetStatusResponse
            {
                CorrelationId = Guid.NewGuid()
            });
        }
    }
}