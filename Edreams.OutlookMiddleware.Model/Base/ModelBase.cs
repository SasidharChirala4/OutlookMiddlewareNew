using System;

namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Base class for all model classes containing common properties.
    /// </summary>
    public class ModelBase : IInsertedBy, IInsertedOn, IUpdatedBy, IUpdatedOn
    {
        /// <summary>
        /// Public identifier; Primary Key
        /// </summary>
        public Guid Id { get; set; }
        public long SysId { get; set; }
        public DateTime InsertedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string InsertedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}