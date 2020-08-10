namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Interface defining a property to know who has updated.
    /// </summary>
    public interface IUpdatedBy
    {
        /// <summary>
        /// Updated by user; 
        /// </summary>
        string UpdatedBy { get; set; }
    }
}