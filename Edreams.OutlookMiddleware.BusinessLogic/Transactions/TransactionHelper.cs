using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;

namespace Edreams.OutlookMiddleware.BusinessLogic.Transactions
{
    public class TransactionHelper : ITransactionHelper
    {
        public ITransactionScope CreateScope()
        {
            return new TransactionScope();
        }
    }
}