using Edreams.Common.DataAccess;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class MetadataRepository : Repository<Metadata>
    {
        public MetadataRepository(
          OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
          : base(dbContext, dbContext.Metadata, securityContext)
        {

        }
    }
}
