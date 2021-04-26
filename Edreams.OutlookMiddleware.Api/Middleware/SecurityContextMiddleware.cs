using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edreams.Common.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Edreams.OutlookMiddleware.Api.Middleware
{
    public class SecurityContextMiddleware : IMiddleware
    {
        private readonly ISecurityContext _securityContext;

        public SecurityContextMiddleware(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Guid correlationId = _securityContext.RefreshCorrelationId();
            LogContext.PushProperty("CorrelationId", correlationId);

            if (context.Request.HttpContext.User.Identity is WindowsIdentity identity)
            {
                _securityContext.SetUserIdentity(identity);
            }
            else
            {
                _securityContext.SetUserIdentity(WindowsIdentity.GetCurrent());
            }

            return next.Invoke(context);
        }
    }
}