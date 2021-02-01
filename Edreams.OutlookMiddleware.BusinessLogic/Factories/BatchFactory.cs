using System;
using Edreams.OutlookMiddleware.BusinessLogic.Factories.Interfaces;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;

namespace Edreams.OutlookMiddleware.BusinessLogic.Factories
{
    public class BatchFactory : IBatchFactory
    {
        private readonly ISecurityContext _securityContext;

        public BatchFactory(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }
        
        public Batch CreatePendingBatch()
        {
            return new Batch
            {
                CreatedOn = DateTime.UtcNow,
                CreatedBy = _securityContext.PrincipalName,
                Status = BatchStatus.Pending
            };
        }
    }
}