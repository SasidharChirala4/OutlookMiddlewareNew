using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api
{
    /// <summary>
    /// DataRequest object for the RecordLog endpoint.
    /// </summary>
    public class RecordLogRequest
    {
        /// <summary>
        /// Public identifier; Primary Key
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// Creation date and timestamp; Default = GETUTCDATE(); 
        /// </summary>
        public DateTime InsertedOn { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        /// <example>Info</example>
        public string Level { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        /// <example>BE-PC0FP4E4</example>
        public string Host { get; set; }

        /// <summary>
        /// Logger
        /// </summary>
        /// <example>Edreams.Web.OutlookMiddlewareWebApi.Swagger</example>
        public string Component { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        /// <example>(Testing) Inserting Informative error with Swagger (can be ignored)</example>
        public string Message { get; set; }

        /// <summary>
        /// Resolve Message
        /// </summary>
        /// <example>Clean User Cache and try again</example>
        public string ResolveMessage { get; set; }

        /// <summary>
        /// Exception Type
        /// </summary>
        /// <example>System.NotImplementedException</example>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Exception Details
        /// </summary>
        /// <example>String representation of the Exception, with full StackTrace</example>
        public string ExceptionDetails { get; set; }

        /// <summary>
        /// Method Path
        /// </summary>
        /// <example>SomeController.GetNextActionToProcess</example>
        public string MethodPath { get; set; }

        /// <summary>
        /// Method Path
        /// </summary>
        /// <example>Testing Response with Swagger</example>
        public string ExecutionStep { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        /// <example>be\\saschirala</example>
        public string Username { get; set; }

        /// <summary>
        /// SharePoint Correlation
        /// </summary>
        public Guid? SharePointCorrelation { get; set; }

        /// <summary>
        /// Product Correlation
        /// </summary>
        public Guid? ProductCorrelation { get; set; }

        /// <summary>
        /// Object Identifier
        /// </summary>
        /// <example>Mail Conversation</example>
        public string ObjectIdentifier { get; set; }

        /// <summary>
        /// Object Value
        /// </summary>
        /// <example>CE407CAD6A9348B39DB2DC253D0429A4</example>
        public string ObjectValue { get; set; }
    }
}
