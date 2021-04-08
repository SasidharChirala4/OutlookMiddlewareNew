using System.Diagnostics;
using Edreams.OutlookMiddleware.Api.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Edreams.Common.Logging.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
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
    public class StatusController : ApiController<StatusController, IStatusManager>
    {
        private readonly IEdreamsConfiguration _configuration;

        /// <summary>Initializes a new instance of the <see cref="StatusController" /> class.</summary>
        /// <param name="statusManager">The status manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        public StatusController(
            IStatusManager statusManager, IEdreamsLogger<StatusController> logger, IEdreamsConfiguration configuration)
            : base(statusManager, logger)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get the status for this e-DReaMS Outlook Middleware HTTP API.
        /// </summary>
        /// <returns>This endpoint should always return an HTTP 200 OK. If it doesn't, there is something wrong.</returns>
        [HttpGet]
        [SwaggerResponse(200, "Successfully returns a GetStatusResponse object.", typeof(ApiResult<GetStatusResponse>))]
        public Task<IActionResult> Status()
        {
            Debug.WriteLine(_configuration.EdreamsExtensibilityUrl);

            return ExecuteManager(manager => manager.GetStatus());
        }
    }
}