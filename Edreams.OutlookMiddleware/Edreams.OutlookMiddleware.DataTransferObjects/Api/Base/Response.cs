using System;

namespace Edreams.OutlookMiddleware.DataTransferObjects.Api.Base
{
    /// <summary>
    /// Base class for response objects.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Unique ID you can use for correlation purposes.
        /// </summary>
        /// <example>4ad4eb08-a9e3-4950-8a52-0ff09cfac6d8</example>
        public Guid CorrelationId { get; set; }
    }
}