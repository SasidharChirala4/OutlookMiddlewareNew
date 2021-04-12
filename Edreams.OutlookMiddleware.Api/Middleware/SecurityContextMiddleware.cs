using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Security.Principal;
using System.Threading.Tasks;

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
            _securityContext.RefreshCorrelationId();

            LogContext.PushProperty("CorrelationId", _securityContext.CorrelationId);
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