using API;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Data.Px;
using PxStat.DataStore;
using PxStat.JsonStatSchema;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace PxStat.DBuild
{
    public class DBuild_BSO : IDisposable
    {
        public IDmatrix refMatrix;


        public bool AreVariablesSequential(StatDimension dimension)
        {
            var variableCodes = dimension.Variables.Select(x => x.Code);
            return variableCodes.SequenceEqual(variableCodes.OrderBy(x => x));
        }

        /// <summary>
        /// Sorts the variables of a dimension in the way specified by the sorting dictionary and adjusts the sorting of the cells accordingly
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="sorting"></param>
        /// <param name="sortedDimensionCode"></param>
        /// <returns></returns>
        public IDmatrix SortVariablesInDimension(IDmatrix matrix, Dictionary<int,int> newSorting,int dimensionSequence)
        {
            StatDimension dimension=null;
            //make sure the matrix contains the dimension sequence referred to in the parameters
            foreach(var spec in matrix.Dspecs)
            {
                if (!spec.Value.Dimensions.Where(x => x.Sequence == dimensionSequence).Any())
                    return matrix;

                //make sure the newSorting list contains all of the existing variable sequences
                dimension = spec.Value.Dimensions.Where(x => x.Sequence == dimensionSequence).First();
                List<int> variableSequences= dimension.Variables.Select(x => x.Sequence).ToList();
                if (!variableSequences.Intersect((newSorting.Values)).Count().Equals(variableSequences.Count))
                    return matrix;
                //Get lambdas etc in case they're not there already
                LoadDspec(spec.Value);
            }



            //Associate the list of Cells with the current variable sorting
            FlatTableBuilder ftb = new ();
            List<List<IDimensionVariable>> allVariables = new();
            int sort = -1;
            int dimensionAbsoluteOrder = 0;
            int counter = 0;
            foreach (var dim in matrix.Dspecs.First().Value.Dimensions.OrderBy(x=>x.Sequence).ToList())
            {
                //Ensure the sequences are contigious
                if (dim.Sequence <= sort) return matrix;
                allVariables.Add(dim.Variables.OrderBy(x=>x.Sequence).ToList());
                sort = dim.Sequence;
                if (dim.Sequence == dimensionSequence) dimensionAbsoluteOrder = counter;
                counter++;
            }

            //Get the metadata ordered correctly
            var cartesianRaw=ftb.CartesianProduct(allVariables);
            if (cartesianRaw.Count() == 0) return matrix;
      

            //Express this as a list of objects. The new variable sequence will be taken from the newSorting dictionary that has been passed in
            List<cartesian> cartesianList = new();
            counter = 0;
            //otherDimensionOrder is a proxy for the sorting due to the dimensions that are not changing
            //The value will increment each time the list of variables cycles by the previous lambda value
            int otherDimensionOrder = 0;
            foreach(var item in cartesianRaw)
            {
                cartesianList.Add(new cartesian() { CellSequence = counter, OtherDimensionOrder = otherDimensionOrder,VrbSequence = item.ElementAt(dimensionAbsoluteOrder).Sequence, NewVrbSequence = newSorting[item.ElementAt(dimensionAbsoluteOrder).Sequence] }) ;
                counter++;

                if (counter % dimension.PreviousLambda == 0) otherDimensionOrder++;
            }

            //Get an ordered list of cells plus their sequence number
            List<numberedCell> numberedCells = new();
            counter = 0;
            foreach(var cell in matrix.Cells)
            {
                numberedCells.Add(new numberedCell() { CellSequence = counter, CellValue =cell });
                counter++;
            }

            //Join the two lists 
            var fullList = from car in cartesianList
                           join cell in numberedCells on car.CellSequence equals cell.CellSequence
                           select new { car.CellSequence,car.OtherDimensionOrder, car.VrbSequence, car.NewVrbSequence, cell.CellValue };

            //Re-order the joined list by the new variable sequence
            fullList =fullList.OrderBy(x=>x.NewVrbSequence).OrderBy(x=>x.OtherDimensionOrder).ToList();

            //Update the metadata
            foreach (var spec in matrix.Dspecs)
            {
                dimension = spec.Value.Dimensions.Where(x => x.Sequence == dimensionSequence).First();
                foreach(var vrb in dimension.Variables)
                {
                    vrb.Sequence = newSorting[vrb.Sequence];
                }
                dimension.Variables = dimension.Variables.OrderBy(x => x.Sequence).ToList();
            }

            matrix.Cells=fullList.Select(x=>x.CellValue).ToList();

            return matrix;
        }

        /// <summary>
        /// Expresses a csv-like input as a List of Dictionary<string,object>
        /// Assumes the header is the first row
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetInputDynamicList(List<List<string>> inputList)
        {
            List<Dictionary<string, object>> dynamicList = new List<Dictionary<string, object>>();
            if (inputList == null) return dynamicList;
            if (inputList.Count == 0) return dynamicList;
            List<string> headerList = inputList[0];
            string vString=Label.Get("xlsx.value");
            int vStringFieldNumber = headerList.IndexOf(vString);
            inputList.RemoveAt(0);
            foreach (var item in inputList)
            {
                Dictionary<string, object> itemDict = new Dictionary<string, object>();
                if (item.Count != headerList.Count) continue;
                int hCount = 0;
                foreach (var column in headerList)
                {
                    if(vStringFieldNumber== hCount && String.IsNullOrWhiteSpace(item[hCount]))
                        itemDict.Add(column, Configuration_BSO.GetStaticConfig("APP_PX_CONFIDENTIAL_VALUE"));
                    else
                        itemDict.Add(column, item[hCount]);
                    hCount++;
                }
                //placeholder for the ordinal
                itemDict.Add("ordinal", -1);
                itemDict.Add("duplicate", false);
                itemDict.Add("updated", false);
                dynamicList.Add(itemDict);
            }
            return dynamicList;
        }

        public List<dlistReport> GetInitialChangeReport(List<List<string>> inputList)
        {
            //List<Dictionary<string, object>> dynamicList = new List<Dictionary<string, object>>();
            List<dlistReport> dlistReport = new List<dlistReport>();
            if (inputList == null) return dlistReport;
            if (inputList.Count == 0) return dlistReport;
            List<string> headerList = inputList[0];
            string vString = Label.Get("xlsx.value");
            int vStringFieldNumber = headerList.IndexOf(vString);
            inputList.RemoveAt(0);
            foreach (var item in inputList)
            {
                Dictionary<string, object> itemDict = new Dictionary<string, object>();
                dlistReport dlp = new dlistReport() { dlist = itemDict };
                if (item.Count != headerList.Count) continue;
                int hCount = 0;
                foreach (var column in headerList)
                {
                    if (vStringFieldNumber == hCount && String.IsNullOrWhiteSpace(item[hCount]))
                        itemDict.Add(column, Configuration_BSO.GetStaticConfig("APP_PX_CONFIDENTIAL_VALUE"));
                    else
                    {
                        
                            itemDict.Add(column, item[hCount]);
                    }
                    hCount++;
                }
                //placeholder for the ordinal
                dlp.dlist.Add("ordinal", -1);
                dlp.dlist.Add("duplicate", false);
                dlp.dlist.Add("updated", false);
                dlistReport.Add(dlp);
                dlp.ordinal = -1;
                dlp.duplicate = false;
                dlp.updated = false;
            }
            return dlistReport;
        }



        /// <summary>
        /// For an item in an expanded list (e.g. a line in a csv input), get the ordinal of the corresponding value in the matrix Cells
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="csvItem"></param>
        /// <returns></returns>
        public Dictionary<int, dynamic> DictionaryWithOrdinal(IDmatrix matrix, List<Dictionary<string, object>> csvItems, string lngIsoCode)
        {
            Dictionary<int, int> ordinalsValues = new Dictionary<int, int>();
            List<Dictionary<string, object>> notUpdated = new List<Dictionary<string, object>>();
            var output = new Dictionary<int, dynamic>();
            IDspec spec = matrix.Dspecs[lngIsoCode];
            List<int> duplicateOrdinalList = new List<int>();
            foreach (var item in csvItems)
            {
                int ordinal = 0;
                bool proceed = true;
                foreach (var dim in spec.Dimensions)
                {
                    if (!item.ContainsKey(dim.Code)) return null;
                    var vrbCode = item[dim.Code].ToString();
                    if (dim.DictionaryVariables.ContainsKey(vrbCode))
                    {
                        var variable = (DimensionVariable)dim.DictionaryVariables[vrbCode];
                        ordinal += ((variable.Sequence * dim.Lambda) - dim.Lambda);
                    }
                    else
                    {
                        
                        proceed = false;
                        break;
                    }
                }
                if (!proceed) continue;

                item["ordinal"] = ordinal + 1;

                string defaultValue = Configuration_BSO.GetStaticConfig("APP_PX_CONFIDENTIAL_VALUE");

                //Check for dupes
                if (output.ContainsKey((int)item["ordinal"]))
                {
                    var outputDupeId = (int)item["ordinal"];
 
                    item["ordinal"] = -1;
                    item["duplicate"] = true;

                    //If we find a duplicate we must also retrospectively flag the original item as duplicate as well
                    var previousDupeItems = csvItems.Where(x => x["ordinal"].Equals(outputDupeId) && !(bool)x["duplicate"]);
                    foreach(var previousDupeItem in previousDupeItems)
                    
                    {
                        previousDupeItem["ordinal"] = -1;
                        previousDupeItem["duplicate"] = true;
                        previousDupeItem["updated"] = false;
                    }
                    //Keep a list of ordinals that are flagged as duplicates
                    if (output.ContainsKey(outputDupeId))
                    {
                        if(!duplicateOrdinalList.Contains(outputDupeId))
                            duplicateOrdinalList.Add(outputDupeId);
                    }
                }
                else
                {
                    if (item["ordinal"].ToString().Equals(defaultValue))
                        output.Add((int)item["ordinal"], null);

                    else if (Double.TryParse(item["VALUE"].ToString(), out Double doubleValue))
                    {
                        output.Add((int)item["ordinal"], doubleValue);
                    }
                    else
                        output.Add((int)item["ordinal"], item["VALUE"]);
                    item["updated"] = true;
                }
            }
            //We don't want duplicates being flagged as cells for update so we remove them
            foreach(var op in duplicateOrdinalList)
            {
                output.Remove(op);
            }
            return output;
        }

        public Dictionary<int, dynamic> DictionaryWithOrdinal(IDmatrix matrix, List<dlistReport> dlistReports, string lngIsoCode)
        {
            Dictionary<int, int> ordinalsValues = new Dictionary<int, int>();
            List<Dictionary<string, object>> notUpdated = new List<Dictionary<string, object>>();
            var output = new Dictionary<int, dynamic>();
            IDspec spec = matrix.Dspecs[lngIsoCode];
            List<int> duplicateOrdinalList = new List<int>();
            foreach (var item in dlistReports)
            {
                int ordinal = 0;
                bool proceed = true;
                foreach (var dim in spec.Dimensions)
                {
                    if (!item.dlist.ContainsKey(dim.Code)) return null;
                    var vrbCode = item.dlist[dim.Code].ToString();
                    if (dim.DictionaryVariables.ContainsKey(vrbCode))
                    {
                        var variable = (DimensionVariable)dim.DictionaryVariables[vrbCode];
                        ordinal += ((variable.Sequence * dim.Lambda) - dim.Lambda);
                    }
                    else
                    {

                        proceed = false;
                        break;
                    }
                }
                if (!proceed) continue;

                
                item.ordinal=ordinal + 1;
                item.dlist["ordinal"] = item.ordinal;

                string defaultValue = Configuration_BSO.GetStaticConfig("APP_PX_CONFIDENTIAL_VALUE"); 

                //Check for dupes
                if (output.ContainsKey((int)item.dlist["ordinal"]))
                {
                    var outputDupeId = (int)item.dlist["ordinal"];

                    item.dlist["ordinal"] = -1;
                    item.ordinal = -1;
                    item.dlist["duplicate"] = true;
                    item.duplicate = true;

                    //If we find a duplicate we must also retrospectively flag the original item as duplicate as well
                    var previousDupeItems = dlistReports.Where(x => x.dlist["ordinal"].Equals(outputDupeId) && !(bool)x.dlist["duplicate"]);
                    foreach (var previousDupeItem in previousDupeItems)

                    {
                        previousDupeItem.dlist["ordinal"] = -1;
                        previousDupeItem.ordinal = -1;
                        previousDupeItem.dlist["duplicate"] = true;
                        previousDupeItem.duplicate=true;
                        previousDupeItem.dlist["updated"] = false;
                        previousDupeItem.updated = false;
                    }
                    //Keep a list of ordinals that are flagged as duplicates
                    if (output.ContainsKey(outputDupeId))
                    {
                        if (!duplicateOrdinalList.Contains(outputDupeId))
                            duplicateOrdinalList.Add(outputDupeId);
                    }
                }
                else
                {
                    if (item.dlist["ordinal"].ToString().Equals(defaultValue))
                        output.Add((int)item.dlist["ordinal"], null);

                    else if (Double.TryParse(item.dlist["VALUE"].ToString(), out Double doubleValue))
                    {
                        output.Add((int)item.dlist["ordinal"], doubleValue);
                    }
                    else
                        output.Add((int)item.dlist["ordinal"], item.dlist["VALUE"]);
                    item.dlist["updated"] = true;
                    item.updated = true;
                }
            }
            //We don't want duplicates being flagged as cells for update so we remove them
            foreach (var op in duplicateOrdinalList)
            {
                output.Remove(op);
            }
            return output;
        }

        /// <summary>
        /// Loads the extra spec properties for a fractal query
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public IDspec LoadDspec(IDmatrix matrix)
        {
            IDspec spec = matrix.Dspecs[matrix.Language];
            int previousLambda = 1;
            foreach (IStatDimension dim in spec.Dimensions)
            {
                previousLambda = previousLambda * dim.Variables.Count;
            }
            foreach (IStatDimension dim in spec.Dimensions)
            {
                dim.Lambda = previousLambda / dim.Variables.Count;
                dim.PreviousLambda = previousLambda;
                previousLambda = dim.Lambda;
            }

            return spec;
        }


        /// <summary>
        /// Adds/updates the Cells with the new cell data
        /// </summary>
        /// <param name="csvCells"></param>
        /// <param name="oldCells"></param>
        /// <param name="mergedCellsCount"></param>
        /// <returns></returns>
        public List<cellMerge> MergeBuildCells(Dictionary<int, object> csvCells, IDmatrix matrix)
        {
            List<cellMerge> mcells = new List<cellMerge>();

            csvCells = csvCells ?? new Dictionary<int, object>();
            List<dynamic> mergedCells = new List<dynamic>();
            int pointer = 1;
            int counter = 1;
            Dictionary<int, dynamic> oldCellDictionary = new Dictionary<int, dynamic>();


            int index = 1;
            oldCellDictionary = matrix.Cells.ToDictionary(ordinal => index++);

            counter = 1;
            for (int i = 0; i < matrix.Dspecs[matrix.Language].GetCellCount(); i++)
            {

                if (csvCells.ContainsKey(counter))
                {
                    mergedCells.Add(csvCells[counter]);
                    mcells.Add(new cellMerge() { ordinal = counter, value = csvCells[counter], updated = !oldCellDictionary[pointer].Equals( csvCells[counter]) });
                }
                else
                {
                    mergedCells.Add(oldCellDictionary[pointer]);
                    mcells.Add(new cellMerge() { ordinal = counter, value = oldCellDictionary[pointer], updated = false });
                }
                pointer++;
                counter++;
            }
            return mcells;

        }

        public List<cellMerge> MergeBuildCells(List<dlistReport> dlistReports, IDmatrix matrix)
        {
            List<cellMerge> mcells = new List<cellMerge>();

            dlistReports = dlistReports ?? new List<dlistReport>();
            List<dynamic> mergedCells = new List<dynamic>();
            int pointer = 1;
            int counter = 1;
            Dictionary<int, dynamic> oldCellDictionary = new Dictionary<int, dynamic>();


            int index = 1;
            oldCellDictionary = matrix.Cells.ToDictionary(ordinal => index++);

            var dlistReportsAsDictionary = dlistReports.Where(y=>y.ordinal>0).ToDictionary(x => x.ordinal, x => x.dlist);

            for (int i = 0; i < matrix.Dspecs[matrix.Language].GetCellCount(); i++)
            {

                if (dlistReportsAsDictionary.ContainsKey(counter))
                {
                    if (Double.TryParse(dlistReportsAsDictionary[counter]["VALUE"].ToString(), out Double dvalue))
                    {
                        mergedCells.Add(dvalue);
                        mcells.Add(new cellMerge() { ordinal = counter, value = dvalue, updated = true });

                    }
                    else
                    {
                        mergedCells.Add(dlistReportsAsDictionary[counter]);
                        mcells.Add(new cellMerge() { ordinal = counter, value = dlistReportsAsDictionary[counter]["VALUE"], updated = true });
                    }
                }
                else
                {
                    mergedCells.Add(oldCellDictionary[pointer]);
                    mcells.Add(new cellMerge() { ordinal = counter, value = oldCellDictionary[pointer], updated = false });
                }
                pointer++;
                counter++;
            }
            return mcells;

        }

        /// <summary>
        /// Updates the matrix with the new variables
        /// Returns a list of ordinals that will feature the new variables
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Dictionary<int, object> UpdateMatrixWithNewMetadata(ref IDmatrix matrix, DBuild_DTO_Update dto, Copyright_BSO cbso, ICopyright icpr = null)
        {
            List<List<IDimensionVariable>> addedVariables = new List<List<IDimensionVariable>>();
            List<StatDimension> updateDims = new List<StatDimension>();
            if (dto.MtrCode != null) matrix.Code = dto.MtrCode;

            if (dto.CprCode != null)
            {
                if (icpr == null)
                {
                    ICopyright cpr = cbso.Read(dto.CprCode);
                    if (cpr != null) matrix.Copyright = cpr;
                }
                else matrix.Copyright = icpr;
            }

            if (dto.MtrOfficialFlag != null)
                matrix.IsOfficialStatistic = (bool)dto.MtrOfficialFlag;

            foreach (var dtospec in dto.Dspecs)
            {
                StatDimension updateDim = new StatDimension();
                if (matrix.Dspecs.ContainsKey(dtospec.Language))
                {
                    foreach (var dim in dtospec.StatDimensions)
                    {
                        if (dim.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))
                        {
                            updateDim = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))?.First();
                            if (dtospec.ContentVariable != null)
                                updateDim.Value = dtospec.ContentVariable;
                        }

                        //Do we have some new data regarding our dimensions?
                        if (matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Value.Equals(dim.Value)).Count() > 0)
                        {
                            updateDim = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Value.Equals(dim.Value)).First();

                            List<IDimensionVariable> addedDimensionVariables = new List<IDimensionVariable>();
                            foreach (var dimVariable in dim.Variables)
                            {
                                if (updateDim.Variables.Where(x => x.Code.Equals(dimVariable.Code)).Count().Equals(0))
                                {
                                    var newVariable = new DimensionVariable()
                                    {
                                        Code = dimVariable.Code,
                                        Value = dimVariable.Value,
                                        AmendFlag = dimVariable.AmendFlag,
                                        Decimals = dimVariable.Decimals,
                                        Elimination = dimVariable.Elimination,
                                        Unit = dimVariable.Unit,
                                        Sequence = updateDim.Variables.Count + 1 // might have to revisit this for out of sequence variables (see below)
                                    };
                                    //We have found a new variable in the request for this dimension
                                    updateDim.Variables.Add(newVariable);
                                    //If this is a time dimension, the sequences must be set by the code
                                    //e.g. if a month is inserted into the the middle of a list rather than at the end
                                    if (updateDim.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME))
                                    {
                                        updateDim.Variables = updateDim.Variables.OrderBy(x => x.Code).ToList();
                                        for (int i = 0; i < updateDim.Variables.Count; i++)
                                        {
                                            updateDim.Variables[i].Sequence = i + 1;
                                        }
                                    }
                                    addedDimensionVariables.Add(newVariable);

                                }
                            }

                            if (addedDimensionVariables.Count > 0)
                            {
                                addedVariables.Add(addedDimensionVariables);
                                updateDims.Add(updateDim);
                            }
                        }

                    }



                    //Update other metadata
                    matrix.Dspecs[dtospec.Language].Title = dtospec.MtrTitle ?? matrix.Dspecs[dtospec.Language].Title;
                   matrix.Dspecs[dtospec.Language].NotesAsString = dtospec.MtrNote ?? matrix.Dspecs[dtospec.Language].NotesAsString;

                    if (dto.Elimination != null)
                    {
                        ICollection<StatDimension> classifications = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION)).ToList();
                        matrix.Dspecs[dtospec.Language].SetEliminationsByCode(ref classifications, dto.Elimination);
                    }

                    if (dto.Map != null)
                    {
                        foreach (var dim in matrix.Dspecs[dtospec.Language].Dimensions)
                        {
                            if (dto.Map.ContainsKey(dim.Code))
                            {
                                var dtoMap = dto.Map[dim.Code];
                                if (dtoMap != null)
                                {
                                    dim.GeoUrl = dto.Map[dim.Code];
                                    dim.GeoFlag = true;
                                }
                                else
                                {
                                    dim.GeoFlag = false;
                                    dim.GeoUrl = null;
                                }

                            }
                            else
                            {
                                dim.GeoFlag = false;
                                dim.GeoUrl = null;
                            }
                        }

                    }

                }

                //Update Notes
                if (dtospec.MtrNote != null)
                {
                    
                    matrix.Dspecs[dtospec.Language].NotesAsString = dtospec.MtrNote;
                    matrix.Dspecs[dtospec.Language].Notes = new List<string>();
                    matrix.Dspecs[dtospec.Language].Notes.Add(dtospec.MtrNote);
                }
              

            }


            ResetLambdas(ref matrix);

            Dictionary<int, object> insertedOrdinals = new Dictionary<int, object>();

            if (updateDims.Count > 0 && addedVariables.Count > 0)
                insertedOrdinals = GetListOfInsertedOrdinals(matrix, updateDims[0], addedVariables[0]);

            return insertedOrdinals;
        }

        public Dictionary<int, object> UpdateMatrixWithNewMetadata(ref IDmatrix matrix, DBuild_DTO_UpdatePublish dto)
        {
            List<List<IDimensionVariable>> addedVariables = new List<List<IDimensionVariable>>();
            List<StatDimension> updateDims = new List<StatDimension>();
            if (dto.MtrCode != null) matrix.Code = dto.MtrCode;


            foreach (var dtospec in dto.Dspecs)
            {
                StatDimension updateDim = new StatDimension();
                if (matrix.Dspecs.ContainsKey(dtospec.Language))
                {
                    foreach (var dim in dtospec.StatDimensions)
                    {
                        if (dim.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))
                        {
                            updateDim = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))?.First();
                            if (dtospec.ContentVariable != null)
                                updateDim.Value = dtospec.ContentVariable;
                        }
                        
                        else if (dim.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME))
                        {
                            dim.Code = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).FirstOrDefault().Code;
                            dim.Value= matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).FirstOrDefault().Value;
                        }

                        //Do we have some new data regarding our dimensions?
                        if (matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Value.Equals(dim.Value)).Count() > 0)
                        {
                            updateDim = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Value.Equals(dim.Value)).First();

                            List<IDimensionVariable> addedDimensionVariables = new List<IDimensionVariable>();
                            foreach (var dimVariable in dim.Variables)
                            {
                                if (updateDim.Variables.Where(x => x.Code.Equals(dimVariable.Code)).Count().Equals(0))
                                {
                                    var newVariable = new DimensionVariable()
                                    {
                                        Code = dimVariable.Code,
                                        Value = dimVariable.Value,
                                        AmendFlag = dimVariable.AmendFlag,
                                        Decimals = dimVariable.Decimals,
                                        Elimination = dimVariable.Elimination,
                                        Unit = dimVariable.Unit,
                                        Sequence = updateDim.Variables.Count + 1 // might have to revisit this for out of sequence variables (see below)
                                    };
                                    //We have found a new variable in the request for this dimension
                                    updateDim.Variables.Add(newVariable);
                                    //If this is a time dimension, the sequences must be set by the code
                                    //e.g. if a month is inserted into the the middle of a list rather than at the end
                                    if (updateDim.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME))
                                    {
                                        updateDim.Variables = updateDim.Variables.OrderBy(x => x.Code).ToList();
                                        for (int i = 0; i < updateDim.Variables.Count; i++)
                                        {
                                            updateDim.Variables[i].Sequence = i + 1;
                                        }
                                    }
                                    addedDimensionVariables.Add(newVariable);

                                }
                            }

                            if (addedDimensionVariables.Count > 0)
                            {
                                addedVariables.Add(addedDimensionVariables);
                                updateDims.Add(updateDim);
                            }
                        }

                    }



                    //Update other metadata
                    matrix.Dspecs[dtospec.Language].Title = dtospec.MtrTitle?? matrix.Dspecs[dtospec.Language].Title;
                    matrix.Dspecs[dtospec.Language].NotesAsString = dtospec.MtrNote ?? matrix.Dspecs[dtospec.Language].NotesAsString;


                }

                //Update Notes
                if (dtospec.MtrNote != null)
                {

                    matrix.Dspecs[dtospec.Language].NotesAsString = dtospec.MtrNote;
                    matrix.Dspecs[dtospec.Language].Notes = new List<string>();
                    matrix.Dspecs[dtospec.Language].Notes.Add(dtospec.MtrNote);
                }


            }


            ResetLambdas(ref matrix);

            Dictionary<int, object> insertedOrdinals = new Dictionary<int, object>();

            if (updateDims.Count > 0 && addedVariables.Count > 0)
                insertedOrdinals = GetListOfInsertedOrdinals(matrix, updateDims[0], addedVariables[0]);

            return insertedOrdinals;
        }

        public Dictionary<int, object> UpdateMatrixWithNewMetadataByRelease(ref IDmatrix matrix, DBuild_DTO_UpdateByRelease dto, Copyright_BSO cbso, ICopyright icpr = null)
        {
            List<List<IDimensionVariable>> addedVariables = new List<List<IDimensionVariable>>();
            List<StatDimension> updateDims = new List<StatDimension>();

            if (dto.CprCode != null)
            {
                if (icpr == null)
                {
                    ICopyright cpr = cbso.Read(dto.CprCode);
                    if (cpr != null) matrix.Copyright = cpr;
                }
                else matrix.Copyright = icpr;
            }

            if (dto.MtrOfficialFlag != null)
                matrix.IsOfficialStatistic = (bool)dto.MtrOfficialFlag;

            foreach (var dtospec in dto.Dspecs)
            {
                StatDimension updateDim = new StatDimension();
                if (matrix.Dspecs.ContainsKey(dtospec.Language))
                {
                    foreach (var dim in dtospec.StatDimensions)
                    {
                        if (dim.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))
                        {
                            updateDim = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))?.First();
                            if (dtospec.ContentVariable != null)
                                updateDim.Value = dtospec.ContentVariable;
                        }

                        //Do we have some new data regarding our dimensions?
                        if (matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Value.Equals(dim.Value)).Count() > 0)
                        {
                            updateDim = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Value.Equals(dim.Value)).First();

                            List<IDimensionVariable> addedDimensionVariables = new List<IDimensionVariable>();
                            foreach (var dimVariable in dim.Variables)
                            {
                                if (updateDim.Variables.Where(x => x.Code.Equals(dimVariable.Code)).Count().Equals(0))
                                {
                                    var newVariable = new DimensionVariable()
                                    {
                                        Code = dimVariable.Code,
                                        Value = dimVariable.Value,
                                        AmendFlag = dimVariable.AmendFlag,
                                        Decimals = dimVariable.Decimals,
                                        Elimination = dimVariable.Elimination,
                                        Unit = dimVariable.Unit,
                                        Sequence = updateDim.Variables.Count + 1 // might have to revisit this for out of sequence variables (see below)
                                    };
                                    //We have found a new variable in the request for this dimension
                                    updateDim.Variables.Add(newVariable);
                                    //If this is a time dimension, the sequences must be set by the code
                                    //e.g. if a month is inserted into the the middle of a list rather than at the end
                                    if (updateDim.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME))
                                    {
                                        updateDim.Variables = updateDim.Variables.OrderBy(x => x.Code).ToList();
                                        for (int i = 0; i < updateDim.Variables.Count; i++)
                                        {
                                            updateDim.Variables[i].Sequence = i + 1;
                                        }
                                    }
                                    addedDimensionVariables.Add(newVariable);

                                }
                            }

                            if (addedDimensionVariables.Count > 0)
                            {
                                addedVariables.Add(addedDimensionVariables);
                                updateDims.Add(updateDim);
                            }
                        }

                    }



                    //Update other metadata
                    matrix.Dspecs[dtospec.Language].Title = dtospec.MtrTitle ?? matrix.Dspecs[dtospec.Language].Title;
                    matrix.Dspecs[dtospec.Language].NotesAsString = dtospec.MtrNote ?? matrix.Dspecs[dtospec.Language].NotesAsString;

                    if (dto.Elimination != null)
                    {
                        ICollection<StatDimension> classifications = matrix.Dspecs[dtospec.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION)).ToList();
                        matrix.Dspecs[dtospec.Language].SetEliminationsByCode(ref classifications, dto.Elimination);
                    }

                    if (dto.Map != null)
                    {
                        foreach (var dim in matrix.Dspecs[dtospec.Language].Dimensions)
                        {
                            if (dto.Map.ContainsKey(dim.Code))
                            {
                                var dtoMap = dto.Map[dim.Code];
                                if (dtoMap != null)
                                {
                                    dim.GeoUrl = dto.Map[dim.Code];
                                    dim.GeoFlag = true;
                                }
                                else
                                {
                                    dim.GeoFlag = false;
                                    dim.GeoUrl = null;
                                }

                            }
                            else
                            {
                                dim.GeoFlag = false;
                                dim.GeoUrl = null;
                            }
                        }

                    }

                }

                //Update Notes
                if (dtospec.MtrNote != null)
                {

                    matrix.Dspecs[dtospec.Language].NotesAsString = dtospec.MtrNote;
                    matrix.Dspecs[dtospec.Language].Notes = new List<string>();
                    matrix.Dspecs[dtospec.Language].Notes.Add(dtospec.MtrNote);
                }


            }


            ResetLambdas(ref matrix);

            Dictionary<int, object> insertedOrdinals = new Dictionary<int, object>();

            if (updateDims.Count > 0 && addedVariables.Count > 0)
                insertedOrdinals = GetListOfInsertedOrdinals(matrix, updateDims[0], addedVariables[0]);

            return insertedOrdinals;
        }


        /// <summary>
        /// Return an indexed list of blank cells, indexed with their ordinals, for the added variables
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="statDim"></param>
        /// <param name="newVariables"></param>
        /// <returns></returns>
        private Dictionary<int, dynamic> GetListOfInsertedOrdinals(IDmatrix matrix, IStatDimension statDim, List<IDimensionVariable> newVariables)
        {
            Dictionary<int, dynamic> insertedOrdinals = new Dictionary<int, dynamic>();
            var spec = matrix.Dspecs.First().Value;

            int previousVrbCount = spec.GetCellCount() / statDim.PreviousLambda;
            foreach (var newVrb in newVariables)
            {
                for (int i = 0; i < previousVrbCount; i++)
                {
                    int basePosition = i * (statDim.Lambda * statDim.Variables.Count);
                    int startPosition = basePosition + 1 + (newVrb.Sequence - 1) * statDim.Lambda;
                    for (int j = 0; j < statDim.Lambda; j++)
                    {
                        insertedOrdinals.Add(j + startPosition, Configuration_BSO.GetStaticConfig("APP_PX_CONFIDENTIAL_VALUE"));
                    }
                }
            }

            return insertedOrdinals;
        }

        /// <summary>
        /// Reset the lambdas for each dimension after addition of a new variable etc
        /// </summary>
        /// <param name="dmatrix"></param>
        private void ResetLambdas(ref IDmatrix dmatrix)
        {
            foreach (var spec in dmatrix.Dspecs)
            {
                int previousLambda = 1;
                foreach (IStatDimension dim in spec.Value.Dimensions)
                {
                    previousLambda = previousLambda * dim.Variables.Count;
                }
                foreach (IStatDimension dim in spec.Value.Dimensions)
                {
                    dim.Lambda = previousLambda / dim.Variables.Count;
                    dim.PreviousLambda = previousLambda;
                    previousLambda = dim.Lambda;
                }
            }
        }

        private void ResetLambdasPlusVariableFinder(ref IDmatrix dmatrix)
        {
            foreach (var spec in dmatrix.Dspecs)
            {
                int previousLambda = 1;
                foreach (IStatDimension dim in spec.Value.Dimensions)
                {
                    previousLambda = previousLambda * dim.Variables.Count;
                }
                foreach (IStatDimension dim in spec.Value.Dimensions)
                {
                    dim.Lambda = previousLambda / dim.Variables.Count;
                    dim.PreviousLambda = previousLambda;
                    previousLambda = dim.Lambda;

                    //Now set the fast find field for each variable:
                    foreach(var vrb in dim.Variables)
                    {
                        vrb.FastFindValue = dim.Lambda * (vrb.Sequence - 1);
                    }

                }
            }
        }


        /// <summary>
        /// Recreate the list of Cells with the added cells from the new variables
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="updatedOrdinals"></param>
        /// <returns></returns>
        public List<dynamic> MergeOldAndNewCells(IDmatrix matrix, Dictionary<int, dynamic> updatedOrdinals)
        {
            List<dynamic> allCells = new List<dynamic>();
            int index = 1;
            var oldCellDictionary = matrix.Cells.ToDictionary(ordinal => index++);
            int counter = 1;
            for (int i = 1; i <= (oldCellDictionary.Count + updatedOrdinals.Count); i++)
            {
                if (updatedOrdinals.ContainsKey(i))
                    allCells.Add(updatedOrdinals[i]);
                else
                {
                    allCells.Add(oldCellDictionary[counter]);
                    counter++;
                }

            }
            return allCells;
        }

        /// <summary>
        /// Tests if the user has sufficient build permissions
        /// </summary>
        /// <param name="PrvCode"></param>
        /// <param name="BuildAction"></param>
        /// <returns></returns>
        internal bool HasBuildPermission(string CcnUsername, string BuildAction)
        {
            Account_ADO adoAccount = new Account_ADO();
            ADO_readerOutput result = null;
            using (IADO ado = AppServicesHelper.StaticADO)
            {
                result = adoAccount.Read(ado, CcnUsername);
            }

            if (!result.hasData) return false;
            if (result.data == null) return false;
            if (result.data.Count == 0) return false;

            if (result.data[0].PrvCode.Equals(Constants.C_SECURITY_PRIVILEGE_MODERATOR))
            {
                return Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "build." + BuildAction + ".moderator");
            }
            return true;
        }

        /// <summary>
        /// Maps the DBuild_DTO_Update to a CubeQuery_DTO so that the matrix may be queried
        /// </summary>
        /// <param name="bDto"></param>
        /// <param name="dmatrix"></param>
        /// <returns></returns>
        public CubeQuery_DTO MapReadToQueryDto(DBuild_DTO_Update bDto, IDmatrix dmatrix)
        {

            CubeQuery_DTO qDto = new CubeQuery_DTO();
            qDto.jStatQuery = new JsonQuery.JsonStatQuery();
            qDto.jStatQuery.Id = new List<string>();
            qDto.jStatQuery.Dimensions = new Dictionary<string, JsonQuery.Dimension>();

            foreach (var dim in bDto.Dspecs[0].StatDimensions)
            {
                var mtrDim = dmatrix.Dspecs[bDto.Dspecs[0].Language].Dimensions.Where(x => x.Value.Equals(dim.Value)).FirstOrDefault();

                dim.Code = mtrDim.Code;
                if (dim.Code != null)
                {
                    qDto.jStatQuery.Id.Add(dim.Code);
                    foreach (var vrb in dim.Variables)
                    {
                        vrb.Code = mtrDim.Variables.Where(x => x.Value.Equals(vrb.Value))?.FirstOrDefault().Code;
                    }
                    qDto.jStatQuery.Dimensions.Add(dim.Code, new JsonQuery.Dimension() { Id = dim.Code, Category = new JsonQuery.Category() { Index = dim.Variables.Select(x => x.Code).ToList() } });
                }
            }
            return qDto;
        }

        public CubeQuery_DTO MapReadToQueryDto(DBuild_DTO_UpdateByRelease bDto, IDmatrix dmatrix)
        {

            CubeQuery_DTO qDto = new CubeQuery_DTO();
            qDto.jStatQuery = new JsonQuery.JsonStatQuery();
            qDto.jStatQuery.Id = new List<string>();
            qDto.jStatQuery.Dimensions = new Dictionary<string, JsonQuery.Dimension>();

            foreach (var dim in bDto.Dspecs[0].StatDimensions)
            {
                var mtrDim = dmatrix.Dspecs[bDto.Dspecs[0].Language].Dimensions.Where(x => x.Value.Equals(dim.Value)).FirstOrDefault();

                dim.Code = mtrDim.Code;
                if (dim.Code != null)
                {
                    qDto.jStatQuery.Id.Add(dim.Code);
                    foreach (var vrb in dim.Variables)
                    {
                        vrb.Code = mtrDim.Variables.Where(x => x.Value.Equals(vrb.Value))?.FirstOrDefault().Code;
                    }
                    qDto.jStatQuery.Dimensions.Add(dim.Code, new JsonQuery.Dimension() { Id = dim.Code, Category = new JsonQuery.Category() { Index = dim.Variables.Select(x => x.Code).ToList() } });
                }
            }
            return qDto;
        }

        public IDmatrix Create(DBuild_DTO_Create dto, ICopyright cpr = null)
        {
            IDmatrix dmatrix = new Dmatrix()
            {
                Language = dto.LngIsoCode,
                FormatType = dto.Format.FrmType,
                FormatVersion = dto.Format.FrmVersion,
                Code = dto.MtrCode,
                CreatedDateTime = DateTime.Now,
                Dspecs = new Dictionary<string, Dspec>(),
                IsOfficialStatistic = dto.MtrOfficialFlag ?? false,
                Languages = dto.Dspecs.Select(x => x.Language).ToList(),
                Copyright = cpr == null ? new Copyright_BSO().Read(dto.CprCode) : cpr
            };

            foreach (var spec in dto.Dspecs)
            {
                Dspec dspec = new Dspec()
                {
                    Contents = spec.Contents,
                    ContentVariable = spec.ContentVariable,
                    Language = spec.Language,
                    CopyrightUrl = spec.CopyrightUrl,
                    Decimals = spec.Decimals,
                    MatrixCode = spec.MatrixCode,
                    NotesAsString = spec.MtrNote, 
                    Title = spec.MtrTitle,
                    Source = dmatrix.Copyright.CprValue,
                    Dimensions = new List<StatDimension>()
                };
                if(dspec.Notes==null)
                {
                    dspec.Notes= new List<string>();
                    dspec.Notes.Add(spec.MtrNote);
                }
                
                int dimSequence = 0;
                foreach (var dim in spec.StatDimensions)
                {
                    StatDimension sdim = (new StatDimension()
                    {
                        Code = dim.Code,
                        Value = dim.Value,
                        Role = dim.Role,
                        GeoFlag = dim.GeoFlag,
                        GeoUrl = dim.GeoUrl,
                        Sequence = ++dimSequence,
                        Variables = new List<IDimensionVariable>()
                    });
                    int vrbSequence = 0;
                    foreach (var vrb in dim.Variables)
                    {
                        sdim.Variables.Add(new DimensionVariable()
                        {
                            Sequence = ++vrbSequence,
                            Code = vrb.Code,
                            Value = vrb.Value,
                            Unit = vrb.Unit,
                            AmendFlag = vrb.AmendFlag,
                            Decimals = vrb.Decimals,
                            Elimination = vrb.Elimination
                        });
                    }

                    dspec.Dimensions.Add(sdim);
                }
                dmatrix.Dspecs.Add(dspec.Language, dspec);
            }

            int cellCount = dmatrix.Dspecs[dmatrix.Language].GetCellCount();
            dmatrix.Cells = new List<dynamic>();
            for (int i = 0; i < cellCount; i++)
            {
                dmatrix.Cells.Add(null);
            }
            //This is a new matrix - get the lambda information for calculating precisions etc
            ResetLambdas(ref dmatrix);
            return dmatrix;
        }

        public void Dispose()
        {

        }

        public IResponseOutput BsoUpdate(IResponseOutput response, DBuild_DTO_Update DTO,  IADO ado=null, bool validatePxDoc = true, ICopyright cpr = null, string lngIsoCode=null)
        {
            if (ado == null) ado = AppServicesHelper.StaticADO;

            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(DTO.MtrInput);
            var pxDocument = pxManualParser.Parse();

            if (validatePxDoc)
            {
                //validate the px document
                PxSchemaValidator psv = new PxSchemaValidator();

                var pxValidation = psv.Validate(pxDocument);
                if (!pxValidation.IsValid)
                {
                    response.error = Error.GetValidationFailure(pxValidation.Errors);
                    return response;
                }
            }

            //Get the basic matrix from the px data
            IDmatrix dmatrix = new Dmatrix();
            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = DTO.FrqValueTimeval, LngIsoCode = DTO.LngIsoCode, FrqCodeTimeval = DTO.FrqCodeTimeval };
            List<PxUpload_DTO> uploads = new List<PxUpload_DTO>();
            if(DTO.Dspecs.Count>1)
            {
                foreach(var spc in DTO.Dspecs)
                {
                    var tdim = spc.StatDimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).FirstOrDefault();
                    uploads.Add(new PxUpload_DTO() { FrqValueTimeval=tdim.Value, LngIsoCode=spc.Language, FrqCodeTimeval=tdim.Code });
                }
            }
            if (uploads.Count == 0) uploads = null;
            List<string> headerData = null;
            if (DTO.ChangeData?.Count > 0)
            {
                headerData = DTO.ChangeData[0];
            }

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto,uploads);

            ResetLambdasPlusVariableFinder(ref dmatrix);


            var dlist = this.GetInitialChangeReport(DTO.ChangeData);

            //Sort the time dimension (along with associated data) if necessary
            var timeDimension = dmatrix.Dspecs[dmatrix.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).First();

            if (timeDimension != null)
            {
                if (!AreVariablesSequential(timeDimension))
                {
                    timeDimension.Variables = timeDimension.Variables.OrderBy(x => x.Code).ToList();

                    Dictionary<int, int> sequenceDictionary = new Dictionary<int, int>();
                    int counter = 1;
                    foreach (var vrb in timeDimension.Variables)
                    {
                        sequenceDictionary.Add(vrb.Sequence, counter);
                        counter++;
                    }
                    dmatrix = SortVariablesInDimension(dmatrix, sequenceDictionary, timeDimension.Sequence);
                }
            }

            //check for a dodgy csv header

            if (!CustomValidations.ValidateCsvHeader(DTO, dmatrix, headerData))
            {
                response.error = Label.Get("error.validation");
                return response;
            }

            //Update the metadata
            var updateOrdinals = this.UpdateMatrixWithNewMetadata(ref dmatrix, DTO, new Copyright_BSO(), cpr);

                //Update the data
                dmatrix.Cells = this.MergeOldAndNewCells(dmatrix, updateOrdinals);

                dmatrix.Dspecs[DTO.LngIsoCode].GetDimensionsAsDictionary();

                //We must also get the ordindals for the csv originated cells
                Dictionary<int, object> csvCells = this.DictionaryWithOrdinal(dmatrix, dlist, DTO.LngIsoCode);

                var amendedData = this.MergeBuildCells(dlist, dmatrix);

                dmatrix.Cells = amendedData.Select(x=>x.value).ToList();



            refMatrix = dmatrix;

            // Validate the matrix we just created
            DMatrix_VLD validator = new DMatrix_VLD( ado, lngIsoCode);
            // Also validate in english - just for the logs
            DMatrix_VLD dmvEn = new DMatrix_VLD( ado);
            dmvEn.Validate(dmatrix);
            var matrixValidation = validator.Validate(dmatrix);
            if (!matrixValidation.IsValid)
            {
                response.error = Error.GetValidationFailure(matrixValidation.Errors);
                return response;
            }

            if (DTO.Map != null)
            {
                foreach (var spec in dmatrix.Dspecs)
                {
                    spec.Value.ValidateMaps(true);
                    if (spec.Value.ValidationErrors == null) continue;
                    if (spec.Value.ValidationErrors.Count > 0)
                    {
                        response.error = spec.Value.ValidationErrors[0].ErrorMessage;
                        return response;
                    }
                }
            }
            Dictionary<int, bool> dupeReport = new Dictionary<int, bool>();
            int dupecounter = 0;
            foreach(var item in dlist.Select(x => x.dlist).ToList())
            {
                if (item.ContainsKey("duplicate"))
                    dupeReport.Add(dupecounter, (bool)item["duplicate"]);
                else
                    dupeReport.Add(dupecounter, false);
                dupecounter++;
            }
            dynamic result = new ExpandoObject();
            if (DTO.Format.FrmType == DatasetFormat.Px)
            {
                PxFileBuilder pxb = new PxFileBuilder();
                string px = pxb.Create(dmatrix, DTO.LngIsoCode);

                List<string> file = new List<string>
                {
                   px
                };
                result.file = file;

                var header = GetHeader(dmatrix, DTO.LngIsoCode);


                result.report = GetOutputReportCsv(dlist,amendedData);
                response.data = result;

                return response;
            }
            else if (DTO.Format.FrmType == DatasetFormat.JsonStat)
            {
                JsonStatBuilder2_0 jxb = new JsonStatBuilder2_0();
                var jsonStat = jxb.Create(dmatrix, dmatrix.Language);
                var data = new JRaw(Serialize.ToJson(jsonStat));

                result.file = data;

                var header = GetHeader(dmatrix, DTO.LngIsoCode);
                result.report = GetOutputReportCsv(dlist,amendedData);

                response.data = result;
            }
            return response;
        }

        public IDmatrix BsoUpdate(IDmatrix dmatrix, IResponseOutput response, DBuild_DTO_UpdatePublish DTO, IADO ado = null,  ICopyright cpr = null, string lngIsoCode = null)
        {
            if (ado == null) ado = AppServicesHelper.StaticADO;

            
           
            List<PxUpload_DTO> uploads = new List<PxUpload_DTO>();
            if (DTO.Dspecs.Count > 1)
            {
                foreach (var spc in DTO.Dspecs)
                {
                    var tdim = spc.StatDimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).FirstOrDefault();
                    uploads.Add(new PxUpload_DTO() { FrqValueTimeval = tdim.Value, LngIsoCode = spc.Language, FrqCodeTimeval = tdim.Code });
                }
            }
            if (uploads.Count == 0) uploads = null;
            List<string> headerData = null;
            if (DTO.ChangeData?.Count > 0)
            {
                headerData = DTO.ChangeData[0];
            }


            var dlist = this.GetInitialChangeReport(DTO.ChangeData);


            //Sort the time dimension (along with associated data) if necessary
            var timeDimension = dmatrix.Dspecs[dmatrix.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).First();

            if (timeDimension != null)
            {
                if (!AreVariablesSequential(timeDimension))
                {
                    timeDimension.Variables = timeDimension.Variables.OrderBy(x => x.Code).ToList();

                    Dictionary<int, int> sequenceDictionary = new Dictionary<int, int>();
                    int counter = 1;
                    foreach (var vrb in timeDimension.Variables)
                    {
                        sequenceDictionary.Add(vrb.Sequence, counter);
                        counter++;
                    }
                    dmatrix = SortVariablesInDimension(dmatrix, sequenceDictionary, timeDimension.Sequence);
                }
            }

            //check for a dodgy csv header

            //Get a DTO update/publish version of this:
            if (!CustomValidations.ValidateCsvHeader(DTO, dmatrix, headerData))
            {
                response.error = Label.Get("error.validation");
                return dmatrix;
            }

            //Update the metadata
            var updateOrdinals = this.UpdateMatrixWithNewMetadata(ref dmatrix, DTO);

            //Update the data
            dmatrix.Cells = this.MergeOldAndNewCells(dmatrix, updateOrdinals);

            dmatrix.Dspecs[DTO.LngIsoCode].GetDimensionsAsDictionary();

            //We must also get the ordindals for the csv originated cells
            Dictionary<int, object> csvCells = this.DictionaryWithOrdinal(dmatrix, dlist, DTO.LngIsoCode);

            var amendedData = this.MergeBuildCells(dlist, dmatrix);

            dmatrix.Cells = amendedData.Select(x => x.value).ToList();



            refMatrix = dmatrix;

            // Validate the matrix we just created
            DMatrix_VLD validator = new DMatrix_VLD(ado, lngIsoCode);
            // Also validate in english - just for the logs
            DMatrix_VLD dmvEn = new DMatrix_VLD(ado);
            dmvEn.Validate(dmatrix);
            var matrixValidation = validator.Validate(dmatrix);
            if (!matrixValidation.IsValid)
            {
                response.error = Error.GetValidationFailure(matrixValidation.Errors);
                return dmatrix;
            }


            Dictionary<int, bool> dupeReport = new Dictionary<int, bool>();
            int dupecounter = 0;
            foreach (var item in dlist.Select(x => x.dlist).ToList())
            {
                if (item.ContainsKey("duplicate"))
                    dupeReport.Add(dupecounter, (bool)item["duplicate"]);
                else
                    dupeReport.Add(dupecounter, false);
                dupecounter++;
            }
            return dmatrix;
        }

        public IResponseOutput BsoUpdateByRelease(IDmatrix dmatrix, IResponseOutput response, DBuild_DTO_UpdateByRelease DTO, IADO ado = null, ICopyright cpr = null, string lngIsoCode = null)
        {
            if (ado == null) ado = AppServicesHelper.StaticADO;



            List<PxUpload_DTO> uploads = new List<PxUpload_DTO>();
            if (DTO.Dspecs.Count > 1)
            {
                foreach (var spc in DTO.Dspecs)
                {
                    var tdim = spc.StatDimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).FirstOrDefault();
                    uploads.Add(new PxUpload_DTO() { FrqValueTimeval = tdim.Value, LngIsoCode = spc.Language, FrqCodeTimeval = tdim.Code });
                }
            }
            if (uploads.Count == 0) uploads = null;
            List<string> headerData = null;
            if (DTO.ChangeData?.Count > 0)
            {
                headerData = DTO.ChangeData[0];
            }


            var dlist = this.GetInitialChangeReport(DTO.ChangeData);


            //Sort the time dimension (along with associated data) if necessary
            var timeDimension = dmatrix.Dspecs[dmatrix.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).First();

            if (timeDimension != null)
            {
                if (!AreVariablesSequential(timeDimension))
                {
                    timeDimension.Variables = timeDimension.Variables.OrderBy(x => x.Code).ToList();

                    Dictionary<int, int> sequenceDictionary = new Dictionary<int, int>();
                    int counter = 1;
                    foreach (var vrb in timeDimension.Variables)
                    {
                        sequenceDictionary.Add(vrb.Sequence, counter);
                        counter++;
                    }
                    dmatrix = SortVariablesInDimension(dmatrix, sequenceDictionary, timeDimension.Sequence);
                }
            }

            //check for a dodgy csv header

            //Get a DTO update/publish version of this:
            if (!CustomValidations.ValidateCsvHeader(DTO, dmatrix, headerData))
            {
                response.error = Label.Get("error.validation");
                return response;
            }

            //Update the metadata
            var updateOrdinals = this.UpdateMatrixWithNewMetadataByRelease(ref dmatrix, DTO, new Copyright_BSO(), cpr);

            //Update the data
            dmatrix.Cells = this.MergeOldAndNewCells(dmatrix, updateOrdinals);

            dmatrix.Dspecs[DTO.LngIsoCode].GetDimensionsAsDictionary();

            //We must also get the ordindals for the csv originated cells
            Dictionary<int, object> csvCells = this.DictionaryWithOrdinal(dmatrix, dlist, DTO.LngIsoCode);

            var amendedData = this.MergeBuildCells(dlist, dmatrix);

            dmatrix.Cells = amendedData.Select(x => x.value).ToList();

            // Filter values from the dmatrix
            dmatrix = FilterMatrix(dmatrix);

            refMatrix = dmatrix;

            // Validate the matrix we just created
            DMatrix_VLD validator = new DMatrix_VLD(ado, lngIsoCode);
            // Also validate in english - just for the logs
            DMatrix_VLD dmvEn = new DMatrix_VLD(ado);
            dmvEn.Validate(dmatrix);
            var matrixValidation = validator.Validate(dmatrix);
            if (!matrixValidation.IsValid)
            {
                response.error = Error.GetValidationFailure(matrixValidation.Errors);
                return response;
            }


            Dictionary<int, bool> dupeReport = new Dictionary<int, bool>();
            int dupecounter = 0;
            foreach (var item in dlist.Select(x => x.dlist).ToList())
            {
                if (item.ContainsKey("duplicate"))
                    dupeReport.Add(dupecounter, (bool)item["duplicate"]);
                else
                    dupeReport.Add(dupecounter, false);
                dupecounter++;
            }
            dynamic result = new ExpandoObject();
            if (DTO.Format.FrmType == DatasetFormat.Px)
            {
                PxFileBuilder pxb = new PxFileBuilder();
                string px = pxb.Create(dmatrix, DTO.LngIsoCode);

                List<string> file = new List<string>
                {
                   px
                };
                result.file = file;

                var header = GetHeader(dmatrix, DTO.LngIsoCode);


                result.report = GetOutputReportCsv(dlist, amendedData);
                response.data = result;

                return response;
            }
            else if (DTO.Format.FrmType == DatasetFormat.JsonStat)
            {
                JsonStatBuilder2_0 jxb = new JsonStatBuilder2_0();
                var jsonStat = jxb.Create(dmatrix, dmatrix.Language);
                var data = new JRaw(Serialize.ToJson(jsonStat));

                result.file = data;

                var header = GetHeader(dmatrix, DTO.LngIsoCode);
                result.report = GetOutputReportCsv(dlist, amendedData);

                response.data = result;
            }
            return response;
        }

        /// <summary>
        /// Filters the SUBJECT-AREA, SUBJECT-CODE and LAST-UPDATED fields of the dmatrix, matrix
        /// </summary>
        /// <param name="dmatrix"></param>
        /// <returns></returns>
        public IDmatrix FilterMatrix(IDmatrix dmatrix)
        {
            dmatrix.Release.RlsLiveDatetimeFrom = default;

            foreach (var spec in dmatrix.Dspecs)
            {
                switch (spec.Value.Language)
                {
                    case "en":
                        spec.Value.PrcValue = "n/a";
                        dmatrix.Release.PrcCode = "NA";                       
                        break;
                    case "ga":
                        spec.Value.PrcValue = "n/b";
                        break;
                    default:
                        spec.Value.PrcValue = "n/a";
                        dmatrix.Release.PrcCode = "NA";
                        break;
                }                   
            }
            return dmatrix;
        }

        private List<dynamic> GetHeader(IDmatrix dmatrix, string lngIsoCode)
        {
            var dspec = dmatrix.Dspecs[lngIsoCode];
            List<dynamic> hList = new List<dynamic>();
            foreach (var dim in dspec.Dimensions)
            {
                hList.Add(dim.Code);
            }
            hList.Add(Label.Get("default.csv.value"));
            hList.Add("updated");
            hList.Add("duplicate");
            return hList;
        }



        private List<List<dynamic>> GetOutputReportCsv( List<dlistReport> dlist,List<cellMerge> amendedData)
        {
            List<List<dynamic>> output = new List<List<dynamic>>();

            var amendedReportDictionary = amendedData.ToDictionary(x => x.ordinal, x => x.updated);

            List < dynamic > header = new List<dynamic>();
            if (dlist.Count > 0)
            {
                Dictionary<string, object> headerDict = dlist[0].dlist;
                header.AddRange(headerDict.Keys.Where(x=>!x.Equals("ordinal")));
            }
            else return output;
            output.Add(header);
            foreach (var rep in dlist)
            {
                if (amendedReportDictionary.ContainsKey(rep.ordinal))
                    rep.dlist["updated"] = amendedReportDictionary[rep.ordinal];
                List<dynamic> line = new List<dynamic>();
                Dictionary<string, object> smallRep = new Dictionary<string, object>();
                var smallDict=rep.dlist.Where(x => !x.Key.Equals("ordinal"));
                line.AddRange(smallDict.Select(x=>x.Value));
                output.Add(line);
            }


            return output;
        }

            private List<List<dynamic>> GetOutputReportCsv(List<dynamic> header,List<cellMerge> cellMergeList,Dictionary<int,bool> dupeReport, List<Dictionary<string, object>> report)
        {
            var cm=cellMergeList.Where(x=>x.updated).ToList();  
            var dr=dupeReport.Where(x=>x.Value==true).ToList();   
            long maxTime = 0;
            List<List<dynamic>> output = new List<List<dynamic>>();
            if (report.Count == 0) return output;

            if (cellMergeList.Count > 0) output.Add(header);
            int cCounter = 0;
            foreach (var rep  in report)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                bool? isUpdated = null;
                bool isDupe = false;
                List<dynamic> lineList = new List<dynamic>();
                foreach (var column in header)
                {
                    if (column.Equals("updated"))
                    {
                        var ordinalCell = cellMergeList.Where(x => x.ordinal.Equals((int)rep["ordinal"])).FirstOrDefault();
                        if (ordinalCell == null) //This was invalid so it is flagged as not being updated
                        {
                            isUpdated = false;
                            lineList.Add(false);
                        }
                        else
                        {
                            if (ordinalCell.updated) isUpdated = true;
                            lineList.Add(ordinalCell.updated);
                        }
                    }
                    else if (column.Equals("duplicate"))
                    {
                        lineList.Add(rep["duplicate"]);
                        isDupe = (bool)rep["duplicate"];
                    }
                    else
                    {
                        var dictValue = rep[column];
                        if (dictValue != null)
                            lineList.Add(dictValue);
                        else
                            lineList.Add(false);
                    }
                }
                if(isUpdated==null)
                {
                    if (isDupe) output.Add(lineList);
                }
                else
                    output.Add(lineList);
                cCounter++;
                sw.Stop();
                var l = sw.ElapsedMilliseconds;
                if (l > maxTime) maxTime = l;
            }
            
            if(output.Count==1) return new List<List<dynamic>>();
            return output;
        }

            private List<List<dynamic>> GetOutputReportCsv(List<dynamic> header, List<Dictionary<string, object>> report)
        {
            List<List<dynamic>> output = new List<List<dynamic>>();

            if (report.Count > 0) output.Add(header);
            foreach (Dictionary<string, object> line in report)
            {
                List<dynamic> lineList = new List<dynamic>();
                foreach (var column in header)
                {
                    if (column.Equals("updated"))
                    {
                        lineList.Add(line["ordinal"].ToString().Equals("-1") ? false : true);
                    }
                    else if (column.Equals("duplicate"))
                        lineList.Add(line["duplicate"].ToString().ToLower().Equals("true") ? true : false);
                    else
                    {
                        if (line.ContainsKey(column))
                            lineList.Add(line[column].ToString());
                    }
                }

                output.Add(lineList);
            }

            return output;
        }

        /// <summary>
        /// Run the Build ReadDataset
        /// </summary>
        /// <param name="pxDocument"></param>
        /// <param name="dto"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public IDmatrix ReadDataset(PxDocument pxDocument, DBuild_DTO_Update dto)
        {


            //Get the basic matrix from the px data
            IDmatrix dmatrix = new Dmatrix();
            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"), FrqCodeTimeval = dto.FrqCodeTimeval };
            



            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);

            //Update the metadata
            var updateOrdinals = this.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  new Copyright_BSO());

            //Update the data
            dmatrix.Cells = this.MergeOldAndNewCells(dmatrix, updateOrdinals);

            dmatrix.Dspecs[dto.LngIsoCode].GetDimensionsAsDictionary();


            Dictionary<int, object> dict = this.DictionaryWithOrdinal(dmatrix, new List<Dictionary<string, object>>(), dto.LngIsoCode);

            var amendedData = this.MergeBuildCells(dict, dmatrix);
            dmatrix.Cells = amendedData.Select(x=>x.value).ToList();

            //Query the matrix with the dimension data..

            DataReader dr = new DataReader();

            //Get a query by mapping parts of the existing DTO to a CubeQuery_DTO
            CubeQuery_DTO query = null;
            using (var b = new DBuild_BSO())
            {
                query = b.MapReadToQueryDto(dto, dmatrix);
            }

            dmatrix = dr.QueryDataset(query, dmatrix);

            return dmatrix;
        }
        public DataTable GetSimpleCsv(IDmatrix dmatrix)
        {
            DataTable dt = new DataTable();

            return dt;
        }


        public void ValidateSpecMap(IDspec spec, bool formatValidationOnly = false)
        {
            List<StatDimension> clsDimensions = spec.Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION)).ToList();
            if (spec.ValidationErrors == null) spec.ValidationErrors = new List<ValidationFailure>();
            foreach (StatDimension cls in clsDimensions)
            {
                if (cls.GeoFlag)
                {
                    if (!Regex.IsMatch(cls.GeoUrl, Configuration_BSO.GetStaticConfig("APP_REGEX_URL")))
                    {

                        spec.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.invalid-format", spec.Language)));
                        return;
                    }
                    if (formatValidationOnly) return;
                    GeoJson geoJson = new GeoJson();
                    using (GeoMap_BSO gBso = new GeoMap_BSO(AppServicesHelper.StaticADO))
                    {
                        string geoCode = cls.GeoUrl;
                        //Accept only the rightmost 32 characters
                        if (cls.GeoUrl.Length > 32)
                        {
                            geoCode = cls.GeoUrl.Substring(cls.GeoUrl.Length - 32);
                            string baseUrl = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.static") + "/PxStat.Data.GeoMap_API.Read/";
                            if (!cls.GeoUrl.Substring(0, cls.GeoUrl.Length - 32).Equals(baseUrl))
                            {
                                spec.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.not-found", spec.Language)));
                                return;

                            }
                        }
                        var mapData = gBso.Read(geoCode);
                        if (mapData != null)
                        {
                            try
                            {
                                geoJson = JsonConvert.DeserializeObject<GeoJson>(mapData.data[0].GmpGeoJson);
                            }
                            catch
                            {
                                spec.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.json-parse", spec.Language)));
                                return;
                            }
                            foreach (var feature in geoJson.Features)
                            {
                                if (!feature.Properties.ContainsKey("code"))
                                {
                                    spec.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.code-tag", spec.Language)));
                                    return;
                                }
                            }
                            foreach (var vrb in cls.Variables)
                            {

                                if (!vrb.Elimination)

                                {
                                    if (geoJson.Features.Where(x => x.Properties["code"] == vrb.Code).Count() == 0)
                                    {
                                        spec.ValidationErrors.Add(new ValidationFailure("GeoJson", String.Format(Label.Get("error.geomap.unmapped-variable", spec.Language), vrb.Code)));
                                        return;

                                    }
                                }


                            }
                        }
                        else
                        {

                            spec.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.not-found", spec.Language)));
                            return;

                        }
                    }
                }
            }
        }

        private IDspec LoadDspec(IDspec spec)
        {
            int previousLambda = 1;
            foreach (var dim in spec.Dimensions)
            {
                previousLambda = previousLambda * dim.Variables.Count;
            }
            foreach (IStatDimension dim in spec.Dimensions)
            {
                dim.Lambda = previousLambda / dim.Variables.Count;
                dim.PreviousLambda = previousLambda;
                previousLambda = dim.Lambda;
            }

            return spec;
        }


    }
    public class cellMerge
    {
        public int ordinal { get; set; }
        public dynamic value { get; set; }
        public bool updated { get; set; }
    }

    public class dlistReport
    {
        public Dictionary<string,object> dlist { get; set; }
        public int ordinal { get; set; }
        public bool duplicate { get; set; }
        public bool updated { get; set; }
    }
    /*CellSequence = counter, VrbSequence = item.ElementAt(dimensionIndex).Sequence, NewVrbSequence*/
    public class cartesian
    {
       public int CellSequence { get; set; }
        public int VrbSequence { get; set; }
        public int NewVrbSequence { get; set; }
        public int OtherDimensionOrder { get; set; }
    }

    public class numberedCell
    {
        public int CellSequence { get; set; }
        public dynamic CellValue { get; set; }
    }
}
