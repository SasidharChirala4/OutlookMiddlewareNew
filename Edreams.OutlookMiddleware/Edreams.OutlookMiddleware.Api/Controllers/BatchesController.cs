using System;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BatchesController : ControllerBase
    {
        private readonly IBatchLogic _batchLogic;
        private readonly ILogger<BatchesController> _logger;

        public BatchesController(
            IBatchLogic batchLogic,
            ILogger<BatchesController> logger)
        {
            _batchLogic = batchLogic;
            _logger = logger;
        }

        [HttpPost("{batchId}/commit")]
        public async Task<IActionResult> CommitBatch(Guid batchId)
        {
            _logger.LogTrace("[API] CommitBatch...");

            CommitBatchRequest request = new CommitBatchRequest
            {
                BatchId = batchId
            };

            CommitBatchResponse response = await _batchLogic.CommitBatch(request);

            return Ok(response);
        }

        [HttpDelete("{batchId}/cancel")]
        public async Task<IActionResult> CancelBatch(Guid batchId)
        {
            _logger.LogTrace("[API] CommitBatch...");

            CancelBatchRequest request = new CancelBatchRequest
            {
                BatchId = batchId
            };

            CancelBatchResponse response = await _batchLogic.CancelBatch(request);

            return Ok(response);
        }
    }
}