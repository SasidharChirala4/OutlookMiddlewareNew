using Edreams.Common.DataAccess;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class EmailsRepository : Repository<Email>
    {
        public EmailsRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.Emails, securityContext)
        {

        }
    }
}