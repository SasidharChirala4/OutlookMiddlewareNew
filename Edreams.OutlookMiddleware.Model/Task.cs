using Edreams.Common.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edreams.OutlookMiddleware.Model
{
    public class Task : ModelBase
    {
        public string TaskName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public virtual IList<AssignedCc> AssignedCc { get; set; }
        
        public Email Email { get; set; }
        [InverseProperty("AssignedBy")]
        public virtual EmailAddress AssignedBy { get; set; }
        [InverseProperty("AssignedTo")]
        public virtual EmailAddress AssignedTo { get; set; }
    }
}
