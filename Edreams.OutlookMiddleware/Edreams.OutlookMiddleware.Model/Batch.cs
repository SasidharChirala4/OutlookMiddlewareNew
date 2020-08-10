using System;
using Edreams.OutlookMiddleware.Model.Base;
using Edreams.OutlookMiddleware.Model.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class Batch : ModelBase, ILongSysId
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public BatchStatus Status { get; set; }
    }
}