using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    public interface ICleanupLogic
    {
        Task<int> VerifyExpiration();

        Task<int> Cleanup();
    }
}