using System;

namespace Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces
{
    public interface ITransactionScope : IDisposable
    {
        void Commit();
    }
}