using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects;

namespace Edreams.OutlookMiddleware.Services.Upload.Engine.Interfaces
{
    public interface IUploadEngineProcessor
    {
        Task Process(TransactionMessage transactionMessage);
    }
}