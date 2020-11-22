using API;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Security;
using PxStat.Template;
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
            Matrix_BSO mBso = new Matrix_BSO(Ado);

            if (mBso.Validate(new PxUpload_DTO() { MtrInput = DTO.MtrInput, FrqCodeTimeval = DTO.FrqCodeTimeval, FrqValueTimeval = DTO.FrqValueTimeval, Signature = DTO.Signature }))
            {


                return true;
            }
            MatrixData = mBso.MatrixData;
            if (MatrixData != null)
            {

                if (MatrixData.MainSpec.requiresResponse)
                {
                    this.RequiresResponse = true;
                    return false;
                }
            }

            Response.error = mBso.ResponseError;
            return false;
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
                Matrix.Specification langSpec = MatrixData.GetSpecFromLanguage(Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code"));
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
