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
    /// API controller containing Get operation of the mail resource
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ApiController<ConfigurationController, IConfigurationManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationController" /> class.
        /// </summary>
        /// <param name="configurationManager">The configuration service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="securityContext">The security context.</param>
        public ConfigurationController(
            IConfigurationManager configurationManager, IEdreamsLogger<ConfigurationController> logger, ISecurityContext securityContext) 
            : base(configurationManager, logger, securityContext) { }

        /// <summary>
        /// Gets the Outlook Middleware shared mailbox.
        /// </summary>
        /// <returns>
        /// The Outlook Middleware shared mailbox, if available. <see cref="string.Empty"/> otherwise.
        /// This endpoint should always return an HTTP 200 OK. If it doesn't, there is something wrong.
        /// </returns>
        /// <remarks>
        /// This endpoint will return the Outlook Middleware mailbox.
        /// The Outlook Middleware Web API will run using a service account
        /// and this endpoint will return the corresponding mailbox identifier.
        /// </remarks>
        [HttpGet("sharedmailbox")]
        [SwaggerResponse(200, "Successfully returns a GetSharedMailBox object.", typeof(ApiResult<GetSharedMailBoxResponse>))]
        [SwaggerResponse(500, "An internal server error has occurred. This is not your fault.", typeof(ApiErrorResult))]
        public Task<IActionResult> GetSharedMailBox()
        {
            return ExecuteManager(manager => manager.GetSharedMailBox());
        }
    }
}