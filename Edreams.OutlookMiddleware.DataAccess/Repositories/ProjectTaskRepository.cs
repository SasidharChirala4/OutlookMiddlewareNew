using Edreams.Common.DataAccess;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class ProjectTaskRepository : Repository<ProjectTask>
    {
        public ProjectTaskRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.ProjectTasks, securityContext)
        {

        }
    }
}
