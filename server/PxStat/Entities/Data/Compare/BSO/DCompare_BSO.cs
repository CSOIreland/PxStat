using API;
using PxStat.DataStore;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PxStat.Data
{
    internal class DCompare_BSO
    {
        internal IDmatrix CompareAmendment(ADO Ado, IMetaData metaData, Compare_DTO_Read dtoRight, Compare_DTO_Read dtoLeft)
        {
            Release_DTO lDto = new Release_DTO();
            Release_DTO rDto = new Release_DTO();

            lDto.RlsCode = dtoLeft.RlsCode;
            rDto.RlsCode = dtoRight.RlsCode;

            DataReader dr = new DataReader();
            var matrixLeft = dr.GetDataset(Ado, metaData, dtoLeft.LngIsoCode, lDto);
            var matrixRight = dr.GetDataset(Ado, metaData, dtoRight.LngIsoCode, rDto);

            //A third matrix (the query matrix is created later from leftMatrix)
            //We need a reference version to ensure that leftMatrix is not changed by reference
            var matrixLeftRef = dr.GetDataset(Ado, metaData, dtoLeft.LngIsoCode, lDto);

            return CompareDmatrixAmendment(matrixLeft, matrixRight, matrixLeftRef);
        }

        /// <summary>
        /// If both metadatas are the same then compare the cells
        /// If metadata is amended then only compare the cells that correspond to the matching metadata
        /// A cell corresponding to new metadata (i.e. a new variable)  will not be returned.
        /// </summary>
        /// <param name="matrixLeft"></param>
        /// <param name="matrixRight"></param>
        /// <param name="confidentialString"></param>
        /// <returns></returns>
        public IDmatrix CompareDmatrixAmendment(IDmatrix matrixLeft, IDmatrix matrixRight, IDmatrix matrixLeftRef, string confidentialString = null, IMetaData meta = null)
        {
            DMatrix_VLD vld = new DMatrix_VLD();
            var res = vld.Validate(matrixLeft);
            res = vld.Validate(matrixRight);
            bool totalChange = false;


            //bool test = matrixLeft.Dspecs[matrixLeft.Language].Dimensions.Equals(matrixRight.Dspecs[matrixRight.Language].Dimensions);

            Dspec commonSpec = new Dspec();


            List<StatDimension> checkDimensions = (matrixLeft.Dspecs[matrixLeft.Language].Dimensions).Except(matrixRight.Dspecs[matrixRight.Language].Dimensions, new StatDimensionComparer()).ToList<StatDimension>();

            //Adding or changing a dimension is a deal breaker. All datapoints are deemed to have changed.

            if (!DimensionsAreEqual(matrixLeft.Dspecs[matrixLeft.Language].Dimensions, matrixRight.Dspecs[matrixRight.Language].Dimensions))
            {
                List<bool> cReport = new List<bool>();
                foreach (var cell in matrixRight.Cells)
                {
                    cReport.Add(true);
                }
                matrixRight.ComparisonReport = cReport;
                return matrixRight;
            }
            else
                commonSpec.Dimensions = (matrixLeft.Dspecs[matrixLeft.Language].Dimensions);//.Intersect(matrixRight.Dspecs[matrixRight.Language].Dimensions, new StatDimensionComparer()).ToList<StatDimension>();

            //if there are different dimensions in each matrix (something added or deleted) then return all false for amendments
            if (commonSpec.Dimensions != null)
            {
                if (commonSpec.Dimensions.Count != matrixLeft.Dspecs[matrixLeft.Language].Dimensions.Count || commonSpec.Dimensions.Count != matrixRight.Dspecs[matrixRight.Language].Dimensions.Count)
                    totalChange = true;
            }
            else totalChange = true;


            foreach (var dim in commonSpec.Dimensions)
            {
                //Get the corresponding dimension
                StatDimension otherDim = matrixRight.Dspecs[matrixRight.Language].Dimensions.Where(x => x.Code.Equals(dim.Code)).FirstOrDefault();

                List<IDimensionVariable> commonVars = dim.Variables.Intersect(otherDim.Variables, new DimensionVariableComparer()).ToList();

                //problem:  matrixLeft corresponding dimension takes on the (reduced) commonVars as its Variables value
                if (commonVars.Count > 0)
                {
                    dim.Variables = commonVars;
                    //commonSpec.Dimensions.Add(dim);
                }
                else totalChange = true;

            }




            IDmatrix leftQueried = new Dmatrix();
            IDmatrix rightQueried = new Dmatrix();

            //To get the two sets of cells to match we must query the larger metadata with the smaller metadata twice
            // so we must query the left matrix with the CommonSpec and then query the right matrix with the commonSpec
            // We then mark the cells from each query where they are different and return the CommonSpec plus compared cells plus report
            if (!totalChange)
            {
                DataReader dr = new DataReader();
                rightQueried = dr.RunFractalQuery(matrixRight, commonSpec, matrixRight.Language);
                res = vld.Validate(matrixLeft);
                leftQueried = dr.RunFractalQuery(matrixLeftRef, commonSpec, matrixLeft.Language);

                res = vld.Validate(rightQueried);
                res = vld.Validate(leftQueried);
            }
            else
            {
                matrixLeft.ComparisonReport = new List<bool>();
                foreach (var c in matrixLeft.Cells)
                {
                    matrixLeft.ComparisonReport.Add(true);

                }
                return matrixLeft;
            }


            string confidential;
            if (meta == null)
                confidential = confidentialString == null ? Configuration_BSO.GetCustomConfig(ConfigType.server, "px.confidential-value") : confidentialString;
            else
                confidential = meta.GetConfidentialValue();


            //matrixRight.ComparisonReport = GetAmendedList(commonSpec);
            var report = GetAmendedList(leftQueried, rightQueried, totalChange);

            leftQueried.ComparisonReport = report;
            leftQueried.Cells = rightQueried.Cells;

            return leftQueried; ;

        }

        private bool DimensionsAreEqual(ICollection<StatDimension> leftDims, ICollection<StatDimension> rightDims)
        {
            Dictionary<string, string> leftDict = leftDims.ToDictionary(x => x.Code, x => x.Value);
            Dictionary<string, string> rightDict = rightDims.ToDictionary(x => x.Code, x => x.Value);

            foreach (var key in leftDict.Keys)
            {
                if (!rightDict.ContainsKey(key)) return false;
                if (!rightDict[key].Equals(leftDict[key])) return false;
            }
            foreach (var key in rightDict.Keys)
            {
                if (!leftDict.ContainsKey(key)) return false;
                if (!leftDict[key].Equals(rightDict[key])) return false;
            }

            return true;
        }


        internal IDmatrix CompareDmatrixAddDelete(IDmatrix matrixLeft, IDmatrix matrixRight, string lngIsoCode, string confidentialString = null)
        {

            DataReader dr = new DataReader();
            List<StatDimension> dimensionWithAddedVariables = new List<StatDimension>();

            Dspec spec = new Dspec() { Language = lngIsoCode };
            // spec.Dimensions = matrixLeft.Dspecs[matrixLeft.TheLanguage].Dimensions.Except(matrixRight.Dspecs[matrixRight.TheLanguage].Dimensions, new StatDimensionComparer()).ToList<StatDimension>();

            if (!DimensionsAreEqual(matrixLeft.Dspecs[matrixLeft.Language].Dimensions, matrixRight.Dspecs[matrixRight.Language].Dimensions))
            {
                List<bool> cReport = new List<bool>();
                foreach (var cell in matrixLeft.Cells)
                {
                    cReport.Add(false);
                }
                matrixLeft.ComparisonReport = cReport;
                return matrixLeft;
            }


            var IntersectDimensions = matrixLeft.Dspecs[matrixLeft.Language].Dimensions;//.Intersect(matrixRight.Dspecs[matrixRight.Language].Dimensions, new StatDimensionComparer()).ToList<StatDimension>();

            foreach (var dim in IntersectDimensions)
            {

                var otherDim = matrixRight.Dspecs[matrixRight.Language].Dimensions.Where(x => x.Equals(dim)).ToList<StatDimension>().FirstOrDefault();
                List<IDimensionVariable> newVars = dim.Variables.Except(otherDim.Variables, new DimensionVariableComparer()).ToList<IDimensionVariable>();

                StatDimension newDimension = new StatDimension
                {
                    Code = dim.Code,
                    Value = dim.Value,
                    Variables = new List<IDimensionVariable>(),
                    Role = dim.Role,

                };

                if (newVars.Count > 0)
                {
                    foreach (var vrb in newVars) vrb.AmendFlag = true;
                    newDimension.Variables = newVars;
                    dimensionWithAddedVariables.Add(newDimension);
                }


                spec.Dimensions.Add(newDimension);
            }
            DataReader reader = new DataReader();
            // Dmatrix result = (Dmatrix)reader.RunFractalQuery(matrixLeft, spec, spec.Language); //nope
            var cellReport = GetAddDeleteList(spec, matrixLeft, dimensionWithAddedVariables);
            matrixLeft.ComparisonReport = cellReport;
            return matrixLeft;
        }

        internal List<bool> GetAmendedList(IDmatrix matrixLeft, IDmatrix matrixRight, bool totalChange)
        {
            List<bool> report = new List<bool>();

            int counter = 0;
            string confidential = Configuration_BSO.GetCustomConfig(ConfigType.server, "px.confidential-value");
            foreach (var cell in matrixLeft.Cells)
            {
                //If there are no periods in common then nothing could have been amended.
                //Similarly, if there was a difference in the number of classifications
                if (totalChange)
                {
                    report.Add(false);
                }
                else
                {
                    //if (cell.Equals(DBNull.Value)) cell = confidential;
                    //if (matrixRight.Cells.ElementAt(counter).Equals(DBNull.Value)) matrixRight.Cells.ElementAt(counter).TdtValue = confidential;

                    try
                    {
                        if (!cell.Equals(matrixRight.Cells.ElementAt(counter)))
                        {
                            var r = matrixRight.Cells.ElementAt(counter);
                            report.Add(true);
                        }
                        else
                            report.Add(false);
                    }
                    catch (Exception ex)
                    {
                        var x = ex.Message;
                    }
                }

                counter++;
            }

            return report;
        }

        internal List<bool> GetAddDeleteList(IDspec spec, IDmatrix referenceMatrix, List<StatDimension> dimensionsWithAddedVariables)
        {
            FlatTableBuilder ftb = new FlatTableBuilder();
            DataTable dt = ftb.GetMatrixDataTableCodesAndLabels(referenceMatrix, spec.Language);
            dt.Columns.Add("Amended");
            dt.Columns.Add("seq");

            int counter = 0;
            foreach (DataRow dr in dt.Rows)
            {
                dr["seq"] = counter;
                dr["Amended"] = false;
                counter++;
            }
            foreach (var dim in dimensionsWithAddedVariables)
            {

                List<string> lvars = new List<string>();
                foreach (var code in dim.Variables)
                {
                    lvars.Add(code.Code);
                }


                var result = dt.AsEnumerable().Where(r => (lvars.Contains((string)r[dim.Code])));

                //update the main table with the result of the query
                foreach (DataRow i in result)
                {

                    var rownum = Convert.ToInt32(i["seq"]);
                    dt.Rows[rownum]["Amended"] = true;

                }

            }
            List<bool> report = new List<bool>();
            foreach (DataRow dr in dt.Rows)
            {
                report.Add(Convert.ToBoolean(dr["Amended"]));
            }
            return report;
        }

        internal IDmatrix CompareAddDelete(IADO Ado, IMetaData metaData, Compare_DTO_Read dtoRight, Compare_DTO_Read dtoLeft)
        {

            Release_DTO lDto = new Release_DTO();
            Release_DTO rDto = new Release_DTO();

            lDto.RlsCode = dtoLeft.RlsCode;
            rDto.RlsCode = dtoRight.RlsCode;

            DataReader dr = new DataReader();
            var matrixLeft = dr.GetDataset(Ado, metaData, dtoLeft.LngIsoCode, lDto);
            var matrixRight = dr.GetDataset(Ado, metaData, dtoRight.LngIsoCode, rDto);


            return CompareDmatrixAddDelete(matrixLeft, matrixRight, dtoRight.LngIsoCode, Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE"));
        }

        private List<bool> GetAmendedList(Dspec spec)
        {
            List<bool> amendedList = new List<bool>();

            List<List<IDimensionVariable>> variableLists = new List<List<IDimensionVariable>>();
            foreach (var dim in spec.Dimensions)
                variableLists.Add(dim.Variables);

            FlatTableBuilder ftb = new FlatTableBuilder();

            var joinedData = ftb.CartesianProduct(variableLists);

            foreach (var row in joinedData) // each row
            {
                bool amend = false;
                foreach (var dim in row) // each dimension (column header) in the row
                {
                    if (dim.AmendFlag) amend = true;
                }
                amendedList.Add(amend);
            }

            return amendedList;

        }

    }
}
