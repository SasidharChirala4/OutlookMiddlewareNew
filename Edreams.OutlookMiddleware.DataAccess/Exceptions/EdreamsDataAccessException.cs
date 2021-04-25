using System;
using Edreams.Common.Exceptions;

namespace Edreams.OutlookMiddleware.DataAccess.Exceptions
{
    /// <summary>
    /// Represents DataAccess errors that occur during e-DReaMS application execution.
    /// </summary>
    /// <seealso cref="EdreamsException" />
    public class EdreamsDataAccessException : EdreamsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsDataAccessException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EdreamsDataAccessException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsDataAccessException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public EdreamsDataAccessException(Exception innerException)
            : base($"e-DReaMS DataAccess exception: {innerException.Message}", innerException) { }
    }
}