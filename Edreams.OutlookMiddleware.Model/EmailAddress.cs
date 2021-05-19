using Edreams.Common.DataAccess.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edreams.OutlookMiddleware.Model
{
    public class EmailAddress : ModelBase
    {
        public string Email { get; set; }
        public string UserId { get; set; }
        public string PrincipalName { get; set; }
        public string DisplayName { get; set; }
        public virtual IList<AssignedCc> AssignedCc { get; set; }
        [InverseProperty("AssignedBy")]
        public virtual IList<Task> AssignedBy { get; set; }
        [InverseProperty("AssignedTo")]
        public virtual IList<Task> AssignedTo { get; set; }
    }
}
