using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Edreams.OutlookMiddleware.Common.Exceptions.Interfaces;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    public class ExceptionFactory : IExceptionFactory
    {
        public Exception CreateFromCode(EdreamsExceptionCode code, Exception innerException, params object[] args)
        {
            string message = string.Empty;

            FieldInfo fieldInfo = typeof(EdreamsExceptionCode).GetField($"{code}");
            if (fieldInfo != null && fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Length == 1)
            {
                message = string.Format(attributes.First().Description, args);
            }

            return new EdreamsException(code, message, innerException);
        }
    }
}