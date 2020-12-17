using System;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    public class UpdateFileRequest : Request
    {
        public Guid FileId { get; set; }
        public string TempPath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
    }
}