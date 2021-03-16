using Edreams.OutlookMiddleware.Enums;
using System;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class EmailDetailsDto
    {
        public Guid Id { get; set; }

        public List<FileDetailsDto> Files { get; set; }

        public EmailUploadOptions UploadOption { get; set; }
    }
}