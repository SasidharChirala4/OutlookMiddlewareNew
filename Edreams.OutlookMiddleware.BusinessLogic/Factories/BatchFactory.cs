using System;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.BusinessLogic.Factories
{
    public class BatchFactory : IBatchFactory
    {
        public Batch CreatePendingBatch()
        {
            return new Batch
            {
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "CREATED",
                Status = BatchStatus.Pending
            };
        }
    }
}