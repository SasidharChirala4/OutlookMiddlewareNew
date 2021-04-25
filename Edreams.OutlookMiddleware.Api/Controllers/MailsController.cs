using System.Threading.Tasks;
using Edreams.Common.Logging.Interfaces;
using Edreams.Common.Web;
using Edreams.Common.Web.Contracts;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    public class MailsController : ApiController<MailsController, IEmailManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailsController"/> class.
        /// </summary>
        /// <param name="emailManager">The email manager.</param>
        /// <param name="logger">The logger.</param>
        public MailsController(
            IEmailManager emailManager,
            IEdreamsLogger<MailsController> logger)
            : base(emailManager, logger) { }

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
        [SwaggerResponse(200, "Successfully created mail by Outlook Middleware.", typeof(ApiResult<CreateMailResponse>))]
        [SwaggerResponse(500, "An internal server error has occurred. This is not your fault.", typeof(ApiErrorResult))]
        public Task<IActionResult> CreateMail(CreateMailRequest request)
        {
            return ExecuteManager(x => x.CreateMail(request));
        }
    }
}