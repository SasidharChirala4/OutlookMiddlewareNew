using System;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    public class ExceptionFactory : IExceptionFactory
    {
        public Exception CreateFromCode(EdreamsExceptionCode code, Exception innerException)
        {
            string message = string.Empty;

            switch (code)
            {
                case EdreamsExceptionCode.UNKNOWN_FAULT:
                    break;
            }

            return new EdreamsException(code, message, innerException);
        }
    }
}