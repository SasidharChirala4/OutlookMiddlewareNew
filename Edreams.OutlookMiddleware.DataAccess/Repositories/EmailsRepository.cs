using Edreams.OutlookMiddleware.Common.Security.Interfaces;
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