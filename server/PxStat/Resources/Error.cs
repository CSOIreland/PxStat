using System.Collections.Generic;
using API;
using FluentValidation.Results;

// Keep it under the PxStat name space because it's globally used
namespace PxStat
{
    /// <summary>
    /// Base Error class based on CRUD to extend/override as needed by the Application
    /// </summary>
    internal static class Error
    {
        /// <summary>
        /// Ge tValidation Failure
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        internal static IList<ErrorValidationFailure> GetValidationFailure(IList<ValidationFailure> errors)
        {
            var failures = new List<ErrorValidationFailure>();
            foreach (var error in errors)
            {
                failures.Add(new ErrorValidationFailure(error));
            }

            return failures;
        }
    }

    /// <summary>
    /// Container for Validation Failure
    /// </summary>
    public class ErrorValidationFailure
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validationFailure"></param>
        public ErrorValidationFailure(ValidationFailure validationFailure)
        {
            this.ErrorCode = validationFailure.ErrorCode;
            this.ErrorMessage = validationFailure.ErrorMessage;
            this.PropertyName = validationFailure.PropertyName;
        }

        /// <summary>
        /// Error Code
        /// </summary>
        public string ErrorCode { get; internal set; }

        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// Property Name
        /// </summary>
        public string PropertyName { get; internal set; }
    }
}