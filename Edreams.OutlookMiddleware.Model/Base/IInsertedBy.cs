namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Interface defining a property to know who has inserted.
    /// </summary>
    public interface IInsertedBy
    {
        /// <summary>
        /// Inserted by user; 
        /// </summary>
        string InsertedBy { get; set; }
    }
}