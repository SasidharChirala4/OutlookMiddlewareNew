using System;

namespace Edreams.OutlookMiddleware.Common.Exceptions.Interfaces
{
    public interface IExceptionFactory
    {
        Exception CreateFromCode(EdreamsExceptionCode code, Exception innerException);
    }
}
