using System;
using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using Edreams.OutlookMiddleware.Enums;

namespace Edreams.OutlookMiddleware.Model
{
    public class Batch : ModelBase, ILongSysId
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public BatchStatus Status { get; set; }
    }
}