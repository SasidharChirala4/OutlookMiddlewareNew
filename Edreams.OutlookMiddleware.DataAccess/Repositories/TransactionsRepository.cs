using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class TransactionsRepository : Repository<Transaction>
    {
        public TransactionsRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.TransactionQueue, securityContext)
        {

        }
    }
}