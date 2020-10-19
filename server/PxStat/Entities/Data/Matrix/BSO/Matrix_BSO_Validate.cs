using API;
using FluentValidation.Results;
using PxParser.Resources.Parser;
using PxStat.Data.Px;
using PxStat.Resources;
using PxStat.Security;
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

            // MatrixData.Sort();

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

            bool isValid = false;

            //Get the matrix, but use the cached version that was created during validation if at all possible
            MemCachedD_Value mtrCache = MemCacheD.Get_BSO("PxStat.Data", "Matrix_API", "Validate", Constants.C_CAS_MATRIX_VALIDATE + signature);

            if (mtrCache.hasData)
            {
                MatrixData = new Matrix().ExtractFromSerializableMatrix(mtrCache.data.ToObject<SerializableMatrix>());
                isValid = true;
            }
            else
            {
                isValid = Validate();
            }


            if (isValid)
            {
                validationResult.Signature = signature;
                validationResult.FrqValueCandidate = FrqValues;
                Response.data = validationResult;

                //cache this so that we won't have to recreate the matrix in future steps
                SerializableMatrix sm = MatrixData.GetSerializableObject();
                MemCacheD.Store_BSO<string>("PxStat.Data", "Matrix_API", "Validate", Constants.C_CAS_MATRIX_VALIDATE + signature, sm, DateTime.Now.AddDays(Convert.ToInt32(Utility.GetCustomConfig("APP_BUILD_MATRIX_CACHE_LIFETIME_DAYS"))));

                return true;
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
                if (!DTO.LngIsoCode.Equals(Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code")))
                {
                    SettingsValidatorResultDefaultLanguage = new PxSettingsValidator(Ado, true, Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code")).Validate(theMatrix);
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
