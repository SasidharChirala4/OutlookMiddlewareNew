using System;
using Edreams.Common.Web.Contracts;

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
        /// <example>4271</example>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// The version of the API used.
        /// </summary>
        /// <example>1.0.7599.26874</example>
        public string ApiVersion { get; set; }

        /// <summary>
        /// Timestamp when the request finished.
        /// </summary>
        /// <example>2020-12-04T13:03:59.346006Z</example>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// HTTP response status code.
        /// </summary>
        /// <example>200</example>
        public string StatusCode { get; set; }
    }

    /// <summary>
    /// Wrapper object used as common response type for all endpoints which include response data.
    /// </summary>
    public class ApiResult<TResponse> : ApiResult where TResponse : Response
    {
        /// <summary>
        /// The response data wrapped by this wrapper object.
        /// </summary>
        public TResponse ResponseData { get; set; }

        /// <summary>
        /// The type of the response data.
        /// </summary>
        /// <example>Response</example>
        public string ResponseType { get; } = $"{typeof(TResponse)}";

        /// <summary>
        /// Default constructor needed to support deserialization
        /// </summary>
        public ApiResult() { }

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

    /// <summary>
    /// Wrapper object used as common response type for all endpoints which return an HTTP 500 Internal Server Error.
    /// </summary>
    /// <seealso cref="ApiResult" />
    public class ApiErrorResult : ApiResult
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}