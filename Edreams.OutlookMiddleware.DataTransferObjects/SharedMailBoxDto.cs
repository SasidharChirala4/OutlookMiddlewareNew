using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class SharedMailBoxDto
    {

        private static readonly SharedMailBoxDto _notFound = new SharedMailBoxDto { IsFound = false };

        public static SharedMailBoxDto NotFound { get { return _notFound; } }

        public bool IsFound { get; set; }

        public string Subject { get; set; }

        public string InternetMessageId { get; set; }

        public string EwsId { get; set; }

        public byte[] Data { get; set; }

        public List<SharedMailBoxAttachmentDto> Attachments { get; set; }

        public SharedMailBoxDto()
        {
            Attachments = new List<SharedMailBoxAttachmentDto>();
        }

    }
}
