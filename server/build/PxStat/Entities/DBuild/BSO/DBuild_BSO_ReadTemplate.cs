using API;
using PxStat.Data;
using PxStat.Data.Px;
using PxStat.Entities.DBuild;
using PxStat.Security;
using PxStat.Template;
using System.Data;
using System.Dynamic;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_ReadTemplate : BaseTemplate_Read<DBuild_DTO_ReadTemplate, DBuild_VLD_ReadTemplate>
    {
        /// <summary>
        /// PxFileBuild_BSO_ReadTemplate
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_ReadTemplate(JSONRPC_API request) : base(request, new DBuild_VLD_ReadTemplate())
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
            //This is required for validation in the Matrix code, but is not used for px build
            Request.parameters.GrpCode = Configuration_BSO.GetStaticConfig("APP_DEFAULT_GROUP");
            Request.parameters.source = Configuration_BSO.GetStaticConfig("APP_DEFAULT_SOURCE");

            //Get the Dmatrix from the px doucment
            if (!DTO.Signature.Equals(Utility.GetMD5(Configuration_BSO.GetStaticConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()))))
            {
                Response.error = Label.Get("error.validation");
                return false;
            }

            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(DTO.MtrInput);
            var pxDocument = pxManualParser.Parse();

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Response.error = Error.GetValidationFailure(pxValidation.Errors);
                return false;
            }

            //Get the basic matrix from the px data
            IDmatrix dmatrix = new Dmatrix();
            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = DTO.FrqValueTimeval, LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"), FrqCodeTimeval = DTO.FrqCodeTimeval };
            

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);

            FlatTableBuilder ftb = new FlatTableBuilder();

            DataTable dt = ftb.GetMatrixDataTableCodesOnly(dmatrix, DTO.LngIsoCode, true, true);

            dynamic result = new ExpandoObject();
            result.FrqValue = "";
            result.template = ftb.GetCsv(dt, "\"",null,true);
            result.MtrCode = dmatrix.Code;
            Response.data = result;

            return true;
        }
    }
}
