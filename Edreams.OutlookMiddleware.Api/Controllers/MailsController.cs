using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    /// <summary>
    /// Group of endpoints that work with emails.
    /// </summary>
    /// <remarks>
    /// Emails need to be uploaded from the e-DReaMS Outlook Plugin to the Outlook Middleware. To optimize
    /// the flow and performance of the Outlook Middleware processing: emails are uploaded as files using
    /// a flat table in a pre-load database.
    /// Binary files need to be uploaded from the Outlook Plugin to the Outlook Middleware by using HTTP
    /// and binary data streaming. Because of this, email and file metadata cannot be sent as part of the
    /// binary stream and this information must be set separately.
    /// The email endpoints are used to prepare the pre-load database for binary data upload and to add
    /// additional metadata to these emails.
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    public class MailsController : ControllerBase
    {
        private readonly IEmailManager _emailLogic;
        private readonly ILogger<MailsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailsController"/> class.
        /// </summary>
        /// <param name="emailLogic">The email logic.</param>
        /// <param name="logger">The logger.</param>
        public MailsController(
            IEmailManager emailLogic,
            ILogger<MailsController> logger)
        {
            _emailLogic = emailLogic;
            _logger = logger;
        }

        /// <summary>
        /// Creates an email to prepare for uploading binary data.
        /// </summary>
        /// <param name="request">
        /// A representation of a single email and optional attachments.
        /// </param>
        /// <remarks>
        /// This HTTP POST endpoint creates an email in the pre-load database by storing a flat list of files.
        /// Each resulting file-record has a relation to a single batch and a single email.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateMail(CreateMailRequest request)
        {
            _logger.LogTrace("[API] File uploading...");

            var response = await _emailLogic.CreateMail(request);

            return Ok(response);
        }
    }
}