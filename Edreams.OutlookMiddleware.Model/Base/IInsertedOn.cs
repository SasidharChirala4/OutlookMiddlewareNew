using System;

namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Interface defining a property to know when was inserted.
    /// </summary>
    public interface IInsertedOn
    {
        /// <summary>
        /// Creation date and timestamp; Default = GETDATE();
        /// </summary>        
        DateTime InsertedOn { get; set; }
    }
}