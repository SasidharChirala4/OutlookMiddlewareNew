using Edreams.OutlookMiddleware.Api.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ApiController<IStatusManager>
    {
        public StatusController(ILogger<StatusController> logger, IStatusManager statusManager)
            : base(statusManager, logger) { }

        [HttpGet]
        public Task<IActionResult> Status()
        {
            return ExecuteManager(manager => manager.GetStatus());
        }
    }
}