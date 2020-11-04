using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    /// <summary>
    /// Interface for LoggingManager
    /// </summary>
    public interface ILoggingManager
    {
        Task<RecordLogResponse> RecordLog(RecordLogRequest log);
    }
}
