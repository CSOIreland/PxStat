using API;
using FluentValidation.Results;
using PxParser.Resources.Parser;
using PxStat.Data.Px;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;

namespace PxStat.Data
{
    /// <summary>
    /// Validations for Matrix
    /// </summary>
    internal class Matrix_BSO_Validate : BaseTemplate_Read<PxUpload_DTO, PxUploadValidator>
    {
        /// <summary>
        /// class variable
        /// </summary>
        private PxDocument PxDoc { get; set; }

        /// <summary>
        /// class variable
        /// </summary>
        private Matrix_ADO matrixAdo;

        /// <summary>
        /// class property
        /// </summary>
        public ValidationResult SchemaValidatorResult { get; private set; }

        /// <summary>
        /// class property
        /// </summary>
        public ValidationResult SettingsValidatorResult { get; private set; }

        public ValidationResult SettingsValidatorResultDefaultLanguage { get; private set; }

        /// <summary>
        /// class property
        /// </summary>
        public ValidationResult ParseValidatorResult { get; private set; }

        /// <summary>
        /// class property
        /// </summary>
        public Matrix MatrixData { get; set; }

        /// <summary>
        /// class property
        /// </summary>
        public ValidationResult IntegrityValidatorResult { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="request"></param>
        internal Matrix_BSO_Validate(JSONRPC_API request) : base(request, new PxUploadValidator())
        {
            matrixAdo = new Matrix_ADO(Ado);
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// run validation
        /// </summary>
        /// <returns></returns>
        internal bool Validate()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();



            if (!ParsePxFile(DTO))
            {
                Response.error = Error.GetValidationFailure(ParseValidatorResult.Errors);
                return false;
            }

            if (!PxSchemaIsValid())
            {
                LogValidatorErrors(SchemaValidatorResult);
                Response.error = Error.GetValidationFailure(SchemaValidatorResult.Errors);
                return false;
            }

            MatrixData = new Matrix(PxDoc, DTO);

            if (MatrixData.MainSpec.Frequency == null)
            {
                //This means that we failed to create a Frequency. This normally occurs where there is no Timeval but a FrqCode/FrqValue was not supplied
                MatrixData.MainSpec.requiresResponse = true;
                return false;
            }



            if (MatrixData.MainSpec.Classification == null)
            {
                return false;
            }

            if (MatrixData.MainSpec.Statistic == null)
            {
                return false;
            }

            if (!PxIntegrityIsValid(MatrixData))
            {
                LogValidatorErrors(IntegrityValidatorResult);
                Response.error = Error.GetValidationFailure(IntegrityValidatorResult.Errors);
                return false;
            }

            if (!PxSettingsAreValid(MatrixData))
            {
                //We want the logged item to be in the default language irrespective of the requested language
                if (SettingsValidatorResultDefaultLanguage == null)
                    LogValidatorErrors(SettingsValidatorResult);
                else
                    LogValidatorErrors(SettingsValidatorResultDefaultLanguage);

                Response.error = Error.GetValidationFailure(SettingsValidatorResult.Errors);
                return false;
            }

            sw.Stop();
            Log.Instance.Debug(string.Format("Matrix validated in {0} ms", Math.Round((double)sw.ElapsedMilliseconds)));

            Response.data = API.JSONRPC.success;
            return true;
        }



        /// <summary>
        /// Validate Px integrity
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        private bool PxIntegrityIsValid(Matrix theMatrix)
        {
            IntegrityValidatorResult = new PxIntegrityValidator(theMatrix.MainSpec, DTO.LngIsoCode).Validate(theMatrix);
            return IntegrityValidatorResult.IsValid;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            var signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()));
            dynamic validationResult = new ExpandoObject();
            List<string> FrqValues = new List<string>();
            PxStat.RequestLanguage.LngIsoCode = DTO.LngIsoCode;

            if (Validate())
            {
                validationResult.Signature = signature;
                validationResult.FrqValueCandidate = FrqValues;
                Response.data = validationResult;
                return true;
            }
            else if (MatrixData == null)
            {
                return false;
            }
            else if (MatrixData.MainSpec.requiresResponse)
            {
                //cancel any validation errors and return an object to enable the user to choose which should be the time dimension
                Matrix.Specification langSpec = MatrixData.GetSpecFromLanguage(DTO.LngIsoCode);
                if (langSpec == null) langSpec = MatrixData.MainSpec;

                foreach (var v in langSpec.MainValues)
                {
                    FrqValues.Add(v.Key);
                }

                validationResult.Signature = null;
                validationResult.FrqValueCandidate = FrqValues;
                Response.data = validationResult;
                return true;
            }

            //Response.error = Label.Get("error.validation");
            return false;
        }

        /// <summary>
        /// Validate Px Settings
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        internal bool PxSettingsAreValid(Matrix theMatrix)
        {
            SettingsValidatorResult = new PxSettingsValidator(Ado, true, DTO.LngIsoCode).Validate(theMatrix);
            if (RequestLanguage.LngIsoCode != null)
            {
                if (!DTO.LngIsoCode.Equals(Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE")))
                {
                    SettingsValidatorResultDefaultLanguage = new PxSettingsValidator(Ado, true, Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE")).Validate(theMatrix);
                }
            }
            return SettingsValidatorResult.IsValid;
        }

        /// <summary>
        /// Validate Px Schema
        /// </summary>
        /// <returns></returns>
        internal bool PxSchemaIsValid()
        {
            SchemaValidatorResult = new PxSchemaValidator(Ado).Validate(PxDoc); ;
            return SchemaValidatorResult.IsValid;
        }

        /// <summary>
        /// Test if a Px file will parse
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal bool ParsePxFile(PxUpload_DTO dto)
        {
            ParseValidatorResult = new ValidationResult();
            try
            {
                PxDoc = PxStatEngine.ParsePxInput(dto.MtrInput);
            }

            catch (Exception e)
            {

                ParseValidatorResult.Errors.Add(new ValidationFailure("MtrInput", RequestLanguage.LngIsoCode == null ? Label.Get("px.parse") : Label.GetFromRequestLanguage("px.parse")));
                ParseValidatorResult.Errors.Add(new ValidationFailure("MtrInput", e.Message));
            }

            return PxDoc != null;
        }

        /// <summary>
        /// Log validation errors
        /// </summary>
        /// <param name="validatorResult"></param>
        private static void LogValidatorErrors(FluentValidation.Results.ValidationResult validatorResult)
        {
            foreach (var error in validatorResult.Errors)
            {
                Log.Instance.Debug(error.ErrorMessage);
            }
        }


    }
}
