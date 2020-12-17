﻿using Edreams.OutlookMiddleware.Api.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    /// <summary>
    /// Group of endpoints that are helpful to Logs a specified message or error..
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LoggingController : ApiController<ILoggingManager>
    {
        /// <summary>Initializes a new instance of the <see cref="LoggingController" /> class.</summary>
        /// <param name="loggingManager">The logging manager.</param>
        /// <param name="logger">The logger.</param>        
        public LoggingController(ILogger<LoggingController> logger, ILoggingManager loggingManager)
            : base(loggingManager, logger) { }

        /// <summary>
        /// Logs a specified message or error.
        /// </summary>
        /// <remarks>This HTTP POST operation records an informational message or error to the logging infrastructure.</remarks>
        [HttpPost]
        [SwaggerResponse(200, "Successfully returns a RecordLogResponse object.", typeof(ApiResult<RecordLogResponse>))]
        public Task<IActionResult> RecordLog(RecordLogRequest log)
        {
            return ExecuteManager(manager => manager.RecordLog(log));
        }
    }
}