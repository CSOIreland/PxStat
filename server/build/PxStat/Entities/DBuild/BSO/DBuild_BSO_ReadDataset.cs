using API;
using PxStat.Data;
using PxStat.Data.Px;
using PxStat.Template;
using System.Data;
using System.Dynamic;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_ReadDataset : BaseTemplate_Read<DBuild_DTO_Update, DBuild_VLD_BuildReadDataset>
    {
        internal DBuild_BSO_ReadDataset(JSONRPC_API request) : base(request, new DBuild_VLD_BuildReadDataset())
        {

        }
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
            DBuild_BSO bso = new DBuild_BSO();
            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(DTO.MtrInput);
            var pxDocument = pxManualParser.Parse();

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Response.error = Label.Get("error.invalid", DTO.LngIsoCode);
                return false;
            }

            IDmatrix dmatrix = bso.ReadDataset(pxDocument, DTO);


            FlatTableBuilder ftb = new FlatTableBuilder();
            DataTable dt = null;

            if (!DTO.Labels)
                dt = ftb.GetMatrixDataTableCodesOnly(dmatrix, DTO.Dspecs[0].Language, true);
            else
                dt = ftb.GetMatrixDataTableCodesAndLabels(dmatrix, DTO.Dspecs[0].Language, true);

            dynamic result = new ExpandoObject();
            result.csv = ftb.GetCsv(dt, "\"", null, true);
            result.MtrCode = dmatrix.Code;
            Response.data = result;
            return true;
        }

    }
}
