using System;
using System.Collections.Generic;
using Edreams.Common.Exceptions;

namespace Edreams.OutlookMiddleware.DataAccess.Exceptions
{
    /// <summary>
    /// Exception class used for concurrency related exceptions.
    /// </summary>
    /// <seealso cref="EdreamsException" />
    public class EdreamsConcurrencyException : EdreamsException
    {
        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public List<string> ValidationErrors { get; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsConcurrencyException"/> class.
        /// </summary>
        /// <param name="validationError">The validation error.</param>
        /// <param name="arguments">The arguments.</param>
        public EdreamsConcurrencyException(string validationError, params object[] arguments)
        {
            ValidationErrors.Add(string.Format(validationError, arguments));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsConcurrencyException"/> class.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        public EdreamsConcurrencyException(params string[] validationErrors)
        {
            ValidationErrors.AddRange(validationErrors);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, ValidationErrors);
        }
    }
}