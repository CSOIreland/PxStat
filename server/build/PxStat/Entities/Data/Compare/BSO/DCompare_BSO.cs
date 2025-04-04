﻿using API;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using PxStat.DataStore;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace PxStat.Data
{
    public class DCompare_BSO
    {
        internal IDmatrix CompareAmendment(IADO Ado,  Compare_DTO_Read dtoRight, Compare_DTO_Read dtoLeft)
        {
            Release_DTO lDto = new Release_DTO();
            Release_DTO rDto = new Release_DTO();

            lDto.RlsCode = dtoLeft.RlsCode;
            rDto.RlsCode = dtoRight.RlsCode;

            DataReader dr = new DataReader();
            var matrixLeft = dr.GetDataset(Ado, dtoLeft.LngIsoCode, lDto);
            var matrixRight = dr.GetDataset(Ado, dtoRight.LngIsoCode, rDto);

            //A third matrix (the query matrix is created later from leftMatrix)
            //We need a reference version to ensure that leftMatrix is not changed by reference
            var matrixLeftRef = dr.GetDataset(Ado,  dtoLeft.LngIsoCode, lDto);

            if (matrixRight.Dspecs.Count == 0 || matrixLeftRef.Dspecs.Count == 0)
                return null;

            return CompareDmatrixAmendment(matrixLeft, matrixRight, matrixLeftRef, Ado, null);
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
        public IDmatrix CompareDmatrixAmendment(IDmatrix matrixLeft, IDmatrix matrixRight, IDmatrix matrixLeftRef, IADO ado = null, string confidentialString = null)
        {
            DMatrix_VLD vld = new DMatrix_VLD( ado);
            var res = vld.Validate(matrixLeft);
           
            bool totalChange = false;

            //check usage of validation
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


            string confidential = confidentialString == null ? Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "px.confidential-value") : confidentialString;
           

            var granularReport=GetGranularReport(leftQueried,rightQueried);

            //matrixRight.ComparisonReport = GetAmendedList(commonSpec);
            var report = granularReport.Select(x => x.value).ToList();// GetAmendedList(leftQueried, rightQueried, totalChange);

            leftQueried.ComparisonReport = report;
            leftQueried.Cells = rightQueried.Cells;

            return leftQueried; ;

        }

        private List<boolId> GetGranularReport(IDmatrix leftQueried, IDmatrix rightQueried)
        {
            
            FlatTableBuilder fb = new FlatTableBuilder();

            //express leftQueried as a DataTable
            DataTable dtLeft = fb.GetMatrixDataTableCodesAndSequences(leftQueried, leftQueried.Dspecs.First().Value.Language);
            //express rightQueried as a DataTable
            DataTable dtRight = fb.GetMatrixDataTableCodesAndSequences(rightQueried, rightQueried.Dspecs.First().Value.Language);

            //create a sort expression based on dimension columns and sort each datatable by its sequences

            List<string> codeList = rightQueried.Dspecs.First().Value.Dimensions.OrderBy(x => x.Sequence).Select(x => x.Code).ToList();

            string sortStatement = "";
            foreach(string s in codeList)
            {
                sortStatement =sortStatement + s + "_sequence"+  ( s!=codeList.Last()? "," : "");
            }
            //Apply sort
            dtLeft.DefaultView.Sort = sortStatement + " ASC";
            dtRight.DefaultView.Sort = sortStatement + " ASC";

            dtLeft =dtLeft.DefaultView.ToTable();
            dtRight = dtRight.DefaultView.ToTable();

            List<objectId> leftObjectIds = new List<objectId>();
            List<objectId> rightObjectIds = new List<objectId>();

            int counter = 0;
            string leftValueColumn = Label.Get("xlsx.value", leftQueried.Dspecs.First().Value.Language);
            string rightValueColumn = Label.Get("xlsx.value", rightQueried.Dspecs.First().Value.Language);
            foreach (DataRow row in dtLeft.Rows)
            {
                if (dtLeft.Columns.Contains(leftValueColumn))
                {
                    leftObjectIds.Add(new objectId { id = counter,value=row[leftValueColumn] });
                    counter++;
               }
            }

            counter = 0;
            foreach (DataRow row in dtRight.Rows)
            {
                if (dtRight.Columns.Contains(rightValueColumn))
                {
                    rightObjectIds.Add(new objectId { id = counter, value = row[rightValueColumn] });
                    counter++;
                }
            }

            //Check that the cells for each matrix are the same or otherwise and compile a report
            var result = from l in leftObjectIds
                         join r in rightObjectIds
                         on l.id equals r.id
                         select new boolId { id = l.id,  value= !l.value.Equals(r.value) };


            return result.ToList();
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


        public IDmatrix CompareDmatrixAddDelete(IDmatrix matrixLeft, IDmatrix matrixRight, string lngIsoCode, string confidentialString = null)
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
            string confidential = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "px.confidential-value");
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

        internal IDmatrix CompareAddDelete(IADO Ado,  Compare_DTO_Read dtoRight, Compare_DTO_Read dtoLeft)
        {

            Release_DTO lDto = new Release_DTO();
            Release_DTO rDto = new Release_DTO();

            lDto.RlsCode = dtoLeft.RlsCode;
            rDto.RlsCode = dtoRight.RlsCode;

            DataReader dr = new DataReader();
            var matrixLeft = dr.GetDataset(Ado,  dtoLeft.LngIsoCode, lDto);
            var matrixRight = dr.GetDataset(Ado,  dtoRight.LngIsoCode, rDto);


            return CompareDmatrixAddDelete(matrixLeft, matrixRight, dtoRight.LngIsoCode, Configuration_BSO.GetStaticConfig("APP_PX_CONFIDENTIAL_VALUE"));
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
    internal class objectId
    {
        internal int id { get; set; }
        internal dynamic value { get; set; }    
    }

    internal class boolId
    {
        internal int id { get; set; }
        internal bool value { get; set; }
    }
}
