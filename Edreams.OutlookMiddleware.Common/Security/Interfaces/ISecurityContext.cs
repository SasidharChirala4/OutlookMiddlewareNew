using System;
using System.Security.Principal;

namespace Edreams.OutlookMiddleware.Common.Security.Interfaces
{
    /// <summary>
    /// Interface defining a security context for e-DReaMS.
    /// </summary>
    public interface ISecurityContext
    {
        /// <summary>
        /// Gets or sets the Windows user identity.
        /// </summary>
        WindowsIdentity UserIdentity { get; }

        /// <summary>
        /// Gets or sets the component description.
        /// </summary>
        string Component { get; set; }

        /// <summary>
        /// Gets the principal name of the current user.
        /// </summary>
        string PrincipalName { get; }

        /// <summary>
        /// Gets the Object SID of the current user.
        /// </summary>
        string ObjectSid { get; }

        /// <summary>
        /// Gets the group SIDs of the current user.
        /// </summary>
        string[] GroupSids { get; }

        /// <summary>
        /// Gets the correlation identifier for the current security context.
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// Gets the size of the request that has created this security context, if available.
        /// </summary>
        long? RequestSize { get; set; }

        /// <summary>
        /// Updates the correlationId for this SecurityContext.
        /// </summary>
        /// <remarks>
        /// To be used with care and for specific logging purposes only.
        /// </remarks>
        void RefreshCorrelationId();

        /// <summary>
        /// Updates the correlationId for this SecurityContext.
        /// </summary>
        /// <param name="correlationId">The specific correlation GUID to use.</param>
        /// <remarks>
        /// To be used with care and for specific logging purposes only.
        /// </remarks>
        void RefreshCorrelationId(Guid correlationId);

        /// <summary>
        /// Sets the user identity.
        /// </summary>
        /// <param name="identity">The user identity.</param>
        void SetUserIdentity(WindowsIdentity identity);
    }
}