using Edreams.OutlookMiddleware.Common.Exceptions;
using Edreams.OutlookMiddleware.Common.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Validation
{
    /// <summary>
    /// Class implementing different validation methods.
    /// </summary>
    public class Validator : IValidator
    {
        /// <summary>
        /// Validates a GUID.
        /// </summary>
        /// <param name="toValidate">The GUID to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void ValidateGuid(Guid toValidate, string validationMessage)
        {
            if (toValidate == Guid.Empty)
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Validates a nullable GUID.
        /// </summary>
        /// <param name="toValidate">The nullable GUID to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void ValidateGuid(Guid? toValidate, string validationMessage)
        {
            if (toValidate.HasValue && toValidate == Guid.Empty)
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Validates a string.
        /// </summary>
        /// <param name="toValidate">The string to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void ValidateString(string toValidate, string validationMessage)
        {
            if (string.IsNullOrWhiteSpace(toValidate))
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }
        /// <summary>
        /// Validates a list of string values
        /// </summary>
        /// <param name="toValidate"></param>
        /// <param name="validationMessage"></param>
        public void ValidateList(List<string> toValidate, string validationMessage) 
        {
            if (toValidate.Count == 0) 
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }
        /// <summary>
        /// Validates a datetime.
        /// </summary>
        /// <param name="toValidate">The datetime to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void ValidateDateTime(DateTime toValidate, string validationMessage)
        {
            if (toValidate == DateTime.MinValue || toValidate == DateTime.MaxValue)
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Validates a datetime.
        /// </summary>
        /// <param name="toValidate">The datetime to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void ValidateDateTime(DateTime? toValidate, string validationMessage)
        {
            if (toValidate.HasValue && (toValidate == DateTime.MinValue || toValidate == DateTime.MaxValue))
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Validates an object.
        /// </summary>
        /// <param name="toValidate">The object to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void ValidateObject(object toValidate, string validationMessage)
        {
            if (toValidate == null)
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Validates the ENUM based on its numeric value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the ENUM.</typeparam>
        /// <param name="enumValue">The numeric ENUM value.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void ValidateEnum<TEnum>(int enumValue, string validationMessage)
        {
            if (!Enum.IsDefined(typeof(TEnum), enumValue))
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Validates the ENUM value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the ENUM.</typeparam>
        /// <param name="enumValue">The ENUM value.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void ValidateEnum<TEnum>(TEnum enumValue, string validationMessage)
        {
            if (!Enum.IsDefined(typeof(TEnum), enumValue))
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The GUID to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void IsRequired(Guid toValidate, string validationMessage)
        {
            if (toValidate == Guid.Empty)
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The GUID to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void IsRequired(Guid? toValidate, string validationMessage)
        {
            if (toValidate == null || toValidate.Value == Guid.Empty)
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The string to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void IsRequired(string toValidate, string validationMessage)
        {
            if (string.IsNullOrWhiteSpace(toValidate))
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The datetime to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void IsRequired(DateTime toValidate, string validationMessage)
        {
            if (toValidate == DateTime.MinValue || toValidate == DateTime.MaxValue)
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The object to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        public void IsRequired(object toValidate, string validationMessage)
        {
            if (toValidate == null)
            {
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Performs validation using a custom validation expression.
        /// </summary>
        /// <param name="customValidation">The custom validation.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        public void Validate(Func<bool> customValidation, string validationMessage, params object[] arguments)
        {
            if (!customValidation())
            {
                validationMessage = string.Format(validationMessage, arguments);
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Performs validation using a custom validation expression.
        /// </summary>
        /// <param name="customValidation">The custom validation.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        public async Task Validate(Func<Task<bool>> customValidation, string validationMessage, params object[] arguments)
        {
            if (!await customValidation())
            {
                validationMessage = string.Format(validationMessage, arguments);
                throw new EdreamsValidationException(validationMessage);
            }
        }

        /// <summary>
        /// Performs validation using a custom validation expression and a generic exception type
        /// </summary>
        /// <param name="customValidation">The custom validation.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        public void Validate<T>(Func<bool> customValidation, string validationMessage, params object[] arguments) where T : EdreamsException
        {
            if (!customValidation())
            {
                throw (T)Activator.CreateInstance(typeof(T), validationMessage, arguments);
            }
        }

        /// <summary>
        /// Performs validation using a custom validation expression and a generic exception type
        /// </summary>
        /// <param name="customValidation">The custom validation.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        public async Task Validate<T>(Func<Task<bool>> customValidation, string validationMessage, params object[] arguments) where T : EdreamsException
        {
            if (!await customValidation())
            {
                throw (T)Activator.CreateInstance(typeof(T), validationMessage, arguments);
            }
        }

        /// <summary>
        /// Throws an EdreamsValidationException with the specified validation message.
        /// </summary>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        public Exception Exception(string validationMessage, params object[] arguments)
        {
            validationMessage = string.Format(validationMessage, arguments);
            return new EdreamsValidationException(validationMessage);
        }
    }
}
