using System.Threading.Tasks;
using Edreams.Common.Logging.Interfaces;
using Edreams.Common.Security.Interfaces;
using Edreams.Common.Web;
using Edreams.Common.Web.Contracts;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    /// <summary>
    /// Group of endpoints that work with asynchronous transactions.
    /// </summary>
    /// <remarks>
    /// Asynchronous transactions are used by the Outlook Middleware to process long-running tasks
    /// in the background. The Outlook Middleware uses Worker Services installed as Windows Services
    /// to perform these long-running processes. Transactions represent these long-running processes
    /// and information about those transactions can be retrieved using these endpoints.
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ApiController<TransactionsController, ITransactionQueueManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsController" /> class.
        /// </summary>
        /// <param name="transactionQueueManager">The transaction queue manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="securityContext">The security context.</param>
        public TransactionsController(
            ITransactionQueueManager transactionQueueManager, IEdreamsLogger<TransactionsController> logger, ISecurityContext securityContext)
            : base(transactionQueueManager, logger, securityContext) { }

        /// <summary>
        /// Gets some statistics about the Azure ServiceBus transaction queue.
        /// </summary>
        /// <remarks>
        /// This HTTP GET endpoint connects to the configured Azure ServiceBus transaction queue and
        /// retrieves statistics like number of active messages, number of scheduled messages and
        /// number of dead lettered messages.
        /// </remarks>
        [HttpGet("queue/statistics")]
        [SwaggerResponse(200, "Successfully returns a GetTransactionQueueStatisticsResponse object.", typeof(ApiResult<GetTransactionQueueStatisticsResponse>))]
        [SwaggerResponse(500, "An internal server error has occurred. This is not your fault.", typeof(ApiErrorResult))]
        public Task<IActionResult> GetTransactionQueueStatistics()
        {
            return ExecuteManager(manager => manager.GetTransactionQueueStatistics());
        }
    }
}