using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;


namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class CategorizationRequestRepository : Repository<CategorizationRequest>
    {
        public CategorizationRequestRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.CategorizationRequests, securityContext)
        {

        }
    }
}
