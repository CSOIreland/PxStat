using System;
using API;
using FluentValidation.Results;
using PxStat.Data.Px;

namespace PxStat.Data
{
    /// <summary>
    /// Validator for a Matix
    /// </summary>
    internal class MatrixValidator
    {

        internal ValidationResult MatrixValidatorResult { get; private set; }

        /// <summary>
        /// Validate method
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="includeSource" - set this to true if you want the Source (Copyright) field to be validated. Otherwise ignore.></param>
        /// <returns></returns>
        internal bool Validate(Matrix theMatrix, bool includeSource = true)
        {
            if (!PxSettingsAreValid(theMatrix, includeSource))
                return false;
            if (!PxIntegrityIsValid(theMatrix))
                return false;

            return true;

        }
        /// <summary>
        /// Test if languages, sources etc are supported in the system
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="includeSource"></param>
        /// <returns></returns>
        private bool PxSettingsAreValid(Matrix theMatrix, bool includeSource = true)
        {
            ADO ado = new ADO("defaultConnection");
            try
            {
                MatrixValidatorResult = new PxSettingsValidator(ado, includeSource).Validate(theMatrix);
                return MatrixValidatorResult.IsValid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ado.Dispose();
            }
        }

        /// <summary>
        /// Test if data and metadata match up
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        private bool PxIntegrityIsValid(Matrix theMatrix)
        {
            MatrixValidatorResult = new PxIntegrityValidator().Validate(theMatrix);
            return MatrixValidatorResult.IsValid;
        }
    }
}
