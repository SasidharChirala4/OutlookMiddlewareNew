using Edreams.Common.DataAccess;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class ProjectTaskUserInvolvementRepository : Repository<ProjectTaskUserInvolvement>
    {
        public ProjectTaskUserInvolvementRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.ProjectTaskUserInvolvements, securityContext) { }
    }
}