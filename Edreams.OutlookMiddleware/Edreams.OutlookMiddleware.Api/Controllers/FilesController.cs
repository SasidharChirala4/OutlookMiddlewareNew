﻿using System;
using System.IO;
using System.Threading.Tasks;
using Edreams.OutlookMiddleware.Api.Helpers;
using Edreams.OutlookMiddleware.BusinessLogic;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        /// <returns></returns>
        [HttpPost("{fileId}")]
        [DisableRequestSizeLimit]
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
            FileStream fs = new FileStream(tempPath, FileMode.CreateNew);
            await Request.Form.Files[0].CopyToAsync(fs);
            fs.Close();
            
            return await ExecuteManager(manager => manager.UpdateFile(request, storagePath));
        }
    }
}