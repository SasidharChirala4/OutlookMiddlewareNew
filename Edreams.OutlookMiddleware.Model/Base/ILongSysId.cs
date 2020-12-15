namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Interface defining a unique identifier.
    /// </summary>
    public interface ILongSysId
    {
        /// <summary>
        /// Private identifier; Clustered index; Auto-increment
        /// </summary>
        long SysId { get; set; }
    }
}