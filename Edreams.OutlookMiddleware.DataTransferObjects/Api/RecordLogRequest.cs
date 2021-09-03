using Edreams.Common.Web.Contracts;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// DataRequest object for the RecordLog endpoint.
    /// </summary>
    public class RecordLogRequest : Request
    {
        /// <summary>
        /// Source
        /// </summary>
        /// <example>Info</example>
        public string Level { get; set; }

        /// <summary>
        /// Component Name
        /// </summary>
        /// <example>Edreams.Web.Api.Outlook.Middleware</example>
        public string Component { get; set; }

        /// <summary>
        /// log message
        /// </summary>
        /// <example>Log Message</example>
        public string Message { get; set; }

        /// <summary>
        /// exception details
        /// </summary>
        /// <example>Complete exception details</example>
        public string ExceptionDetails { get; set; }

        /// <summary>
        /// exception type
        /// </summary>
        /// <example>System.Exception</example>
        public string ExceptionType { get; set; }

        /// <summary>
        /// execution step details
        /// </summary>
        /// <example>SetCategorization</example>
        public string ExecutionStep { get; set; }

        /// <summary>
        /// Method path of log raised
        /// </summary>
        /// <example>Edreams.Plugin.Outlook.Services.MailCategoryService</example>
        public string MethodPath { get; set; }
    }
}
