using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Api.Helpers
{
    /// <summary>
    /// Base Api controller
    /// </summary>
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// Success Method
        /// </summary>
        /// <returns></returns>
        protected Task<IActionResult> Success()
        {
            return Task.FromResult((IActionResult)Ok());
        }
    }

    /// <summary>
    /// Base Api controller with TManager
    /// </summary>
    /// <typeparam name="TManager"></typeparam>
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
            return await Try(async () =>
            {
                _logger.LogTrace($"Executing business logic: {managerCall}");

                // Call into the Manager class that is associated with this ApiController<TManager>
                // and record the time it takes in milliseconds using the .NET Stopwatch.
                Stopwatch stopwatch = Stopwatch.StartNew();
                TResponse response = await managerCall(_manager);
                stopwatch.Stop();

                // Wrap the response inside an ApiResult<TResponse> that also contains data like
                // the time it takes to execute the business logic and a correlationId.
                ApiResult result = new ApiResult<TResponse>(response)
                {
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                };

                // Return the result, wrapped in an HTTP 200 OK, or HTTP 404 Not Found if the response is null.
                return response != null ? Ok(result) : (IActionResult)NotFound();
            });
        }

        private async Task<IActionResult> Try(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (EdreamsValidationException ex)
            {
                return BadRequest(ex.ValidationErrors);
            }
            catch (EdreamsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ActionResult(500, 0);
            }
        }

        private IActionResult ActionResult<T>(int status, Response value, long elapsedMilliseconds)
        {
            return StatusCode(status, new ApiResult<Response>(value)
            {
                CorrelationId = Guid.NewGuid(),
                StatusCode = $"{status}",
                TimeStamp = DateTime.UtcNow,
                ApiVersion = $"{Assembly.GetEntryAssembly()?.GetName().Version}",
                ElapsedMilliseconds = elapsedMilliseconds
            });
        }

        private IActionResult ActionResult(int status, long elapsedMilliseconds)
        {
            return StatusCode(status, new ApiResult
            {
                CorrelationId = Guid.NewGuid(),
                StatusCode = $"{status}",
                TimeStamp = DateTime.UtcNow,
                ApiVersion = $"{Assembly.GetEntryAssembly()?.GetName().Version}",
                ElapsedMilliseconds = elapsedMilliseconds
            });
        }
    }
}