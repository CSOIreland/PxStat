using API;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Resources.PxParser;
using PxStat.Security;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace PxStat.Build
{
    /// <summary>
    /// Validations for Matrix
    /// </summary>
    internal class Build_BSO_Validate : BaseTemplate_Read<Build_DTO_Read, Build_Validate_VLD>
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
        public Matrix MatrixData { get; set; }

        bool RequiresResponse = false;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="request"></param>
        internal Build_BSO_Validate(JSONRPC_API request) : base(request, new Build_Validate_VLD())
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
        internal bool Validate(string signature)
        {
            PxValidator ppValidator = new PxValidator();
            PxDocument PxDoc = ppValidator.ParsePxFile(DTO.MtrInput);

            if (!ppValidator.ParseValidatorResult.IsValid)
            {
                Response.error = Error.GetValidationFailure(ppValidator.ParseValidatorResult.Errors);
                return false;
            }
            //There might be a cache:
            MemCachedD_Value mtrCache = MemCacheD.Get_BSO("PxStat.Build", "Build_BSO_Validate", "Validate", Constants.C_CAS_BUILD_MATRIX + signature);

            if (mtrCache.hasData)
            {
                SerializableMatrix smtx = Newtonsoft.Json.JsonConvert.DeserializeObject<SerializableMatrix>(mtrCache.data.ToString());
                MatrixData = new Matrix().ExtractFromSerializableMatrix(smtx);
            }
            else
                MatrixData = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");


            if (MatrixData.MainSpec.requiresResponse)
            {
                this.RequiresResponse = true;
                return false;
            }


            MatrixValidator matrixValidator = new MatrixValidator();
            if (!matrixValidator.Validate(MatrixData, false))
            {
                Response.error = Error.GetValidationFailure(matrixValidator.MatrixValidatorResult.Errors);
                return false;
            }

            //If we've new data then cache it for a set period.
            if (!mtrCache.hasData)
            {

                SerializableMatrix sm = MatrixData.GetSerializableObject();
                MemCacheD.Store_BSO<string>("PxStat.Build", "Build_BSO_Validate", "Validate", Constants.C_CAS_BUILD_MATRIX + signature, sm, DateTime.Now.AddDays(Convert.ToInt32(Utility.GetCustomConfig("APP_BUILD_MATRIX_CACHE_LIFETIME_DAYS"))), Constants.C_CAS_BUILD_MATRIX);


            }

            return true;
        }




        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {



            // Init
            List<string> FrqValues = new List<string>();
            dynamic validationResult = new ExpandoObject();
            validationResult.Signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()));// null;
            validationResult.FrqValueCandidate = FrqValues;

            if (!Validate(validationResult.Signature) && !RequiresResponse)
            {
                return false;

            }

            if (RequiresResponse)
            {
                //cancel any validation errors and return an object to enable the user to choose which should be the time dimension
                Matrix.Specification langSpec = MatrixData.GetSpecFromLanguage(Configuration_BSO.GetCustomConfig("language.iso.code"));
                if (langSpec == null) langSpec = MatrixData.MainSpec;

                foreach (var v in langSpec.MainValues)
                {
                    FrqValues.Add(v.Key);
                }

                // Set Frequency candidates
                validationResult.FrqValueCandidate = FrqValues;
                validationResult.Signature = null;
                Response.data = validationResult;
                return true;
            }

            // Set the Signature
            Response.data = validationResult;
            return true;
        }



    }
}
