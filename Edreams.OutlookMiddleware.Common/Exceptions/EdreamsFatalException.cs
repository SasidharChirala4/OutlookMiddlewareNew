using System;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    /// <summary>
    /// Exception class used for fatal code-based exceptions.
    /// </summary>
    /// <seealso cref="EdreamsException" />
    [Serializable]
    public sealed class EdreamsFatalException : EdreamsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsFatalException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EdreamsFatalException(string message) : base(EdreamsExceptionCode.UNKNOWN_FAULT, message) { }
    }
}