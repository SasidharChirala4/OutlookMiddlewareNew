using System;

namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Interface defining a property to know when was deleted.
    /// </summary>
    public interface IDeletedOn
    {
        /// <summary>
        /// Deleted date and timestamp
        /// </summary>
        DateTime? DeletedOn { get; set; }
    }
}