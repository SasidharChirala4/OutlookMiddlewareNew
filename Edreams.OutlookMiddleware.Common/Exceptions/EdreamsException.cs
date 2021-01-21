﻿using System;
using System.Runtime.Serialization;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    /// <summary>
    /// Represents errors that occur during e-DReaMS application execution.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class EdreamsException : Exception
    {
        public EdreamsExceptionCode Code { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsException"/> class.
        /// </summary>
        public EdreamsException()
        {

        }

        /// <summary>Initializes a new instance of the <see cref="EdreamsException" /> class.</summary>
        /// <param name="code">The exception code.</param>
        public EdreamsException(EdreamsExceptionCode code)
        {
            Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EdreamsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsException"/> class.
        /// </summary>
        /// <param name="code">The exception code.</param>
        /// <param name="message">The message that describes the error.</param>
        public EdreamsException(EdreamsExceptionCode code, string message) : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public EdreamsException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsException"/> class.
        /// </summary>
        /// <param name="code">The exception code.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public EdreamsException(EdreamsExceptionCode code, string message, Exception innerException) : base(message,
            innerException)
        {
            Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected EdreamsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}