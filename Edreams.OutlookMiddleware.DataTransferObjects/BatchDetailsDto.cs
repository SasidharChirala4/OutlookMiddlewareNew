using System;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public class BatchDetailsDto
    {
        public Guid Id { get; set; }

        public List<EmailDetailsDto> Emails { get; set; }

    }
}