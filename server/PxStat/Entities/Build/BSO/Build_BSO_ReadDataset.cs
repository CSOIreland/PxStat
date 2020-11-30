using API;
using PxParser.Resources.Parser;
using PxStat.Data;
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


            Matrix theMatrixData;

            //Get this matrix from the px file 
            theMatrixData = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");

            Build_BSO bBso = new Build_BSO();

            //We need to add the new periods to the matrix
            //And either add cells of default data or else the values from DTO.data

            var requestPeriods = DTO.Periods;//.OrderBy(x => x.Code).ToList();


            DTO.Periods = DTO.Periods.Except(theMatrixData.MainSpec.Frequency.Period).ToList();

            theMatrixData = bBso.UpdateMatrixFromBuild(theMatrixData, DTO, Ado);

            theMatrixData.MainSpec.MainValues = sortMainValuesSpc(theMatrixData.MainSpec);

            theMatrixData.MainSpec.Values = theMatrixData.MainSpec.MainValues;

            //bBso.Query needs to be in the same order as the periods in the matrix - why?
            theMatrixData = bBso.Query(theMatrixData, GetQueryMatrix(requestPeriods, theMatrixData));


            var dataList = bBso.GetMatrixDataItems(theMatrixData, DTO.LngIsoCode, theMatrixData.MainSpec, false);

            dynamic result = new ExpandoObject();

            result.csv = theMatrixData.GetCsvObject(dataList, DTO.LngIsoCode, true, null, DTO.Labels);
            result.MtrCode = theMatrixData.Code;
            Response.data = result;


            return true;
        }

        private IList<KeyValuePair<string, IList<IPxSingleElement>>> sortMainValuesSpc(Matrix.Specification spec)
        {
            var newValues = new List<object>();


            if (spec.MainValues.Where(x => x.Key == spec.ContentVariable).Count() > 0)
            {
                var val = spec.MainValues.Where(x => x.Key == spec.ContentVariable).FirstOrDefault();
                newValues.Add(val);
            }

            if (spec.MainValues.Where(x => x.Key == spec.Frequency.Value).Count() > 0)
            {
                var val = spec.MainValues.Where(x => x.Key == spec.Frequency.Value).FirstOrDefault();
                newValues.Add(val);
            }

            foreach (var cls in spec.Classification)
            {
                if (spec.MainValues.Where(x => x.Key == cls.Value).Count() > 0)
                {
                    var val = spec.MainValues.Where(x => x.Key == cls.Value).FirstOrDefault();
                    newValues.Add(val);
                }
            }

            spec.MainValues.Clear();
            foreach (var v in newValues)
            {
                spec.MainValues.Add((KeyValuePair<string, IList<PxParser.Resources.Parser.IPxSingleElement>>)v);
            }

            return spec.MainValues;
        }

        private Matrix GetQueryMatrix(List<PeriodRecordDTO_Create> periods, Matrix dataMatrix)
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
                foreach (var per in periods)
                {
                    queryMatrix.MainSpec.Frequency.Period.AddRange(dataMatrix.MainSpec.Frequency.Period.Where(x => x.Code == per.Code));
                }
            }

            return queryMatrix;
        }

    }
}
