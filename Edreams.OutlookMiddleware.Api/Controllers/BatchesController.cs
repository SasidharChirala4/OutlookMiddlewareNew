using System;
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
    /// Group of endpoints that work with batches.
    /// </summary>
    /// <remarks>
    /// Batches of emails to be processed are introduced to make sure that the production database is not
    /// polluted with partial uploads from the e-DReaMS Outlook Plugin. The Outlook Plugin needs multiple
    /// calls to the Outlook Middleware API to set emails metadata and upload the actual binary files.
    /// In this process, multiple things can go wrong or the user can stop the process in the middle. All
    /// this partially uploaded email data will be stored in the pre-load database and will therefore not
    /// cause a negative effect on the production database that does the actual processing.
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    public class BatchesController : ApiController<BatchesController, IBatchManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchesController" /> class.
        /// </summary>
        /// <param name="batchManager">The batch manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="securityContext">The security context.</param>
        public BatchesController(
            IBatchManager batchManager, IEdreamsLogger<BatchesController> logger, ISecurityContext securityContext) 
            : base(batchManager, logger, securityContext) { }

        /// <summary>
        /// Commits the specified batch of files to be processed by the Outlook Middleware.
        /// </summary>
        /// <param name="batchId">
        /// The unique batch identifier that is shared by file-records that have previously been prepared by calling
        /// the 'mails' and 'files' endpoints.
        /// </param>
        /// <param name="request">Request body.</param>
        /// <remarks>
        /// This HTTP POST endpoint commits an open batch of files to be processed by the Outlook Middleware.
        /// A batch exists in the pre-load database if there are file-records available that share the same batch-id.
        /// Committing a batch moves all related file-records from the pre-load database to the production database
        /// and marks them ready for processing.
        /// </remarks>
        [HttpPost("{batchId}/commit")]
        [SwaggerResponse(200, "Successfully committed the specified batch of files to be processed by the Outlook Middleware.", typeof(ApiResult<CommitBatchResponse>))]
        [SwaggerResponse(404, "The specified batch does not exist and cannot be committed.", typeof(ApiResult))]
        [SwaggerResponse(500, "An internal server error has occurred. This is not your fault.", typeof(ApiErrorResult))]
        public Task<IActionResult> CommitBatch(Guid batchId, CommitBatchRequest request)
        {
            return ExecuteManager(x => x.CommitBatch(batchId, request));
        }

        /// <summary>
        /// Cancels the specified batch of files to be cleaned by the Outlook Middleware.
        /// </summary>
        /// <param name="batchId">
        /// The unique batch identifier that is shared by file-records that have previously been prepared by calling
        /// the 'mails' and 'files' endpoints.
        /// </param>
        /// <param name="request">Request body.</param>
        /// <remarks>
        /// This HTTP DELETE endpoint cancels an open batch of files to be cleaned by the Outlook Middleware.
        /// A batch exists in the pre-load database if there are file-records available that share the same batch-id.
        /// Cancelling a batch changes the state of all related file-records and marks them ready for cleanup.
        /// </remarks>
        [HttpDelete("{batchId}/cancel")]
        [SwaggerResponse(200, "Successfully cancelled the specified batch of emails to be processed by the Outlook Middleware.", typeof(ApiResult<CancelBatchResponse>))]
        [SwaggerResponse(404, "The specified batch does not exist and cannot be cancelled.", typeof(ApiResult))]
        [SwaggerResponse(500, "An internal server error has occurred, this is not your fault.", typeof(ApiErrorResult))]
        public Task<IActionResult> CancelBatch(Guid batchId, CancelBatchRequest request)
        {
            return ExecuteManager(x => x.CancelBatch(batchId, request));
        }
    }
}