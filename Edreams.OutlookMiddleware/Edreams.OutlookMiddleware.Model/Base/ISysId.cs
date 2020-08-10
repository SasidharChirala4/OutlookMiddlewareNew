namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Interface defining a unique identifier.
    /// </summary>
    public interface ISysId
    {
        /// <summary>
        /// Private identifier; Clustered index; Auto-increment
        /// </summary>
        int SysId { get; set; }
    }
}