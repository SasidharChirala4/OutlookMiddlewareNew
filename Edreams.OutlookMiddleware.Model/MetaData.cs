using Edreams.Common.DataAccess.Model;
using Edreams.Common.DataAccess.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edreams.OutlookMiddleware.Model
{
    public  class Metadata : ModelBase, ILongSysId
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public Guid FileId { get; set; }
        public virtual File File { get; set; }
    }
}
