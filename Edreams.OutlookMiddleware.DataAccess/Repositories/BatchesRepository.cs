using Edreams.Common.DataAccess;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class BatchesRepository : Repository<Batch>
    {
        public BatchesRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.Batches, securityContext)
        {

        }
    }
}