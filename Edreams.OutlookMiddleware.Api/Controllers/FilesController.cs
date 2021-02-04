using System;
using System.IO;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Api.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Edreams.OutlookMiddleware.DataTransferObjects;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    /// <summary>
    /// Group of endpoints to handle different operations related to File(s) 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ApiController<IFileManager>
    {
        private readonly IEdreamsConfiguration _configuration;
        private readonly ILogger<FilesController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesController" /> class.
        /// </summary>
        /// <param name="fileManager"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public FilesController(
            IFileManager fileManager,
            ILogger<FilesController> logger,
            IEdreamsConfiguration configuration)
            : base(fileManager, logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Method to upload file to temporary location
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns>An ApiResult containing uploaded file details.</returns>
        /// <remarks>This HTTP POST operation uploads file to a temporary location.</remarks>
        [HttpPost("{fileId}")]
        [DisableRequestSizeLimit]
        [SwaggerResponse(200, "Successfully uploads the file to temporary location and return the uploaded object.", typeof(ApiResult<UpdateFileResponse>))]
        [SwaggerResponse(400, "There were validation errors while uplaoding file.", typeof(ApiErrorResult))]
        [SwaggerResponse(404, "File with specified Id does not exist.", typeof(ApiResult))]
        [SwaggerResponse(500, "An internal server error has occurred. This is not your fault.", typeof(ApiErrorResult))]
        public async Task<IActionResult> UploadFile(Guid fileId)
        {
            _logger.LogTrace("[API] File uploading...");
            string storagePath = _configuration.StoragePath;
            string tempPath = Path.Combine(storagePath, $"{fileId}");
            UpdateFileRequest request = new UpdateFileRequest()
            {
                FileId = fileId,
                TempPath = tempPath,
                FileName = Request.Form.Files[0].FileName,
                FileSize = Request.Form.Files[0].Length
            };

            //Store the file in the temporary location
            await using (FileStream fs = new FileStream(tempPath, FileMode.CreateNew))
            {
                await Request.Form.Files[0].CopyToAsync(fs);
            }
            
            return await ExecuteManager(manager => manager.UpdateFile(request, storagePath));
        }
    }
}