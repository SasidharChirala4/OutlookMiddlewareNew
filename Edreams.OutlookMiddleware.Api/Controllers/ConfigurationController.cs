using Edreams.OutlookMiddleware.Api.Helpers;
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
    /// API controller containing Get operation of the mail resource
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ApiController<IConfigurationManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationController" /> class.
        /// </summary>
        /// <param name="configurationManager">The configuration service.</param>
        /// <param name="logger">The logger.</param>
        public ConfigurationController(IConfigurationManager configurationManager, ILogger<ConfigurationController> logger)
            : base(configurationManager, logger) { }

        /// <summary>
        /// Gets the Outlook Middleware shared mailbox.
        /// </summary>
        /// <returns>
        /// The Outlook Middleware shared mailbox, if available. <see cref="System.String.Empty"/> otherwise.
        /// This endpoint should always return an HTTP 200 OK. If it doesn't, there is something wrong.
        /// </returns>
        /// <remarks>
        /// This endpoint will return the Outlook Middleware mailbox.
        /// The Outlook Middleware Web API will run using a service account
        /// and this endpoint will return the corresponding mailbox identifier.
        /// </remarks>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(ApiResult), Description = "Successfully returns a GetSharedMailBox object.")]
        public async Task<IActionResult> GetSharedMailBox()
        {
            return await ExecuteManager(manager => manager.GetSharedMailBox());
        }

    }
}
