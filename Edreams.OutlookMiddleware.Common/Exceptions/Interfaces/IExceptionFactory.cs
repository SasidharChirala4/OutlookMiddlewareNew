using System;

namespace Edreams.OutlookMiddleware.Common.Exceptions.Interfaces
{
    public interface IExceptionFactory
    {
        EdreamsException CreateEdreamsExceptionFromCode(EdreamsExceptionCode code, params object[] args);

        EdreamsException CreateEdreamsExceptionFromCode(EdreamsExceptionCode code, Exception innerException, params object[] args);
    }
}
