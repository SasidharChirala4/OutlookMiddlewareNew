using Edreams.OutlookMiddleware.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Services.Categorization.Engine.Interfaces
{
    public interface ICategorizationEngineProcessor
    {
        Task Process(TransactionMessage transactionMessage);
    }
}
