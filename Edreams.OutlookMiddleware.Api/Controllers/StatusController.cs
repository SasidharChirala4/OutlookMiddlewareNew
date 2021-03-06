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
    /// Group of endpoints that are related to getting the status for this e-DReaMS Outlook Middleware HTTP API.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ApiController<StatusController, IStatusManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusController" /> class.
        /// </summary>
        /// <param name="statusManager">The status manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="securityContext">The security context.</param>
        public StatusController(
            IStatusManager statusManager, IEdreamsLogger<StatusController> logger, ISecurityContext securityContext) 
            : base(statusManager, logger, securityContext) { }

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