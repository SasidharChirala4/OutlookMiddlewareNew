using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class SharedMailBoxAttachmentDto
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }
}
