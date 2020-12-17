using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class StatusManager : IStatusManager
    {
        private readonly ISecurityContext _securityContext;

        public StatusManager(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public Task<GetStatusResponse> GetStatus()
        {
            return Task.FromResult(new GetStatusResponse
            {
                CorrelationId = _securityContext.CorrelationId
            });
        }
    }
}