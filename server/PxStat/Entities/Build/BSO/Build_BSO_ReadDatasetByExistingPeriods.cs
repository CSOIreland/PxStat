using API;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Resources.PxParser;
using PxStat.Template;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using static PxStat.Data.Matrix;

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

            Stopwatch sw = new Stopwatch();
            sw.Start();

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
            Log.Instance.Debug("*Diagnostic* px valid: " + sw.ElapsedMilliseconds);

            //There might be a cache:
            Matrix matrixPxFile;
            MemCachedD_Value mtrCache = MemCacheD.Get_BSO("PxStat.Build", "Build_BSO_Validate", "Validate", Constants.C_CAS_BUILD_MATRIX + DTO.Signature);

            if (mtrCache.hasData)
            {
                SerializableMatrix sm = Newtonsoft.Json.JsonConvert.DeserializeObject<SerializableMatrix>(mtrCache.data.ToString());
                matrixPxFile = new Matrix().ExtractFromSerializableMatrix(sm);
            }
            else
                matrixPxFile = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");



            Log.Instance.Debug("*Diagnostic* Matrix created: " + sw.ElapsedMilliseconds);

            //Sorting, in case the px file is not in SPC order
            Build_BSO pBso = new Build_BSO();
            //List<DataItem_DTO> existingItems = pBso.GetExistingDataItems(matrixPxFile, matrixPxFile.MainSpec, true, false);

            Specification theSpec = matrixPxFile.GetSpecFromLanguage(DTO.LngIsoCode);

            // pBso.SetMetadataSortIds(ref theSpec);

            List<DataItem_DTO> existingItems = pBso.GetMatrixDataItems(matrixPxFile, DTO.LngIsoCode, theSpec);

            // pBso.SetDataSortIds(ref existingItems, theSpec);

            //Log.Instance.Debug("*Diagnostic* GetExistingDataItems: " + sw.ElapsedMilliseconds);



            Log.Instance.Debug("*Diagnostic* Read Cells - count = " + matrixPxFile.Cells.Count + "  elapsed: " + sw.ElapsedMilliseconds);

            result.csv = matrixPxFile.GetCsvObject(existingItems, DTO.LngIsoCode, true);

            Log.Instance.Debug("*Diagnostic* GetCsvObject-   elapsed: " + sw.ElapsedMilliseconds);
            result.MtrCode = matrixPxFile.Code;

            Response.data = result;
            Log.Instance.Debug("GetCsvObject: " + sw.ElapsedMilliseconds);

            return true;
        }

    }
}
