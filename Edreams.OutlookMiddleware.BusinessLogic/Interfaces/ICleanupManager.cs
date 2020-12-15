using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface ICleanupManager
    {
        Task<int> VerifyExpiration();

        Task<int> Cleanup();
    }
}