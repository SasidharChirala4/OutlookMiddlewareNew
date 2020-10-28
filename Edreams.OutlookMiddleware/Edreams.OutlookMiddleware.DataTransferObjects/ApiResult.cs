using Edreams.OutlookMiddleware.DataTransferObjects.Api.Base;
using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects
{
    public interface IApiResult
    {
        Guid CorrelationId { get; set; }
        long ElapsedMilliseconds { get; set; }
    }

    public class ApiResult : IApiResult
    {
        public Guid CorrelationId { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }

    public class ApiResult<TResponse> : ApiResult where TResponse : Response
    {
        public TResponse ResponseData { get; }

        public string ResponseType { get; } = $"{typeof(TResponse)}";

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