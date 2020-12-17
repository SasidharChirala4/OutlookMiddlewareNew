using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// Response class for Update File
    /// </summary>
    public class UpdateFileResponse : Response
    {
        /// <summary>
        /// Preloaded File Unique Identifier
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// Temporary storage path of the file
        /// </summary>
        public string TempPath { get; set; }

        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName { get; set; }
    }
}