namespace Edreams.OutlookMiddleware.BusinessLogic.Transactions.Interfaces
{
    public interface ITransactionHelper
    {
        ITransactionScope CreateScope();
    }
}