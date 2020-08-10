namespace Edreams.OutlookMiddleware.Model.Base
{
    /// <summary>
    /// Interface defining a rowversion for optimistic concurrency.
    /// </summary>
    public interface IRowVersion
    {
        /// <summary>
        /// Field to verify concurrency.
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}