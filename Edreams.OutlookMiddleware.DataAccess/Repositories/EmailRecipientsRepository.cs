using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class EmailRecipientsRepository : Repository<EmailRecipient>
    {
        public EmailRecipientsRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.EmailRecipients, securityContext)
        {

        }
    }
}