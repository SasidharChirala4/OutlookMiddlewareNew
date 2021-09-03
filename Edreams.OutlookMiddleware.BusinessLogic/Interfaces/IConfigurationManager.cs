using System.Threading.Tasks;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;

namespace Edreams.OutlookMiddleware.BusinessLogic.Interfaces
{
    /// <summary>
    /// Interface for Configuration Manager
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the Outlook Middleware shared mailbox.
        /// </summary>
        /// <returns>The Outlook Middleware shared mailbox, if available. <see cref="System.String.Empty"/> otherwise.</returns>
        Task<GetSharedMailBoxResponse> GetSharedMailBox();
    }
}