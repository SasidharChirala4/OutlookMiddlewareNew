using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Edreams.Common.Exceptions.Constants;

namespace Edreams.OutlookMiddleware.Common.Constants
{
    public class EdreamsOutlookMiddlewareExceptionCode : EdreamsExceptionCode
    {
        [Description("The specified batch was not found.")]
        private static int _outlookMiddlewareBatchNotFound = 9001;
        public static Expression<Func<EdreamsOutlookMiddlewareExceptionCode, int>> OutlookMiddlewareBatchNotFound => _ => _outlookMiddlewareBatchNotFound;

        [Description("The upload to e-DReaMS has failed.")]
        private static int _outlookMiddlewareUploadToEdreamsFailed = 9101;
        public static Expression<Func<EdreamsOutlookMiddlewareExceptionCode, int>> OutlookMiddlewareUploadToEdreamsFailed => _ => _outlookMiddlewareUploadToEdreamsFailed;
    }
}