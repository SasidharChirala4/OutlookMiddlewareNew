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
    public class MailsController : ControllerBase
    {
        private readonly IEmailLogic _emailLogic;
        private readonly ILogger<MailsController> _logger;

        public MailsController(
            IEmailLogic emailLogic,
            ILogger<MailsController> logger)
        {
            _emailLogic = emailLogic;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMail(CreateMailRequest request)
        {
            _logger.LogTrace("[API] File uploading...");

            var response = await _emailLogic.CreateMail(request);

            return Ok(response);
        }
    }
}