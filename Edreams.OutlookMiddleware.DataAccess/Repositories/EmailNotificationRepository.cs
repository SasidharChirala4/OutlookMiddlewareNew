using Edreams.Common.DataAccess;
using Edreams.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class EmailNotificationRepository : Repository<EmailNotification>
    {
        public EmailNotificationRepository(
                   OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
                   : base(dbContext, dbContext.EmailNotifications, securityContext)
        {

        }
    }
}
