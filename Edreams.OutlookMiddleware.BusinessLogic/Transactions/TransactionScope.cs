using System.Transactions;
using Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces;

using SystemTransactionScope = System.Transactions.TransactionScope;

namespace Edreams.OutlookMiddleware.BusinessLogic.Transactions
{
    public class TransactionScope : ITransactionScope
    {
        public SystemTransactionScope Transaction { get; }

        public TransactionScope()
        {
            Transaction = new SystemTransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        }

        public void Commit()
        {
            Transaction.Complete();
        }

        public void Dispose()
        {
            Transaction.Dispose();
        }
    }
}