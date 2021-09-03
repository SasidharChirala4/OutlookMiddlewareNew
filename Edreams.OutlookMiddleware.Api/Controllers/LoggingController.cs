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
    /// Group of endpoints that are helpful to Logs a specified message or error..
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LoggingController : ApiController<LoggingController, ILoggingManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingController" /> class.
        /// </summary>
        /// <param name="loggingManager">The logging manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="securityContext">The security context.</param>
        public LoggingController(
            ILoggingManager loggingManager, IEdreamsLogger<LoggingController> logger, ISecurityContext securityContext) 
            : base(loggingManager, logger, securityContext) { }

        /// <summary>
        /// Logs a specified message or error.
        /// </summary>
        /// <remarks>This HTTP POST operation records an informational message or error to the logging infrastructure.</remarks>
        [HttpPost]
        [SwaggerResponse(200, "Successfully returns a RecordLogResponse object.", typeof(ApiResult<RecordLogResponse>))]
        [SwaggerResponse(500, "An internal server error has occurred. This is not your fault.", typeof(ApiErrorResult))]
        public Task<IActionResult> RecordLog(RecordLogRequest log)
        {
            return ExecuteManager(manager => manager.RecordLog(log));
        }
    }
}