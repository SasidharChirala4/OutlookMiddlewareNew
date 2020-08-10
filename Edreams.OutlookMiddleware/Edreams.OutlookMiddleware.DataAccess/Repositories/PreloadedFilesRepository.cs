using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class PreloadedFilesRepository : Repository<FilePreload>
    {
        public PreloadedFilesRepository(
            OutlookMiddlewarePreloadDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.PreloadedFiles, securityContext)
        {

        }
    }
}