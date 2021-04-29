using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using System;
using System.Threading.Tasks;

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

        /// <summary>
        /// Find the email in SharedMailBox by edreamsReferenceId
        /// </summary>
        /// <param name="edreamsReferenceId">unique id of sent email</param>
        /// <returns></returns>
        Task<SentEmailDto> GetSharedMailBoxEmail(Guid edreamsReferenceId);
    }
}