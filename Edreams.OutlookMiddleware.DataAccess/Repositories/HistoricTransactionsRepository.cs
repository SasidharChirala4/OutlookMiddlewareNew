using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.DataAccess.Repositories
{
    public class HistoricTransactionsRepository : Repository<HistoricTransaction>
    {
        public HistoricTransactionsRepository(
            OutlookMiddlewareDbContext dbContext, ISecurityContext securityContext)
            : base(dbContext, dbContext.TransactionHistory, securityContext)
        {

        }
    }
}