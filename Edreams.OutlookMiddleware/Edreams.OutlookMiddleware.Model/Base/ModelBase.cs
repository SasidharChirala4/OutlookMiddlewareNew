using System;

namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Base class for all model classes containing common properties.
    /// </summary>
    public class ModelBase
    {
        /// <summary>
        /// Public identifier; Primary Key
        /// </summary>
        public Guid Id { get; set; }

        public long SysId { get; set; }
    }
}