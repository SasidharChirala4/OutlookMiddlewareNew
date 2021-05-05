using System.Threading.Tasks;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic
{
    public class StatusManager : IStatusManager
    {
        private readonly ISecurityContext _securityContext;

        public StatusManager(
            ISecurityContext securityContext)
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