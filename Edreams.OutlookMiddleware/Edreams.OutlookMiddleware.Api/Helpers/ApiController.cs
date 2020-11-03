using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Api.Helpers
{
    public class ApiController : ControllerBase
    {
        protected Task<IActionResult> Success()
        {
            return Task.FromResult((IActionResult)Ok());
        }
    }

    public class ApiController<TManager> : ApiController
    {
        private readonly TManager _manager;
        private readonly ILogger _logger;

        public ApiController(TManager manager, ILogger logger)
        {
            _manager = manager;
            _logger = logger;
        }

        /// <summary>
        /// This method executes a method on the Manager that is associated with this ApiController<typeparamref name="TManager"/>.
        /// Based on the response from the Manager, it will return an IActionResult that is an HTTP status code like 200, 404, 500, ...
        /// </summary>
        /// <typeparam name="TResponse">The type of the response. This can be inferred from the <paramref name="managerCall"/></typeparam>
        /// <param name="managerCall">A delegate to the method on the Manager that needs to be called.</param>
        /// <returns>An IActionResult that contains the response data and the appropriate HTTP status code.</returns>
        protected async Task<IActionResult> ExecuteManager<TResponse>(Func<TManager, Task<TResponse>> managerCall) where TResponse : Response
        {
            // Call into the Manager class that is associated with this ApiController<TManager>
            // and record the time it takes in milliseconds using the .NET Stopwatch.
            Stopwatch stopwatch = Stopwatch.StartNew();
            TResponse response = await managerCall(_manager);
            stopwatch.Stop();

            // Wrap the response inside an ApiResult<TResponse> that also contains data like
            // the time it takes to execute the business logic and a correlationId.
            ApiResult result = new ApiResult<TResponse>(response);
            result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Return the result, wrapped in an HTTP 200 OK, or HTTP 404 Not Found if the response is null.
            return response != null ? Ok(result) : (IActionResult)NotFound();
        }
    }
}