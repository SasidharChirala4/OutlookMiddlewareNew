using System;
using System.Collections.Generic;

namespace Edreams.OutlookMiddleware.Common.Exceptions
{
    /// <summary>
    /// Exception class used for validation related exceptions.
    /// </summary>
    /// <seealso cref="Edreams.Common.Exceptions.EdreamsException" />
    public class EdreamsValidationException : EdreamsException
    {
        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public List<string> ValidationErrors { get; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsValidationException"/> class.
        /// </summary>
        /// <param name="validationError">The validation error.</param>
        /// <param name="arguments">The arguments.</param>
        public EdreamsValidationException(string validationError, params object[] arguments)
        {
            ValidationErrors.Add(string.Format(validationError, arguments));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsValidationException"/> class.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        public EdreamsValidationException(params string[] validationErrors)
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
