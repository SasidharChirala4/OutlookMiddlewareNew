using Edreams.OutlookMiddleware.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edreams.OutlookMiddleware.Common.Validation.Interface
{
    /// <summary>
    /// Interface defining different validation methods.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates a GUID.
        /// </summary>
        /// <param name="toValidate">The GUID to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void ValidateGuid(Guid toValidate, string validationMessage);

        /// <summary>
        /// Validates a nullable GUID.
        /// </summary>
        /// <param name="toValidate">The nullable GUID to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void ValidateGuid(Guid? toValidate, string validationMessage);

        /// <summary>
        /// Validates a string.
        /// </summary>
        /// <param name="toValidate">The string to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void ValidateString(string toValidate, string validationMessage);

        /// <summary>
        /// Validates a list of string values
        /// </summary>
        /// <param name="toValidate"></param>
        /// <param name="validationMessage"></param>
        void ValidateList(List<string> toValidate, string validationMessage);

        /// <summary>
        /// Validates a datetime.
        /// </summary>
        /// <param name="toValidate">The datetime to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void ValidateDateTime(DateTime toValidate, string validationMessage);

        /// <summary>
        /// Validates a datetime.
        /// </summary>
        /// <param name="toValidate">The datetime to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void ValidateDateTime(DateTime? toValidate, string validationMessage);

        /// <summary>
        /// Validates an object.
        /// </summary>
        /// <param name="toValidate">The object to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void ValidateObject(object toValidate, string validationMessage);

        /// <summary>
        /// Validates the ENUM based on its numeric value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the ENUM.</typeparam>
        /// <param name="enumValue">The numeric ENUM value.</param>
        /// <param name="validationMessage">The validation message.</param>
        void ValidateEnum<TEnum>(int enumValue, string validationMessage);

        /// <summary>
        /// Validates the ENUM value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the ENUM.</typeparam>
        /// <param name="enumValue">The ENUM value.</param>
        /// <param name="validationMessage">The validation message.</param>
        void ValidateEnum<TEnum>(TEnum enumValue, string validationMessage);

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The GUID to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void IsRequired(Guid toValidate, string validationMessage);

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The GUID to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void IsRequired(Guid? toValidate, string validationMessage);

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The string to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void IsRequired(string toValidate, string validationMessage);

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The datetime to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void IsRequired(DateTime toValidate, string validationMessage);

        /// <summary>
        /// Determines whether the specified value to validate is valid.
        /// </summary>
        /// <param name="toValidate">The object to validate.</param>
        /// <param name="validationMessage">The validation message.</param>
        void IsRequired(object toValidate, string validationMessage);

        /// <summary>
        /// Performs validation using a custom validation expression.
        /// </summary>
        /// <param name="customValidation">The custom validation.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        void Validate(Func<bool> customValidation, string validationMessage, params object[] arguments);

        /// <summary>
        /// Performs validation using a custom validation expression.
        /// </summary>
        /// <param name="customValidation">The custom validation.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        Task Validate(Func<Task<bool>> customValidation, string validationMessage, params object[] arguments);

        /// <summary>
        /// Performs validation using a custom validation expression and a generic exception type
        /// </summary>
        /// <param name="customValidation">The custom validation.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        void Validate<T>(Func<bool> customValidation, string validationMessage, params object[] arguments)
            where T : EdreamsException;

        /// <summary>
        /// Performs validation using a custom validation expression and a generic exception type
        /// </summary>
        /// <param name="customValidation">The custom validation.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        Task Validate<T>(Func<Task<bool>> customValidation, string validationMessage, params object[] arguments)
            where T : EdreamsException;

        /// <summary>
        /// Builds an EdreamsValidationException with the specified validation message.
        /// </summary>
        /// <param name="validationMessage">The validation message.</param>
        /// <param name="arguments">The arguments for the validation message.</param>
        Exception Exception(string validationMessage, params object[] arguments);
    }
}
