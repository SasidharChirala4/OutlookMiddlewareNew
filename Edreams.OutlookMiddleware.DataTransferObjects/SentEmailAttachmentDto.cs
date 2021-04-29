using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class SentEmailAttachmentDto
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }
}
