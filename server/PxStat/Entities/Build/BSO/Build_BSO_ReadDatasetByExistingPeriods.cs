using API;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources.PxParser;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;


namespace PxStat.Build
{
    internal class Build_BSO_ReadDatasetByExistingPeriods : BaseTemplate_Read<BuildUpdate_DTO, Build_VLD_BuildExistingPeriods>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Build_BSO_ReadDatasetByExistingPeriods(JSONRPC_API request) : base(request, new Build_VLD_BuildExistingPeriods())
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
            dynamic result = new ExpandoObject();



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


            Matrix matrixPxFile = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");


            List<dynamic> cells = new List<dynamic>();

            foreach (var c in matrixPxFile.Cells)
            {
                dynamic tdt = new ExpandoObject();
                tdt.TdtValue = c.Value;
                cells.Add(tdt);
            }

            matrixPxFile.Cells = cells;


            result.csv = matrixPxFile.GetCSVObject(DTO.LngIsoCode).ToString();
            result.MtrCode = matrixPxFile.Code;
            Response.data = result;


            return true;
        }

    }
}
