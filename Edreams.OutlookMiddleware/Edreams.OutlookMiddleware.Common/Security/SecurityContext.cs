using System;
using System.Linq;
using System.Security.Principal;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;

namespace Edreams.OutlookMiddleware.Common.Security
{
    public class SecurityContext : ISecurityContext
    {
        private Guid _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityContext"/> class.
        /// </summary>
        public SecurityContext()
        {
            RefreshCorrelationId();
        }

        /// <summary>
        /// Gets or sets the Windows user identity.
        /// </summary>
        public WindowsIdentity UserIdentity { get; private set; }

        /// <summary>
        /// Gets or sets the component description.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets the Object SID of the current user.
        /// </summary>
        public string ObjectSid { get; private set; }

        /// <summary>
        /// Gets the group SIDs of the current user.
        /// </summary>
        public string[] GroupSids { get; private set; }

        /// <summary>
        /// Gets the principal name of the current user.
        /// Return "Anonymous" if no Windows user identity is available.
        /// </summary>
        public string PrincipalName { get; private set; }

        /// <summary>
        /// Gets the correlation identifier for the current security context.
        /// </summary>
        public Guid CorrelationId => _correlationId;

        /// <summary>
        /// Gets the size of the request that has created this security context, if available.
        /// </summary>
        public long? RequestSize { get; set; }

        /// <summary>
        /// Updates the correlationId for this SecurityContext.
        /// </summary>
        /// <remarks>
        /// To be used with care and for specific logging purposes only.
        /// </remarks>
        public void RefreshCorrelationId()
        {
            _correlationId = Guid.NewGuid();
        }

        /// <summary>
        /// Updates the correlationId for this SecurityContext.
        /// </summary>
        /// <param name="correlationId">The specific correlation GUID to use.</param>
        /// <remarks>
        /// To be used with care and for specific logging purposes only.
        /// </remarks>
        public void RefreshCorrelationId(Guid correlationId)
        {
            _correlationId = correlationId;
        }

        /// <summary>
        /// Sets the user identity.
        /// </summary>
        /// <param name="identity">The user identity.</param>
        public void SetUserIdentity(WindowsIdentity identity)
        {
            UserIdentity = identity;
            ObjectSid = identity?.User?.Value;
            GroupSids = identity?.Groups?.Select(x => x.Value).ToArray();
            PrincipalName = string.IsNullOrEmpty(UserIdentity?.Name) ? "Anonymous" : UserIdentity.Name;
        }
    }
}