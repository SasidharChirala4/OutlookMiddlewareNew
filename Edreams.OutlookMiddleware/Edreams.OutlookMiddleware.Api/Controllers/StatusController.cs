using Edreams.OutlookMiddleware.Api.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Swashbuckle.AspNetCore.Annotations;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    /// <summary>
    /// Group of endpoints that are related to getting the status for this e-DReaMS Outlook Middleware HTTP API.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ApiController<IStatusManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusController" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="statusManager">The status manager.</param>
        public StatusController(ILogger<StatusController> logger, IStatusManager statusManager)
            : base(statusManager, logger) { }

        /// <summary>
        /// Get the status for this e-DReaMS Outlook Middleware HTTP API.
        /// </summary>
        /// <returns>This endpoint should always return an HTTP 200 OK. If it doesn't, there is something wrong.</returns>
        [HttpGet]
        [SwaggerResponse(200, "Successfully returns a GetStatusResponse object.", typeof(ApiResult<GetStatusResponse>))]
        public Task<IActionResult> Status()
        {
            return ExecuteManager(manager => manager.GetStatus());
        }
    }
}