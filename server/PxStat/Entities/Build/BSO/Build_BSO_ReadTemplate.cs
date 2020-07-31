using API;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Entities.BuildData;
using PxStat.Resources.PxParser;
using PxStat.Template;
using System.Dynamic;

namespace PxStat.Build
{
    /// <summary>
    /// Returns a csv template for PxBuild Update.
    /// </summary>
    internal class Build_BSO_ReadTemplate : BaseTemplate_Read<Build_DTO_ReadTemplate, Build_VLD_ReadTemplate>
    {
        /// <summary>
        /// PxFileBuild_BSO_ReadTemplate
        /// </summary>
        /// <param name="request"></param>
        internal Build_BSO_ReadTemplate(JSONRPC_API request) : base(request, new Build_VLD_ReadTemplate())
        {
        }

        /// <summary>
        /// HasPrivilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            dynamic result = new ExpandoObject();


            //This is required for validation in the Matrix code, but is not used for px build
            Request.parameters.GrpCode = Utility.GetCustomConfig("APP_DEFAULT_GROUP");

            //This is required for validation in the Matrix code, but is not used for px build
            Request.parameters.GrpCode = Utility.GetCustomConfig("APP_DEFAULT_GROUP");
            Request.parameters.source = Utility.GetCustomConfig("APP_DEFAULT_SOURCE");

            //We get the PxDocument from the validator
            PxValidator pxValidator = new PxValidator();
            PxDocument PxDoc = pxValidator.ParsePxFile(DTO.MtrInput);
            if (!pxValidator.ParseValidatorResult.IsValid)
            {
                Response.error = Error.GetValidationFailure(pxValidator.ParseValidatorResult.Errors);
                return false;
            }



            //Get this matrix from the px file , but we also need to pass in the Timeval stuff
            //The "" bit is temporary until we make the parameters optional (Currently this interferes with existing overloads)
            Matrix theMatrixData = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");

            var signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()));
            if (signature != DTO.Signature)
            {
                Response.error = Label.Get("error.validation");
                return false;
            }



            Build_BSO pBso = new Build_BSO();

            result.FrqValue = "";
            result.template = pBso.GetCsvTemplate(theMatrixData, DTO.LngIsoCode, DTO.FrqCodeTimeval);
            result.MtrCode = theMatrixData.Code;
            Response.data = result;



            return true;
        }
    }
}
