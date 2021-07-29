using Edreams.Common.DataAccess;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class MetaDataRepository : Repository<MetaData>
    {
        public MetaDataRepository(
          OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
          : base(dbContext, dbContext.MetaData, securityContext)
        {

        }
    }
}
