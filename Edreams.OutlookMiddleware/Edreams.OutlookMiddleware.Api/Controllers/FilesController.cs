using System;
using System.IO;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Edreams.OutlookMiddleware.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileManager _fileLogic;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IFileManager fileLogic,
            ILogger<FilesController> logger)
        {
            _fileLogic = fileLogic;
            _logger = logger;
        }

        [HttpPut("{fileId}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(Guid fileId)
        {
            _logger.LogTrace("[API] File uploading...");

            string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}");
            using (FileStream fs = new FileStream(tempPath, FileMode.CreateNew))
            {
                await Request.Form.Files[0].CopyToAsync(fs);
            }

            UpdateFileRequest updateFileRequest = new UpdateFileRequest
            {
                FileId = fileId,
                TempPath = tempPath,
                FileName = Request.Form.Files[0].FileName,
                FileSize = Request.Form.Files[0].Length
            };

            await _fileLogic.UpdateFile(updateFileRequest);

            return Ok();
        }
    }
}