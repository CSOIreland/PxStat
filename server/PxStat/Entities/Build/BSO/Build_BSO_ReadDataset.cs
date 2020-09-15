using API;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Resources.PxParser;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Build
{
    internal class Build_BSO_ReadDataset : BaseTemplate_Read<BuildUpdate_DTO, Build_VLD_BuildReadNewPeriods>
    {
        internal Build_BSO_ReadDataset(JSONRPC_API request) : base(request, new Build_VLD_BuildReadNewPeriods())
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
            //do the physical structure validation

            //This is required for validation in the Matrix code, but is not used for px build
            Request.parameters.GrpCode = Utility.GetCustomConfig("APP_DEFAULT_GROUP");
            Request.parameters.CprCode = Utility.GetCustomConfig("APP_DEFAULT_SOURCE");
            //validate the px file

            //We get the PxDocument from the validator
            PxValidator pxValidator = new PxValidator();
            PxDocument PxDoc = pxValidator.ParsePxFile(DTO.MtrInput);
            if (!pxValidator.ParseValidatorResult.IsValid)
            {
                Response.error = Error.GetValidationFailure(pxValidator.ParseValidatorResult.Errors);
                return false;
            }



            //There might be a cache:

            Matrix theMatrixData;
            MemCachedD_Value mtrCache = MemCacheD.Get_BSO("PxStat.Build", "Build_BSO_Validate", "Validate", Constants.C_CAS_BUILD_MATRIX + DTO.Signature);

            if (mtrCache.hasData)
            {
                SerializableMatrix sm = Newtonsoft.Json.JsonConvert.DeserializeObject<SerializableMatrix>(mtrCache.data.ToString());
                theMatrixData = new Matrix().ExtractFromSerializableMatrix(sm);
            }
            else
                //Get this matrix from the px file 
                theMatrixData = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");

            Build_BSO bBso = new Build_BSO();

            theMatrixData = bBso.Query(theMatrixData, GetQueryMatrix(new List<string>() { "2020M05", "2020M06" }, theMatrixData));

            //var dataList = bBso.GetDataForAllPeriods(theMatrixData, DTO, Ado);

            var dataList = bBso.GetMatrixDataItems(theMatrixData, DTO.LngIsoCode, theMatrixData.MainSpec);

            dynamic result = new ExpandoObject();

            result.csv = theMatrixData.GetCsvObject(dataList, DTO.LngIsoCode, true);
            result.MtrCode = theMatrixData.Code;
            Response.data = result;


            return true;
        }

        private Matrix GetQueryMatrix(List<string> periods, Matrix dataMatrix)
        {
            Matrix queryMatrix = new Matrix();
            queryMatrix.MainSpec = new Matrix.Specification();
            queryMatrix.MainSpec.Statistic = dataMatrix.MainSpec.Statistic;
            queryMatrix.MainSpec.Classification = dataMatrix.MainSpec.Classification;
            queryMatrix.MainSpec.Frequency = new FrequencyRecordDTO_Create() { Code = dataMatrix.MainSpec.Frequency.Code, Value = dataMatrix.MainSpec.Frequency.Value };


            if (periods.Count == 0) queryMatrix.MainSpec.Frequency.Period = dataMatrix.MainSpec.Frequency.Period;
            else
            {
                queryMatrix.MainSpec.Frequency.Period = new List<PeriodRecordDTO_Create>();
                foreach (string per in periods)
                {
                    queryMatrix.MainSpec.Frequency.Period.AddRange(dataMatrix.MainSpec.Frequency.Period.Where(x => x.Code == per));
                }
            }

            return queryMatrix;
        }

    }
}
