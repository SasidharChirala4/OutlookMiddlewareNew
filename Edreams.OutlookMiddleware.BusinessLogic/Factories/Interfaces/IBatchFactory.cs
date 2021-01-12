using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces
{
    public interface IBatchFactory
    {
        Batch CreatePendingBatch();
    }
}