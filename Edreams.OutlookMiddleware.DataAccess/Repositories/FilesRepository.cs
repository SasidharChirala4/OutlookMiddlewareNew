using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class FilesRepository : Repository<File>
    {
        public FilesRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.Files, securityContext)
        {

        }
    }
}