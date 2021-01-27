using System;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model.Base;

namespace Edreams.OutlookMiddleware.Model
{
    public class Batch : ModelBase, ILongSysId
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public BatchStatus Status { get; set; }
    }
}