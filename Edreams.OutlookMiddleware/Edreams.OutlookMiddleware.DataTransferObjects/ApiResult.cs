using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    /// <summary>
    /// Wrapper object used as common response type for all endpoints.
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// The CorrelationId copied from the response data, if any.
        /// </summary>
        /// <example>4ad4eb08-a9e3-4950-8a52-0ff09cfac6d8</example>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// The time needed to execute the actual business logic and generate the response.
        /// </summary>
        /// <example>42</example>
        public long ElapsedMilliseconds { get; set; }
    }

    /// <summary>
    /// Wrapper object used as common response type for all endpoints which include response data.
    /// </summary>
    public class ApiResult<TResponse> : ApiResult where TResponse : Response
    {
        /// <summary>
        /// The response data wrapped by this wrapper object.
        /// </summary>
        public TResponse ResponseData { get; }

        /// <summary>
        /// The type of the response data.
        /// </summary>
        /// <example>Response</example>
        public string ResponseType { get; } = $"{typeof(TResponse)}";

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResponse}" /> class.
        /// </summary>
        /// <param name="responseData">The response data that should be wrapped by this object.</param>
        public ApiResult(TResponse responseData)
        {
            ResponseData = responseData;
            if (responseData != null)
            {
                CorrelationId = responseData.CorrelationId;
            }
        }
    }
}