using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface ICleanupManager
    {
        /// <summary>
        /// Verify the preloaded files and changes the status to expired for the old ones.
        /// </summary>
        /// <returns>The number of preloaded files that were marked as expired.</returns>
        Task<int> ExpirePreloadedFiles();

        /// <summary>
        /// Verify the historic transactions and changes the status to expired for the old ones.
        /// </summary>
        /// <returns>The number of historic transactions that were marked as expired.</returns>
        Task<int> ExpireTransactions();

        /// <summary>
        /// Removes the oldest preloaded files that share a unique batch ID that were marked as expired.
        /// </summary>
        /// <returns>The number of preloaded files that were removed.</returns>
        Task<int> CleanupPreloadedFiles();

        /// <summary>
        /// Removes the oldest historic transaction that was marked as expired.
        /// </summary>
        /// <returns>The number of historic transactions and their related batches, mails and files that were removed.</returns>
        Task<int> CleanupTransactions();
    }
}