using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface ICleanupManager
    {
        Task<int> ExpirePreloadedFiles();

        Task<int> ExpireTransactionHistory();

        Task<int> CleanupPreloadedFiles();

        Task<int> CleanupTransactions();
    }
}