using Edreams.OutlookMiddleware.Enums;
using System;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class BatchDetailsDto
    {
        public Guid Id { get; set; }

        public List<EmailDetailsDto> Emails { get; set; }

        public EmailUploadOptions UploadOption { get; set; }
        public string UploadLocationSite { get; set; }
        public string UploadLocationFolder { get; set; }
        public string VersionComment { get; set; }
        public bool DeclareAsRecord { get; set; }

    }
}