using System;

namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Interface defining a property to know when was updated.
    /// </summary>
    public interface IUpdatedOn
    {
        /// <summary>
        /// Update date and timestamp
        /// </summary>
        DateTime? UpdatedOn { get; set; }
    }
}