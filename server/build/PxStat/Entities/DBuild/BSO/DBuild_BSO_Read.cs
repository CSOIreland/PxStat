using API;
using Newtonsoft.Json.Linq;
using PxStat.Data;
using PxStat.Data.Px;
using PxStat.Security;
using PxStat.Template;
using System.Collections.Generic;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_Read : BaseTemplate_Read<DBuild_DTO_Read, DBuild_Read_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_Read(JSONRPC_API request) : base(request, new DBuild_Read_VLD())
        { }

        /// <summary>
        /// Test privilege
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
            //Create a structure to contain the dspecs 

            JsonStatBuilder2_0 jxb = new JsonStatBuilder2_0();
            List<JRaw> jsonData = new List<JRaw>();

            foreach (var spec in dmatrix.Dspecs.Values)
            {
                var jsonStat = jxb.Create(dmatrix, spec.Language);
                jsonData.Add(new JRaw(Serialize.ToJson(jsonStat)));

            }


            //Return as a collection of dspecs in JSON-stat
            Response.data = jsonData;

            return true;
        }
    }
}
