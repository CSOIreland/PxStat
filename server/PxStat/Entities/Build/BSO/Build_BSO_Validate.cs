using System.Collections.Generic;
using System.Dynamic;
using API;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources.PxParser;
using PxStat.Template;

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
        internal bool Validate()
        {
            PxValidator ppValidator = new PxValidator();
            PxDocument PxDoc = ppValidator.ParsePxFile(DTO.MtrInput);


            if (!ppValidator.ParseValidatorResult.IsValid)
            {
                Response.error = Error.GetValidationFailure(ppValidator.ParseValidatorResult.Errors);
                return false;
            }

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
            validationResult.Signature = null;
            validationResult.FrqValueCandidate = FrqValues;

            if (!Validate() && !RequiresResponse)
            {
                return false;

            }

            if (RequiresResponse)
            {
                //cancel any validation errors and return an object to enable the user to choose which should be the time dimension
                Matrix.Specification langSpec = MatrixData.GetSpecFromLanguage(Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE"));
                if (langSpec == null) langSpec = MatrixData.MainSpec;

                foreach (var v in langSpec.MainValues)
                {
                    FrqValues.Add(v.Key);
                }

                // Set Frequency candidates
                validationResult.FrqValueCandidate = FrqValues;
                Response.data = validationResult;
                return true;
            }

            // Set the Signature
            validationResult.Signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()));
            Response.data = validationResult;
            return true;
        }



    }
}
