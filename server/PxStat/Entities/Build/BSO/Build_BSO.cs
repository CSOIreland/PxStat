
using API;
using Newtonsoft.Json;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using static PxStat.Data.Matrix;

namespace PxStat.Build
{
    /// <summary>
    /// General purpose class for building Px files
    /// </summary>
    internal class Build_BSO
    {
        internal class MarkedCell
        {
            internal dynamic Cell { get; set; }
            internal bool Marked { get; set; }
        }
        allDTO AllMergeLists = new allDTO();

        /// <summary>
        /// Queries a Matrix to return a full matrix with a subset of the data and metadata
        /// dataMatrix is the full matrix to be queried
        /// queryMatrix is a matrix that only contains the metadata we are interested in and contains no data cells
        /// The function will return dataMatrix with the data cells corresponding to the cut-down metadata
        /// </summary>
        /// <param name="dataMatrix"></param>
        /// <param name="queryMatrix"></param>
        /// <returns></returns>
        /// 

        /*WARNING - Work in progress
         Currently only reliably queries SPC sorted matrixes 
         Queries limited  Period queries
         */
        internal Matrix Query(Matrix dataMatrix, Matrix queryMatrix)
        {
            if (queryMatrix.MainSpec.GetDataSize() == dataMatrix.Cells.Count) return dataMatrix;

            List<MarkedCell> selectedCells = new List<MarkedCell>();
            foreach (dynamic cell in dataMatrix.Cells)
            {
                selectedCells.Add(new MarkedCell() { Cell = cell, Marked = false });
            };

            foreach (var val in dataMatrix.MainSpec.MainValues)
            {
                if (IsStatistic(val.Key, dataMatrix))
                    selectedCells = ExtractForStatistic(dataMatrix, queryMatrix, selectedCells);
                else if (IsFrequency(val.Key, dataMatrix))
                    selectedCells = ExtractForFrequency(dataMatrix, queryMatrix, selectedCells);
                else if (IsClassification(val.Key, dataMatrix))
                {
                    ClassificationRecordDTO_Create cls = dataMatrix.MainSpec.Classification.Where(x => x.Value == val.Key).FirstOrDefault();
                    selectedCells = ExtractForClassification(dataMatrix, queryMatrix, selectedCells, cls);
                }
            }

            //Set metadata:

            dataMatrix.MainSpec.Classification = dataMatrix.MainSpec.Classification.Intersect(queryMatrix.MainSpec.Classification).ToList();
            dataMatrix.MainSpec.Statistic = dataMatrix.MainSpec.Statistic.Intersect(queryMatrix.MainSpec.Statistic).ToList();
            dataMatrix.MainSpec.Frequency.Period = dataMatrix.MainSpec.Frequency.Period.Intersect(queryMatrix.MainSpec.Frequency.Period).ToList();




            if (dataMatrix.OtherLanguageSpec != null)
            {

                foreach (var spec in dataMatrix.OtherLanguageSpec)
                {
                    List<StatisticalRecordDTO_Create> sList = new List<StatisticalRecordDTO_Create>();
                    foreach (var s in dataMatrix.MainSpec.Statistic)
                    {
                        sList.AddRange(spec.Statistic.Where(x => x.Code == s.Code));
                    }
                    spec.Statistic = sList;

                    List<PeriodRecordDTO_Create> pList = new List<PeriodRecordDTO_Create>();
                    foreach (var p in dataMatrix.MainSpec.Frequency.Period)
                    {
                        pList.AddRange(spec.Frequency.Period.Where(x => x.Code == p.Code));
                    }
                    spec.Frequency.Period = pList;
                    int counter = 0;
                    foreach (var cls in spec.Classification)
                    {
                        List<VariableRecordDTO_Create> vList = new List<VariableRecordDTO_Create>();
                        foreach (var v in dataMatrix.MainSpec.Classification[counter].Variable)
                        {
                            vList.AddRange(cls.Variable.Where(x => x.Code == v.Code));
                        }
                        spec.Classification[counter].Variable = vList;
                        counter++;
                    }
                }
            }

            dataMatrix.Cells = selectedCells.Where(y => y.Marked).Select(x => x.Cell).ToList();
            return dataMatrix;
        }



        private bool IsStatistic(string dimName, Matrix matrix)
        {
            return matrix.MainSpec.ContentVariable == dimName;
        }

        private bool IsFrequency(string dimName, Matrix matrix)
        {
            return matrix.MainSpec.Frequency.Value == dimName;
        }

        private bool IsClassification(string dimName, Matrix matrix)
        {
            return matrix.MainSpec.Classification.Where(x => x.Value == dimName).Count() > 0;
        }


        private List<MarkedCell> ExtractForStatistic(Matrix dataMatrix, Matrix queryMatrix, List<MarkedCell> selectedCells)
        {

            //if (dataMatrix.MainSpec.Statistic.Count > queryMatrix.MainSpec.Statistic.Count)
            //{
            //    int counter = 0;

            //    int sliceCount = selectedCells.Count / dataMatrix.MainSpec.Statistic.Count;
            //    foreach (var s in dataMatrix.MainSpec.Statistic)
            //    {

            //        if (!queryMatrix.MainSpec.Statistic.Contains(s))
            //        {
            //            selectedCells.RemoveRange(counter * sliceCount, sliceCount);
            //            counter--;
            //        }

            //        counter++;
            //    }

            //}
            return selectedCells;
        }

        private List<MarkedCell> ExtractForFrequency(Matrix dataMatrix, Matrix queryMatrix, List<MarkedCell> selectedCells)
        {
            if (dataMatrix.MainSpec.Frequency.Period.Count > queryMatrix.MainSpec.Frequency.Period.Count)
            {
                int slice = (selectedCells.Count / dataMatrix.MainSpec.Frequency.Period.Count) / dataMatrix.MainSpec.Statistic.Count;
                int pcounter = 0;
                foreach (var p in dataMatrix.MainSpec.Frequency.Period)
                {
                    int statcounter = 0;
                    foreach (var s in dataMatrix.MainSpec.Statistic)
                    {
                        if (queryMatrix.MainSpec.Frequency.Period.Contains(p))
                        {
                            int statslice = selectedCells.Count / dataMatrix.MainSpec.Statistic.Count;
                            int periodslice = statslice / dataMatrix.MainSpec.Frequency.Period.Count;
                            int statstart = statslice * statcounter;
                            int periodstart = statstart + periodslice * pcounter;
                            int startpos = periodstart;
                            for (int i = startpos; i < startpos + slice; i++)
                            {
                                selectedCells.ElementAt(i).Marked = true;
                            }
                        }
                        statcounter++;
                    }
                    pcounter++;
                }
            }
            return selectedCells;
        }

        private List<MarkedCell> ExtractForClassification(Matrix dataMatrix, Matrix queryMatrix, List<MarkedCell> selectedCells, ClassificationRecordDTO_Create cls)
        {
            //int counter = 0;
            //ClassificationRecordDTO_Create qClass = queryMatrix.MainSpec.Classification.Where(x => x.Code == cls.Code).FirstOrDefault();

            //int sliceCount = selectedCells.Count / cls.Variable.Count;
            //if (cls.Variable.Count > qClass.Variable.Count)
            //{
            //    foreach (var v in cls.Variable)
            //    {
            //        if (!qClass.Variable.Contains(v))
            //        {
            //            selectedCells.RemoveRange(counter * sliceCount, sliceCount);
            //            counter--;
            //        }
            //        counter++;
            //    }
            //}

            return selectedCells;
        }



        /// <summary>
        /// class variable
        /// </summary>
        Matrix pxFileMatrix;
        Specification pxSpec;




        /// <summary>
        /// Get an ordered list of cells from a list of DataItem_DTO
        /// </summary>
        /// <param name="sortedData"></param>
        /// <returns></returns>
        internal IList<dynamic> GetNewTdtCells(List<DataItem_DTO> sortedData)
        {

            //Replace the existing cells with the sorted cell data
            List<dynamic> newCells = new List<dynamic>();
            foreach (var v in sortedData)
            {
                dynamic cl = new ExpandoObject();
                double outDouble;
                if (Double.TryParse(v.dataValue, out outDouble))
                {
                    PxDoubleValue newpx = new PxDoubleValue(Convert.ToDouble(v.dataValue));
                    cl.TdtValue = newpx;
                    cl.Value = newpx;
                    newCells.Add(cl);
                }
                else
                {
                    // PxStringValue newpx = new PxStringValue(v.dataValue);
                    cl.TdtValue = v.dataValue;//.ToPxValue();
                    cl.Value = v.dataValue;
                    newCells.Add(cl);
                }

            }
            return newCells;
        }

        /// <summary>
        /// Get details of a dimension based from a spec based on its name
        /// </summary>
        /// <param name="dimName"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        private DimensionValue_DTO GetDimensionData(string dimName, Specification spec)
        {
            DimensionValue_DTO dimData = new DimensionValue_DTO();

            //Is the Statistic implied rather than defined in VALUES as a dimension?
            //If so, then create the dimension here as it won't be picked up from VALUES
            if (spec.ContentVariable.Equals(dimName))
            {
                foreach (StatisticalRecordDTO_Create stat in spec.Statistic)
                {
                    DimensionDetail_DTO detail = new DimensionDetail_DTO();
                    detail.key = stat.Code;
                    detail.value = stat.Value;
                    detail.dimensionValue = dimData;
                    dimData.details.Add(detail);
                }
                dimData.dimType = DimensionType.STATISTIC;
                dimData.code = spec.ContentVariable;
                dimData.value = spec.ContentVariable;
                return dimData;
            }

            if (spec.Frequency.Value.Equals(dimName))
            {
                foreach (var period in spec.Frequency.Period)
                {
                    DimensionDetail_DTO detail = new DimensionDetail_DTO();
                    detail.key = period.Code;
                    detail.value = period.Value;
                    detail.dimensionValue = dimData;
                    dimData.details.Add(detail);
                }
                dimData.dimType = DimensionType.PERIOD;
                dimData.code = spec.Frequency.Code;
                dimData.value = spec.Frequency.Value;
                return dimData;
            }

            var cls = spec.Classification.Where(x => x.Value == dimName).FirstOrDefault();

            if (cls != null)
            {
                foreach (var vrb in cls.Variable)
                {
                    DimensionDetail_DTO detail = new DimensionDetail_DTO();
                    detail.key = vrb.Code;
                    detail.value = vrb.Value;
                    detail.dimensionValue = dimData;
                    dimData.details.Add(detail);
                }
                dimData.dimType = DimensionType.CLASSIFICATION;
                dimData.code = cls.Code;
                dimData.value = cls.Value;
                return dimData;
            }

            return dimData;
        }

        /// <summary>
        /// Get a list of Data items for a matrix
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="lngIsoCode"></param>
        /// <param name="theSpec"></param>
        /// <param name="defaultValues"></param>
        /// <returns></returns>

        internal List<DataItem_DTO> GetMatrixDataItems(Matrix theMatrix, string lngIsoCode, Specification theSpec = null, bool defaultValues = false, bool databaseDerived = false)
        {


            if (theSpec == null)

            {
                if (lngIsoCode != null)
                {
                    pxSpec = theMatrix.GetSpecFromLanguage(lngIsoCode);
                    if (pxSpec == null) pxSpec = theMatrix.MainSpec;
                    pxSpec.Frequency = theMatrix.MainSpec.Frequency;
                }
                else
                    pxSpec = theMatrix.MainSpec;
            }
            else pxSpec = theSpec;

            List<DimensionValue_DTO> valueDimensions = new List<DimensionValue_DTO>();
            List<DimensionValue_DTO> nativeDimensions = new List<DimensionValue_DTO>();
            IEnumerable<object[]> graph;

            //
            if (!databaseDerived)
            {
                int count = 0;
                foreach (var val in theSpec.Values)
                {
                    foreach (var pair in val.Value)
                    {
                        count++;
                    }
                }
                if (pxSpec.Values.Where(x => x.Key == theSpec.ContentVariable).Count() == 0)
                {
                    DimensionValue_DTO dimData = new DimensionValue_DTO();
                    foreach (StatisticalRecordDTO_Create stat in theSpec.Statistic)
                    {
                        DimensionDetail_DTO detail = new DimensionDetail_DTO();
                        detail.key = stat.Code;
                        detail.value = stat.Value;
                        detail.dimensionValue = dimData;
                        dimData.details.Add(detail);
                    }
                    dimData.dimType = DimensionType.STATISTIC;
                    dimData.code = theSpec.ContentVariable;
                    dimData.value = theSpec.ContentVariable;
                    valueDimensions.Add(dimData);
                }
                //This data came from a px file and we read the data by order of VALUES
                foreach (var val in pxSpec.Values)
                {
                    //val.Key will contain the name of the dimension - look it up from the statistics, frequency, classifications, etc
                    DimensionValue_DTO sVal = new DimensionValue_DTO();

                    valueDimensions.Add(GetDimensionData(val.Key, pxSpec));


                }

                graph = CartesianProduct(valueDimensions[0].details.ToArray());

                for (int i = 1; i < valueDimensions.Count; i++)
                {
                    graph = CartesianProduct(graph.ToArray(), valueDimensions[i].details.ToArray());
                }

            }

            else
            {
                //This data came from a database read and is sorted SPC. Therefore we read in the order of the matrix dimensions
                DimensionValue_DTO sv = new DimensionValue_DTO();
                sv.dimType = DimensionType.STATISTIC;
                foreach (StatisticalRecordDTO_Create stat in pxSpec.Statistic)
                {
                    DimensionDetail_DTO detail = new DimensionDetail_DTO();
                    detail.key = stat.Code;
                    detail.value = stat.Value;
                    detail.dimensionValue = sv;
                    sv.details.Add(detail);

                }
                nativeDimensions.Add(sv);


                DimensionValue_DTO pVal = new DimensionValue_DTO();
                pVal.dimType = DimensionType.PERIOD;
                foreach (PeriodRecordDTO_Create per in pxSpec.Frequency.Period)
                {
                    DimensionDetail_DTO detail = new DimensionDetail_DTO();
                    detail.key = per.Code;
                    detail.value = per.Value;
                    detail.dimensionValue = pVal;
                    pVal.details.Add(detail);
                }
                nativeDimensions.Add(pVal);

                foreach (ClassificationRecordDTO_Create cls in pxSpec.Classification)
                {
                    DimensionValue_DTO cVal = new DimensionValue_DTO();
                    cVal.dimType = DimensionType.CLASSIFICATION;
                    cVal.code = cls.Code;
                    cVal.value = cls.Value;

                    foreach (var vrb in cls.Variable)
                    {
                        DimensionDetail_DTO detail = new DimensionDetail_DTO();
                        detail.key = vrb.Code;
                        detail.value = vrb.Value;
                        detail.dimensionValue = cVal;
                        cVal.details.Add(detail);
                    }
                    nativeDimensions.Add(cVal);

                }

                graph = CartesianProduct(nativeDimensions[0].details.ToArray());

                for (int i = 1; i < nativeDimensions.Count; i++)
                {
                    graph = CartesianProduct(graph.ToArray(), nativeDimensions[i].details.ToArray());
                }
            }


            List<DataItem_DTO> itemList = new List<DataItem_DTO>();

            int counter = 0;
            long c = graph.Count();
            string nullValue = Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE");
            foreach (var item in graph)
            {

                DataItem_DTO dto = new DataItem_DTO();
                populateDataItem(ref dto, item, true);


                if (defaultValues) dto.dataValue = nullValue;
                else
                    if (databaseDerived)
                    dto.dataValue = counter < theMatrix.Cells.Count ? theMatrix.Cells[counter].TdtValue.ToString() : new PxStringValue(nullValue);
                else
                    dto.dataValue = counter < theMatrix.Cells.Count ? theMatrix.Cells[counter].Value.ToString() : new PxStringValue(nullValue);
                itemList.Add(dto);
                counter++;
            }
            return itemList;
        }

        //Only use this if the data in the matrix is already sorted SPC
        // This could form the basis of a Matrix search function
        private List<dynamic> GetCellsForPeriods(Matrix theMatrix, Specification theSpec, List<PeriodRecordDTO_Create> Periods, bool newData = false)
        {
            List<dynamic> dataList = new List<dynamic>();
            int blockSize = 1;


            Dictionary<string, int> periodOffsets = new Dictionary<string, int>();
            foreach (var cls in theSpec.Classification)
            {
                blockSize = blockSize * cls.Variable.Count;
            }
            int statBlock = blockSize * theMatrix.MainSpec.Frequency.Period.Count;
            if (newData)
            {
                for (int i = 0; i < statBlock; i++)
                {
                    dynamic cell = new ExpandoObject();
                    cell.Value = Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE");
                    dataList.Add(cell);
                }
                return dataList;
            }

            int statCounter = 1;
            foreach (var stat in theSpec.Statistic)
            {
                foreach (var period in Periods)
                {
                    periodOffsets.Add(period.Code, theMatrix.MainSpec.Frequency.Period.FindIndex(x => x.Code == period.Code));
                    for (int i = statCounter * periodOffsets[period.Code]; i < blockSize; i++)
                    {
                        dynamic cell = new ExpandoObject();
                        cell.Value = theMatrix.Cells[i];
                        dataList.Add(cell);
                    }
                }
                statCounter++;
            }

            return dataList;
        }


        /// <summary>
        /// Populates an individual DataItem_DTO from a DimensionValue_DTO object
        /// This function calls itself recursively as it navigates through the DimensionValue_DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="item"></param>
        private void populateDataItem(ref DataItem_DTO dto, object[] item, bool getIds = false)
        {
            foreach (var subItem in item)
            {
                if (subItem.GetType().IsArray)
                {
                    populateDataItem(ref dto, (object[])subItem, getIds);
                }
                else
                {
                    var v = (DimensionDetail_DTO)subItem;
                    switch (v.dimensionValue.dimType)
                    {
                        case DimensionType.STATISTIC:
                            dto.statistic.Code = v.key;
                            dto.statistic.Value = v.value;
                            StatisticalRecordDTO_Create newStat = pxSpec.Statistic.Where(x => x.Code == v.key).FirstOrDefault();
                            dto.statistic.Decimal = newStat.Decimal;
                            dto.statistic.Unit = newStat.Unit;
                            dto.statistic.StatisticalProductId = newStat.StatisticalProductId;
                            dto.statistic.SortId = newStat.SortId;
                            dto.statistic.Id = newStat.Id;
                            break;
                        case DimensionType.PERIOD:
                            dto.period.Code = v.key;
                            dto.period.Value = v.value;
                            if (getIds)
                            {
                                PeriodRecordDTO_Create newPer = pxSpec.Frequency.Period.Where(x => x.Code == v.key).FirstOrDefault();
                                dto.period.FrequencyPeriodId = newPer.FrequencyPeriodId;
                                dto.period.SortId = newPer.SortId;
                                dto.period.Id = newPer.Id;
                            }
                            break;
                        case DimensionType.CLASSIFICATION:
                            var cls = new ClassificationRecordDTO_Create();
                            var vrb = new VariableRecordDTO_Create();
                            vrb.Code = v.key;
                            vrb.Value = v.value;

                            ClassificationRecordDTO_Create newCls = pxSpec.Classification.Where(x => x.Code == v.dimensionValue.code).FirstOrDefault();
                            cls.Code = newCls.Code;
                            cls.GeoFlag = newCls.GeoFlag;
                            cls.GeoUrl = newCls.GeoUrl;
                            if (getIds)
                            {
                                VariableRecordDTO_Create newVar = newCls.Variable.Where(x => x.Code == v.key).FirstOrDefault();
                                vrb.ClassificationVariableId = newVar.ClassificationVariableId;
                                vrb.SortId = newVar.SortId;
                                vrb.Id = newVar.Id;
                            }
                            cls.ClassificationId = newCls.ClassificationId;
                            cls.Value = newCls.Value;
                            cls.Variable = new List<VariableRecordDTO_Create>();
                            cls.Variable.Add(vrb);
                            cls.IdMultiplier = newCls.IdMultiplier;
                            dto.classifications.Add(cls);
                            break;

                    }


                }



            }
        }


        /// <summary>
        /// Create data tables based on the Matrix data and metadata. Load the data to the database TD_DATA and TM_DATA_CELL tables
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="getIds"></param>
        /// <returns></returns>
        internal async Task CreateAndLoadDataTables(ADO ado, Matrix theMatrixData, bool getIds = false)
        {

            List<Task> listOfTasks = new List<Task>();

            List<dynamic> dimensions = new List<dynamic>();

            pxFileMatrix = theMatrixData;

            //Get a list of dimensions from the VALUES of the px file (via MainSpec.MainValues)
            // A dimension is any Statistic, classification or period
            List<DimensionValue_DTO> dtoList = new List<DimensionValue_DTO>();


            foreach (var pair in theMatrixData.MainSpec.MainValues)
            {
                dtoList = getDimensionCodes(theMatrixData, dtoList, pair);
            }



            //If there is no specified STATISTIC in the VALUES then the statistic must be inferred from the CONTENTS
            if ((dtoList.Where(f => f.dimType == DimensionType.STATISTIC)).Count() == 0)
            {
                DimensionValue_DTO statDim = new DimensionValue_DTO();
                statDim.code = "0";
                statDim.value = theMatrixData.MainSpec.Contents;
                statDim.dimType = DimensionType.STATISTIC;

                foreach (var stat in theMatrixData.MainSpec.Statistic)
                {
                    DimensionDetail_DTO dtoDetail = new DimensionDetail_DTO();
                    dtoDetail.key = stat.Code;
                    dtoDetail.value = stat.Value;
                    dtoDetail.dimensionValue = statDim;
                    statDim.details.Add(dtoDetail);

                }
                dtoList.Add(statDim);
            }

            //Get a notional list of all of the data based on the structure as it appears in the order of px values
            var graph = CartesianProduct(dtoList[0].details.ToArray());

            for (int i = 1; i < dtoList.Count; i++)
            {
                graph = CartesianProduct(graph.ToArray(), dtoList[i].details.ToArray());
            }

            //Now iterate through the graph and create the two tables for upload
            DataTable tdData = getTdTable();
            DataTable cellData = getCellTable();
            Matrix_ADO matrixAdo = new Matrix_ADO(ado);

            //To keep memory usage low, we can create and load the data in tranches
            //Set this high if you have lots of server memory and want relatively faster performance
            //Set this low if you want to conserve memory and you don't mind taking a hit on the time to upload          
            long trancheSize = Configuration_BSO.GetCustomConfig(ConfigType.server, "bulkcopy-tranche-multiplier") * Convert.ToInt32(ConfigurationManager.AppSettings["API_ADO_BULKCOPY_BATCHSIZE"]);
            Log.Instance.Debug(graph.Count().ToString() + " rows of data to be uploaded.");
            int counter = 0;

            //We only load data for the specification corresponding to the default language.
            Specification dataSpec = theMatrixData.MainSpec;
            var cgraph = graph.Count();
            foreach (var item in graph)
            {
                //Simply a list of classification id /variable id pairs
                List<ClassificationVariable> classVars = new List<ClassificationVariable>();

                DataRow tdRow = tdData.NewRow();


                ClassificationVariable clsVar = new ClassificationVariable(0, 0);
                //Start off the recursive function and get one data row
                populateDataItemForDataTables(ref tdRow, ref classVars, item, getIds);

                tdRow["TDT_MTR_ID"] = dataSpec.MatrixId;
                tdRow["TDT_IX"] = counter;
                double outDouble;
                dynamic v;
                try
                {

                    v = theMatrixData.Cells[counter].Value.ToString();
                }
                catch (Exception ex)
                {
                    Log.Instance.Debug("Load Error. counter=" + counter);
                    Log.Instance.Debug("Load Error. Cell count=" + theMatrixData.Cells.Count);
                    Log.Instance.Debug("Load Error. Graph count=" + graph.Count());
                    Log.Instance.Debug("Load Error. Stat count=" + theMatrixData.MainSpec.Statistic.Count);
                    Log.Instance.Debug("Load Error. Period count=" + theMatrixData.MainSpec.Frequency.Period.Count);
                    Log.Instance.Debug("Load Error. dtoList count=" + dtoList.Count);
                    foreach (var dto in dtoList)
                    {
                        Log.Instance.Debug("Load Error - dtovalues: . " + dto.value + " count=" + dto.details.Count);
                    }
                    foreach (var cls in theMatrixData.MainSpec.Classification)
                    {
                        Log.Instance.Debug("Load Error. " + cls.Value + " count=" + cls.Variable.Count);
                    }
                    throw ex;
                }
                if (Double.TryParse(v, out outDouble))
                {
                    var s = (int)tdRow["decimals"];
                    tdRow["TDT_VALUE"] = DecimalPlaceDouble(outDouble, s);

                }
                else
                    tdRow["TDT_VALUE"] = theMatrixData.Cells[counter].Value.ToString();

                tdData.Rows.Add(tdRow);

                //We create a cell row for each item in the classVars struct but we relate it to main table
                foreach (ClassificationVariable cv in classVars)
                {
                    DataRow cellRow = cellData.NewRow();
                    cellRow["DTC_TDT_IX"] = counter;
                    cellRow["DTC_MTR_ID"] = dataSpec.MatrixId;
                    cellRow["DTC_VRB_ID"] = cv.vrbId;
                    cellData.Rows.Add(cellRow);
                }

                //If we've hit the max tranche size then we upload the data and create two brand new, empty tables
                if (tdData.Rows.Count % trancheSize == 0)
                {
                    //push the next tranche only when the last one is done
                    await Task.WhenAll(listOfTasks);

                    listOfTasks.Add(matrixAdo.CreateTdtDataAndCellRecords(tdData, cellData));

                    tdData = getTdTable();
                    cellData = getCellTable();
                    Log.Instance.Debug("Data tranche uploaded :" + tdData.Rows.Count + " rows");
                }

                counter++;
            }
            //upload any data that hasn't already been loaded
            if (tdData.Rows.Count > 0)
            {

                listOfTasks.Add(matrixAdo.CreateTdtDataAndCellRecords(tdData, cellData));

                Log.Instance.Debug("Data tranche uploaded :" + tdData.Rows.Count + " rows");
            }


            await Task.WhenAll(listOfTasks);

        }

        private string DecimalPlaceDouble(double d, int decimalPlaces)
        {
            if (decimalPlaces == 0) return d.ToString("0");
            string format = "0.";
            for (int i = 0; i < decimalPlaces; i++)
            {
                format = format + "0";
            }
            return d.ToString(format);
        }

        /// <summary>
        /// Populates an individual DataItem_DTO from a DimensionValue_DTO object
        /// This function calls itself recursively as it navigates through the DimensionValue_DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="item"></param>
        private void populateDataItemForDataTables(ref DataRow tdRow, ref List<ClassificationVariable> clsVars, object[] item, bool getIds = false)
        {
            foreach (var subItem in item)
            {
                //If we haven't create a full data entry yet
                if (subItem.GetType().IsArray)
                {
                    populateDataItemForDataTables(ref tdRow, ref clsVars, (object[])subItem, getIds);
                }
                else
                {
                    //we now have all the data we need for a single data item, so let's populate...
                    var v = (DimensionDetail_DTO)subItem;
                    switch (v.dimensionValue.dimType)
                    {
                        case DimensionType.STATISTIC:

                            tdRow["TDT_STT_ID"] = pxFileMatrix.MainSpec.Statistic.Where(x => x.Code == v.key).FirstOrDefault().StatisticalProductId;
                            tdRow["decimals"] = pxFileMatrix.MainSpec.Statistic.Where(x => x.Code == v.key).FirstOrDefault().Decimal;
                            break;
                        case DimensionType.PERIOD:

                            if (getIds)
                            {

                                tdRow["TDT_PRD_ID"] = pxFileMatrix.MainSpec.Frequency.Period.Where(x => x.Code == v.key).FirstOrDefault().FrequencyPeriodId;
                            }
                            break;
                        case DimensionType.CLASSIFICATION:
                            var cls = new ClassificationRecordDTO_Create();
                            var vrb = new VariableRecordDTO_Create();


                            ClassificationRecordDTO_Create newCls = pxFileMatrix.MainSpec.Classification.Where(x => x.Code == v.dimensionValue.code).FirstOrDefault();
                            ClassificationVariable clsvar = new ClassificationVariable();
                            clsvar.clsId = pxFileMatrix.MainSpec.Classification.Where(x => x.Code == v.dimensionValue.code).FirstOrDefault().ClassificationId;

                            if (getIds)
                            {
                                clsvar.vrbId = newCls.Variable.Where(x => x.Code == v.key).FirstOrDefault().ClassificationVariableId;
                            }
                            cls.ClassificationId = newCls.ClassificationId;

                            clsVars.Add(clsvar);

                            break;



                    }

                }

            }
        }

        /// <summary>
        /// Returns a blank TD_DATA table
        /// </summary>
        /// <returns></returns>
        private DataTable getTdTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("TDT_VALUE", typeof(string));
            dataTable.Columns.Add("TDT_STT_ID", typeof(int));
            dataTable.Columns.Add("TDT_IX", typeof(int));
            dataTable.Columns.Add("TDT_PRD_ID", typeof(int));
            dataTable.Columns.Add("TDT_MTR_ID", typeof(int));
            dataTable.Columns.Add("decimals", typeof(int));

            return dataTable;

        }

        /// <summary>
        /// Returns a blank TM_DATA_CELL table
        /// </summary>
        /// <returns></returns>
        private DataTable getCellTable()
        {
            DataTable dataCellTable = new DataTable();
            dataCellTable.Columns.Add("DTC_TDT_IX", typeof(string));
            dataCellTable.Columns.Add("DTC_MTR_ID", typeof(int));
            dataCellTable.Columns.Add("DTC_VRB_ID", typeof(int));

            return dataCellTable;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        internal static IEnumerable<object[]> CartesianProduct(params object[][] inputs)
        {
            return inputs.Aggregate(
                (IEnumerable<object[]>)new object[][] { new object[0] },
                (soFar, input) =>
                    from prevProductItem in soFar
                    from item in input
                    select prevProductItem.Concat(new object[] { item }).ToArray());
        }



        /// <summary>
        /// Associates the values with their relevant Classifications, Statistics and Periods
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="dtoList"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        private List<DimensionValue_DTO> getDimensionCodes(Matrix theMatrixData, List<DimensionValue_DTO> dtoList, KeyValuePair<string, IList<IPxSingleElement>> pair, Specification theSpec = null)
        {
            if (theSpec == null) theSpec = theMatrixData.MainSpec;

            var c = theSpec.Classification.Where(f => f.Value == pair.Key);
            if (c.Count() > 0)
            {
                DimensionValue_DTO dto = new DimensionValue_DTO();
                var cls = c.FirstOrDefault();
                dto.code = cls.Code;
                dto.value = cls.Value;
                dto.dimType = DimensionType.CLASSIFICATION;

                string values = "";

                foreach (var vrb in cls.Variable)
                {
                    foreach (var item in pair.Value)
                    {
                        if (item.SingleValue == vrb.Value)
                        {
                            DimensionDetail_DTO dtoDetail = new DimensionDetail_DTO();

                            dtoDetail.key = vrb.Code;
                            dtoDetail.value = vrb.Value;
                            dtoDetail.dimensionValue = dto;
                            dto.details.Add(dtoDetail);
                            values = values + vrb.Value + '~';
                        }
                    }
                }
                dtoList.Add(dto);
            }
            //Statistic may be rolled up as ContentVariable (if more than one exist) or else be just the value in Contents
            if (pair.Key == theSpec.ContentVariable || pair.Key == theSpec.Contents)
            {
                DimensionValue_DTO dto = new DimensionValue_DTO();
                dto.code = "";
                dto.value = pair.Key;
                dto.dimType = DimensionType.STATISTIC;
                foreach (var stat in theSpec.Statistic)
                {
                    DimensionDetail_DTO dtoDetail = new DimensionDetail_DTO();
                    dtoDetail.key = stat.Code;
                    dtoDetail.value = stat.Value;
                    dtoDetail.dimensionValue = dto;
                    dto.details.Add(dtoDetail);
                }
                dtoList.Add(dto);
            }

            if (pair.Key == theSpec.Frequency.Value)
            {
                DimensionValue_DTO dto = new DimensionValue_DTO();
                dto.code = theSpec.Frequency.Code;
                dto.value = theSpec.Frequency.Code;
                dto.dimType = DimensionType.PERIOD;
                foreach (var period in theSpec.Frequency.Period)
                {
                    DimensionDetail_DTO dtoDetail = new DimensionDetail_DTO();
                    dtoDetail.key = period.Code;
                    dtoDetail.value = period.Value;
                    dtoDetail.dimensionValue = dto;
                    dto.details.Add(dtoDetail);
                }
                dtoList.Add(dto);
            }
            return dtoList;
        }







        /// <summary>
        /// Get a csv representation of a Matrix
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <returns></returns>
        internal string GetCsvTemplate(Matrix theMatrixData, string lngIsoCode = null, string overrideFrqCode = null)
        {

            List<string> headerString = new List<string>();

            List<string> clsLines = new List<string>();
            if (headerString == null) headerString = new List<string>();

            ClassificationRecordDTO_Create cDto = theMatrixData.MainSpec.Classification.FirstOrDefault();


            if (lngIsoCode == null) lngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

            Matrix.Specification theSpec = theMatrixData.GetSpecFromLanguage(lngIsoCode);

            if (overrideFrqCode != null)
            {
                theSpec.Frequency = new Data.FrequencyRecordDTO_Create();
                theSpec.Frequency.Code = overrideFrqCode;
                theSpec.Frequency.Value = "";
            }

            if (theSpec == null)
                throw new FormatException(Label.Get("px.build.language"));


            headerString.Add(new PxQuotedValue(Utility.GetCustomConfig("APP_CSV_STATISTIC")).ToPxString());

            headerString.Add(new PxQuotedValue(theSpec.Frequency.Code).ToPxString());


            foreach (var cls in theSpec.Classification)
            {
                headerString.Add(new PxQuotedValue(cls.Code).ToPxString());
            }

            headerString.Add(new PxQuotedValue(Utility.GetCustomConfig("APP_CSV_VALUE")).ToPxString());
            return (string.Join(",", headerString.ToArray()));
        }

        internal List<DataItem_DTO> GetDataForNewPeriods(Matrix theMatrixData, BuildUpdate_DTO DTO, ADO Ado)
        {
            List<DataItem_DTO> requestItems = new List<DataItem_DTO>();
            Specification theSpec = theMatrixData.GetSpecFromLanguage(DTO.LngIsoCode);



            Matrix matrixNewMetadata = new Matrix(DTO);
            if (DTO.Dimension.Count > 0)
            {
                matrixNewMetadata.OtherLanguageSpec = new List<Matrix.Specification>();
                foreach (var dimension in DTO.Dimension)
                {
                    matrixNewMetadata.OtherLanguageSpec.Add(new Matrix.Specification(dimension.LngIsoCode, DTO));

                }

            }


            if (DTO.MtrCode != null)
                theMatrixData.Code = DTO.MtrCode;


            if (DTO.CprCode != null)
            {
                //Look this up from 
                Copyright_ADO cAdo = new Copyright_ADO();
                Copyright_DTO_Read cDto = new Copyright_DTO_Read();
                cDto.CprCode = DTO.CprCode;
                var result = cAdo.Read(Ado, cDto);
                if (result.hasData)
                {
                    theSpec.Source = result.data[0].CprValue;
                }
                else
                {

                    return null;
                }
            }

            theMatrixData.IsOfficialStatistic = DTO.MtrOfficialFlag;

            if (theSpec == null)
                throw new FormatException(Label.Get("px.build.language"));


            List<PeriodRecordDTO_Create> Periods = new List<PeriodRecordDTO_Create>();






            Periods.AddRange(DTO.Periods);


            //Periods may have been added or removed. Furthermore, the same periods may have different values for different languages


            var dim = DTO.Dimension.Where(x => x.LngIsoCode == theSpec.Language).FirstOrDefault();


            theSpec.Frequency.Period = Periods;


            theMatrixData.Cells = GetCellsForPeriods(theMatrixData, theSpec, DTO.Periods, true);

            //Get the new data and metadata from the csv input in the DTO
            //We will ignore any rows that have no corresponding dimension information in the main input file or in the added periods

            requestItems = GetMatrixDataItems(theMatrixData, DTO.LngIsoCode, theSpec);

            SetMetadataSortIds(ref theSpec);

            var filteredRequestItems = requestItems.Where(x => Periods.Contains(x.period)).ToList();




            SetDataSortIds(ref filteredRequestItems, theSpec);




            return filteredRequestItems.OrderBy(x => x.sortID).ToList(); ;

        }



        internal List<DataItem_DTO> GetDataForAllPeriods(Matrix theMatrixData, BuildUpdate_DTO DTO, ADO Ado)
        {

            List<DataItem_DTO> requestItems = new List<DataItem_DTO>();
            Specification theSpec = theMatrixData.GetSpecFromLanguage(DTO.LngIsoCode);


            if (DTO.MtrCode != null)
                theMatrixData.Code = DTO.MtrCode;

            List<PeriodRecordDTO_Create> Periods = new List<PeriodRecordDTO_Create>();

            // theSpec.Frequency.Period.AddRange(DTO.Dimension[0].Frequency.Period);


            //First we get the list of the existing data
            SetMetadataSortIds(ref theSpec);

            List<DataItem_DTO> existingItems = GetMatrixDataItems(theMatrixData, DTO.LngIsoCode, theSpec);

            SetDataSortIds(ref existingItems, theSpec);



            //Periods may have been added or removed. Furthermore, the same periods may have different values for different languages


            var dim = DTO.Dimension.Where(x => x.LngIsoCode == theSpec.Language).FirstOrDefault();

            //Periods.AddRange(DTO.Dimension[0].Frequency.Period);

            if (dim.Frequency != null)
            {
                //var commons = TestList1.Select(s1 => s1.SomeProperty).ToList().Intersect(TestList2.Select(s2 => s2.SomeProperty).ToList()).ToList();
                Periods.AddRange(DTO.Dimension.Where(x => x.LngIsoCode == DTO.LngIsoCode).FirstOrDefault().Frequency.Period.Except(theSpec.Frequency.Period));
            }
            theSpec.Frequency.Period = Periods;

            SetMetadataSortIds(ref theSpec);

            requestItems = GetMatrixDataItems(theMatrixData, DTO.LngIsoCode, theSpec);
            foreach (var r in requestItems) r.dataValue = Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE");

            SetDataSortIds(ref requestItems, theSpec);

            requestItems.AddRange(existingItems);

            return requestItems.OrderBy(x => x.sortID).ToList();



        }



        //Dictionary<string, List<DataItem_DTO>> allDto = new Dictionary<string, List<DataItem_DTO>>();

        internal struct allDTO
        {
            internal List<DataItem_DTO> existing { get; set; }
            internal List<DataItem_DTO> request { get; set; }
            internal List<DataItem_DTO> defaultItems { get; set; }
        }


        private void createExistingItems(Matrix theMatrixData, string LngIsoCode, Specification theSpec)
        {
            List<DataItem_DTO> existingItems = GetMatrixDataItems(theMatrixData, LngIsoCode, theSpec);

            SetDataSortIds(ref existingItems, theSpec);


            AllMergeLists.existing = existingItems;

        }

        private void createRequestItems(List<PeriodRecordDTO_Create> Periods, Specification theSpec, BuildUpdate_DTO DTO)
        {
            List<DataItem_DTO> requestItems = new List<DataItem_DTO>();
            //Get the new data and metadata from the csv input in the DTO
            //We will ignore any rows that have no corresponding dimension information in the main input file or in the added periods
            //SortIds are set in this function and also applied retrospectively to the DTO in order to flag duplicates in the next step
            if (DTO.PxData != null)
                requestItems = GetInputObjectsJson(Periods, theSpec, DTO);

            //Find and remove dupes 

            List<DataItem_DTO> dupes = requestItems.GroupBy(x => x).SelectMany(g => g.Skip(1)).ToList();

            if (dupes.Any())
            {

                HashSet<long> dupeIds = new HashSet<long>(dupes.Select(x => x.sortID));
                requestItems.RemoveAll(x => dupes.Contains(x));

                //Also we must flag the DTO items for the report to indicate that these values were not updated
                var dtoDupes = (DTO.PxData.DataItems.Where(x => dupeIds.Contains((long)x.SortId)));
                dtoDupes.ToList().ForEach(c => c.updated = false);
                dtoDupes.ToList().ForEach(c => c.duplicate = true);

                foreach (var v in dtoDupes)
                {
                    Log.Instance.Debug("**Build Validation** Dupe: " + Utility.JsonSerialize_IgnoreLoopingReference(v));
                }
                Log.Instance.Debug("**Build Validation** Count of Dupes found : " + dtoDupes.Count().ToString());
            }
            else
                requestItems.ToList().ForEach(c => c.duplicate = false);

            AllMergeLists.request = requestItems;

        }

        private void createDefaultItems(Matrix defaultMatrix, string LngIsoCode, Specification theSpec)
        {

            List<DataItem_DTO> defaultItems = GetMatrixDataItems(defaultMatrix, LngIsoCode, theSpec, true);

            SetDataSortIds(ref defaultItems, theSpec);

            AllMergeLists.defaultItems = defaultItems;
        }


        internal void UpdateMatrixMetadata(ref Matrix theMatrixData, BuildUpdate_DTO DTO)
        {
            theMatrixData.Code = DTO.MtrCode != null ? DTO.MtrCode : theMatrixData.Code;
            theMatrixData.Copyright.CprCode = String.IsNullOrEmpty(DTO.CprCode) ? theMatrixData.Copyright.CprCode : DTO.CprCode;
            theMatrixData.IsOfficialStatistic = DTO.MtrOfficialFlag;


            foreach (Dimension_DTO dim in DTO.Dimension)
            {

                Specification spec = theMatrixData.GetSpecFromLanguage(dim.LngIsoCode);
                PopulateSpec(dim, ref spec, DTO);

            }


        }

        private void PopulateSpec(Dimension_DTO dim, ref Specification spec, BuildUpdate_DTO DTO)
        {
            spec.Frequency.Code = String.IsNullOrEmpty(DTO.FrqCodeTimeval) ? spec.Frequency.Code : DTO.FrqCodeTimeval;
            spec.Frequency.Value = String.IsNullOrEmpty(dim.Frequency.Value) ? spec.Frequency.Value : dim.Frequency.Value;

            if (dim.Classifications != null)
            {
                foreach (var cls in spec.Classification)
                {
                    var dtoCls = dim.Classifications.Where(x => x.Code == cls.Code).FirstOrDefault();
                    cls.GeoUrl = dtoCls.GeoUrl;
                    cls.GeoFlag = dtoCls.GeoFlag;
                }
            }

            spec.Title = String.IsNullOrEmpty(dim.MtrTitle) ? spec.Title : dim.MtrTitle;
            spec.ContentVariable = String.IsNullOrEmpty(dim.StatisticLabel) ? spec.ContentVariable : dim.StatisticLabel;
            spec.Notes = new List<string>() { dim.MtrNote };
            Copyright_BSO cBso = new Copyright_BSO();
            if (!String.IsNullOrEmpty(DTO.CprCode))
            {
                spec.Source = cBso.Read(DTO.CprCode).CprValue;
            }

            spec.Contents = spec.Title;
        }



        /// <summary>
        /// Updates a matrix based on the extra items supplied in the BuildUpdate_DTO
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="DTO"></param>
        /// <param name="Ado"></param>
        /// <returns></returns>
        internal Matrix UpdateMatrixFromBuild(Matrix theMatrixData, BuildUpdate_DTO DTO, ADO Ado)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();



            Specification theSpec = new Matrix.Specification();

            theSpec = theSpec.Clone(theMatrixData.GetSpecFromLanguage(DTO.LngIsoCode));

            SetMetadataSortIds(ref theSpec, DTO.Periods);

            List<PeriodRecordDTO_Create> existingPeriods = new List<PeriodRecordDTO_Create>();
            foreach (var period in theSpec.Frequency.Period) existingPeriods.Add(period);

            Matrix matrixNewMetadata = new Matrix(DTO);
            if (DTO.Dimension.Count > 0)
            {
                matrixNewMetadata.OtherLanguageSpec = new List<Matrix.Specification>();
                foreach (var dimension in DTO.Dimension)
                {
                    matrixNewMetadata.OtherLanguageSpec.Add(new Matrix.Specification(dimension.LngIsoCode, DTO));

                }

            }


            if (DTO.MtrCode != null)
                theMatrixData.Code = DTO.MtrCode;


            if (DTO.CprCode != null)
            {
                //Look this up from 
                Copyright_ADO cAdo = new Copyright_ADO();
                Copyright_DTO_Read cDto = new Copyright_DTO_Read();
                cDto.CprCode = DTO.CprCode;
                var result = cAdo.Read(Ado, cDto);
                if (result.hasData)
                {
                    theSpec.Source = result.data[0].CprValue;
                }
                else
                {

                    return null;
                }
            }

            theMatrixData.IsOfficialStatistic = DTO.MtrOfficialFlag;

            if (theSpec == null)
                throw new FormatException(Label.Get("px.build.language"));


            List<PeriodRecordDTO_Create> Periods = new List<PeriodRecordDTO_Create>();


            Log.Instance.Debug("*Diagnostic* Matrix created: " + sw.ElapsedMilliseconds); //141942




            Periods.AddRange(theSpec.Frequency.Period);



            //Periods may have been added or removed. Furthermore, the same periods may have different values for different languages


            var dim = DTO.Dimension.Where(x => x.LngIsoCode == theSpec.Language).FirstOrDefault();



            if (dim.Frequency != null)
            {
                Periods.AddRange(DTO.Dimension.Where(x => x.LngIsoCode == DTO.LngIsoCode).FirstOrDefault().Frequency.Period.Except(theSpec.Frequency.Period));
            }

            int counter = 1;
            foreach (var period in Periods)
            {
                period.SortId = counter;
                counter++;
            }



            Matrix defaultMatrix = new Matrix();
            defaultMatrix.MainSpec = theSpec.Clone();

            // defaultMatrix.MainSpec.Frequency.Period = Periods;
            defaultMatrix.MainSpec.Frequency.Period = new List<PeriodRecordDTO_Create>();
            foreach (var per in Periods) defaultMatrix.MainSpec.Frequency.Period.Add(new PeriodRecordDTO_Create() { Code = per.Code, FrequencyPeriodId = per.FrequencyPeriodId, SortId = per.SortId, Value = per.Value });

            if (theMatrixData.OtherLanguageSpec != null)
            {
                defaultMatrix.OtherLanguageSpec = new List<Specification>();
                foreach (var spec in theMatrixData.OtherLanguageSpec)
                {
                    spec.Frequency.Period = Periods;
                    defaultMatrix.OtherLanguageSpec.Add(spec);

                }
            }



            Log.Instance.Debug("*Diagnostic* Preparing all dto item lists " + sw.ElapsedMilliseconds);

            Specification existingSpec = theSpec.Clone();
            Specification requestSpec = theSpec.Clone();




            createRequestItems(Periods, requestSpec, DTO);
            createExistingItems(theMatrixData, DTO.LngIsoCode, existingSpec);
            createDefaultItems(defaultMatrix, DTO.LngIsoCode, defaultMatrix.MainSpec);
            //if (DTO.PxData == null)
            //{
            //    //There is no new update data, just deal with the existing and default items
            //    Parallel.Invoke(() => createExistingItems(theMatrixData, DTO.LngIsoCode, existingSpec),
            //        () => createDefaultItems(defaultMatrix, DTO.LngIsoCode, defaultMatrix.MainSpec));
            //    AllMergeLists.request = new List<DataItem_DTO>();
            //}
            //else
            //    Parallel.Invoke(() => createRequestItems(Periods, requestSpec, DTO), () => createExistingItems(theMatrixData, DTO.LngIsoCode, existingSpec),
            //        () => createDefaultItems(defaultMatrix, DTO.LngIsoCode, defaultMatrix.MainSpec));

            Log.Instance.Debug("*Diagnostic* All dto item lists complete " + sw.ElapsedMilliseconds);


            List<DataItem_DTO> merged = MergeAndSortData(AllMergeLists.existing, AllMergeLists.request, AllMergeLists.defaultItems);


            Log.Instance.Debug("*Diagnostic* Merge and sort: " + sw.ElapsedMilliseconds);

            //Set the Cells to the merged and sorted data
            Build_BSO pBso = new Build_BSO();
            theMatrixData.Cells = pBso.GetNewTdtCells(merged);
            theMatrixData.MainSpec.Frequency.Period = defaultMatrix.MainSpec.Frequency.Period;
            UpdateMatrixMetadata(ref theMatrixData, DTO);

            sw.Stop();
            Log.Instance.Debug("Update and merge complete in " + (int)sw.ElapsedMilliseconds / 1000 + " seconds");

            return theMatrixData;
        }



        /// <summary>
        /// Merge the existing, requested and default data. Sort it in SPC order 
        /// </summary>
        /// <param name="existingData"></param>
        /// <param name="newData"></param>
        /// <param name="defaultData"></param>
        /// <returns></returns>
        internal List<DataItem_DTO> MergeAndSortData(List<DataItem_DTO> existingData, List<DataItem_DTO> newData, List<DataItem_DTO> defaultData)
        {


            List<DataItem_DTO> merged = new List<DataItem_DTO>();
            var exEXnew = existingData.Except(newData);
            merged.AddRange(exEXnew);
            var newINTex = newData.Intersect(existingData);
            merged.AddRange(newINTex);
            var newEXex = newData.Except(existingData);
            merged.AddRange(newEXex);
            var devEXmer = defaultData.Except(merged);
            merged.AddRange(devEXmer);

            var merEXnew = merged.Except(newData);
            var merINTnew = merged.Intersect(newData);

            merged = new List<DataItem_DTO>();
            merged.AddRange(merEXnew);
            merged.AddRange(merINTnew);

            return merged.OrderBy(x => x.sortID).ToList();

        }

        /// <summary>
        /// Create the sort ids for each data item. This id's will be sequenial by SPC row-major order
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="theSpec"></param>
        internal void SetDataSortIds(ref List<DataItem_DTO> dto, Specification theSpec)
        {

            foreach (DataItem_DTO item in dto)
            {
                item.sortID = GetSortId(item, theSpec);

            }
        }

        /// <summary>
        /// Set the Sort Id root values for each dimension
        /// </summary>
        /// <param name="theSpec"></param>
        internal void SetMetadataSortIds(ref Specification theSpec, List<PeriodRecordDTO_Create> periods = null)
        {
            int total = 0;
            int pcount = periods == null ? 0 : periods.Count;

            for (int i = theSpec.Classification.Count - 1; i >= 0; i--)
            {
                int len = (int)Math.Log10(theSpec.Classification[i].Variable.Count) + 1;

                theSpec.Classification[i].IdMultiplier = total;
                total += len;
            }

            theSpec.Frequency.IdMultiplier = total;

            total += (int)Math.Log10(theSpec.Frequency.Period.Count + pcount) + 1;


            theSpec.StatisticMetadata = new StatisticMetadata() { IdMultiplier = total };

            int counter = 1;
            foreach (var s in theSpec.Statistic)
            {
                s.SortId = counter;
                counter++;
            }

            counter = 1;
            foreach (var p in theSpec.Frequency.Period)
            {
                p.SortId = counter;
                counter++;
            }

            foreach (var cls in theSpec.Classification)
            {
                counter = 1;
                foreach (var vrb in cls.Variable)
                {
                    vrb.SortId = counter;
                    counter++;
                }
            }
        }

        /// <summary>
        /// Convert the csv items in the request to a 2D array of data items
        /// </summary>
        /// <param name="periods"></param>
        /// <param name="theSpec"></param>
        /// <param name="DTO"></param>
        /// <returns></returns>
        private List<DataItem_DTO> GetInputObjectsJson(List<PeriodRecordDTO_Create> periods, Specification theSpec, BuildUpdate_DTO DTO)
        {
            List<DataItem_DTO> buildList = new List<DataItem_DTO>();
            int statMultiplier = theSpec.Frequency.IdMultiplier + (int)Math.Log10(theSpec.Statistic.Count) + 1;
            try
            {

                foreach (var item in DTO.PxData.DataItems)
                {
                    var v = Utility.GetCustomConfig("APP_CSV_VALUE");
                    Dictionary<string, string> readData = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                    DataItem_DTO readItem = new DataItem_DTO
                    {
                        dataValue = readData[Utility.GetCustomConfig("APP_CSV_VALUE")]
                    };
                    //Forget about this iteration if we can't line up with the Frequency code
                    if (!readData.ContainsKey(theSpec.Frequency.Code))
                    {
                        Log.Instance.Debug("**Build Validation** request item does not contain frequency code: " + theSpec.Frequency.Code);
                        item.updated = false;
                        item.duplicate = false;
                        continue;
                    }


                    readItem.period.Code = readData[theSpec.Frequency.Code] != null ? readData[theSpec.Frequency.Code] : "";

                    readItem.period = periods.Where(x => x.Code == readItem.period.Code).FirstOrDefault();

                    if (readItem.period == null)
                    {
                        Log.Instance.Debug("**Build Validation** Period not found ");
                        item.updated = false;
                        item.duplicate = false;
                        continue;
                    }

                    //Skip this iteration if the data doesn't contain the statistic indicator
                    if (!readData.ContainsKey(Utility.GetCustomConfig("APP_CSV_STATISTIC")))
                    {
                        Log.Instance.Debug("**Build Validation** statistic full set code not contained in " + Utility.GetCustomConfig("APP_CSV_STATISTIC"));
                        item.updated = false;
                        item.duplicate = false;
                        continue;
                    }

                    readItem.statistic.Code = readData[Utility.GetCustomConfig("APP_CSV_STATISTIC")];

                    var sttRead = theSpec.Statistic.Where(x => x.Code == readItem.statistic.Code).FirstOrDefault();
                    if (sttRead == null)
                    {
                        Log.Instance.Debug("**Build Validation** statistic items do not contain " + readItem.statistic.Code);
                        item.updated = false;
                        item.duplicate = false;
                        continue;
                    }

                    readItem.statistic.Value = sttRead != null ? sttRead.Value : "";
                    readItem.statistic.SortId = sttRead != null ? sttRead.SortId : 0;


                    foreach (var c in theSpec.Classification)
                    {
                        ClassificationRecordDTO_Create cls = new ClassificationRecordDTO_Create() { Code = c.Code, Value = c.Value, IdMultiplier = c.IdMultiplier };

                        var vrbCode = readData.ContainsKey(cls.Code) ? readData[cls.Code] : "";


                        VariableRecordDTO_Create vrb = c.Variable.Where(x => x.Code == vrbCode).FirstOrDefault();

                        var variables = new List<VariableRecordDTO_Create>() { vrb };
                        cls.Variable = variables;
                        readItem.classifications.Add(cls);
                    }



                    //Test the readItem to check if it validates against the px input and/or the new periods
                    // If it's ok then add it to our list
                    if (CheckReadItem(readItem, periods, theSpec))
                    {
                        readItem.sortID = GetSortId(readItem, theSpec);
                        //We need to link the sort id back to the DTO item in case we need to flag it as invalid later:
                        item.SortId = readItem.sortID;
                        buildList.Add(readItem);
                        item.updated = true;

                    }
                    else item.updated = false;

                    item.duplicate = readItem.duplicate;
                }
            }
            catch (Exception ex)
            {
                throw new UnmatchedParametersException(ex);
            }

            return buildList;
        }
        private long GetSortId(DataItem_DTO item, Specification theSpec)
        {
            long sortid = 0;

            sortid = (long)Math.Pow(10, theSpec.StatisticMetadata.IdMultiplier) * item.statistic.SortId;
            sortid += (long)Math.Pow(10, theSpec.Frequency.IdMultiplier) * item.period.SortId;
            foreach (var cls in item.classifications)
            {

                var v = (long)Math.Pow(10, cls.IdMultiplier) * cls.Variable[0].SortId;
                var w = (long)Math.Pow(10, cls.IdMultiplier);
                sortid = sortid + v + w;

            }
            item.sortID = sortid;
            return sortid;
        }


        /// <summary>
        /// Flag whether or not each data item will be updated - depends on validity
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="periods"></param>
        /// <param name="theSpec"></param>
        /// <returns></returns>

        private bool CheckReadItem(DataItem_DTO dataItem, List<PeriodRecordDTO_Create> periods, Specification theSpec)
        {
            if (theSpec.Statistic.Where(x => x.Code == dataItem.statistic.Code).Count() == 0)
            {
                Log.Instance.Debug("**Build Validation**" + " statistic:" + dataItem.statistic.Code);
                return false;
            }
            if (periods.Where(x => x.Code == dataItem.period.Code).Count() == 0)
            {
                Log.Instance.Debug("**Build Validation**" + " period:" + dataItem.period.Code);
                return false;
            }
            foreach (ClassificationRecordDTO_Create cls in dataItem.classifications)
            {
                var specClassification = theSpec.Classification.Where(x => x.Code == cls.Code).FirstOrDefault();
                if (specClassification == null)
                {
                    Log.Instance.Debug("**Build Validation**" + "classification code null:" + cls.Code);
                    return false;
                }
                if (specClassification.Variable.Count == 0)
                {
                    Log.Instance.Debug("**Build Validation**" + " variables null - cls code: " + specClassification.Code);
                    return false;
                }
                if (cls.Variable[0] == null)
                {
                    Log.Instance.Debug("**Build Validation**" + " first variable null - cls code: " + specClassification.Code);
                    return false;
                }
                if (specClassification.Variable.Where(x => x.Code == cls.Variable[0].Code).Count() == 0)
                {
                    Log.Instance.Debug("**Build Validation**" + " null variables2  " + specClassification.Code);
                    return false;
                }
            }


            return true;
        }



        /// <summary>
        /// Tests if the user has sufficient build permissions
        /// </summary>
        /// <param name="PrvCode"></param>
        /// <param name="BuildAction"></param>
        /// <returns></returns>
        internal bool HasBuildPermission(ADO ado, string CcnUsername, string BuildAction)
        {
            Account_ADO adoAccount = new Account_ADO();
            ADO_readerOutput result = adoAccount.Read(ado, CcnUsername);
            if (!result.hasData) return false;
            if (result.data == null) return false;
            if (result.data.Count == 0) return false;

            if (result.data[0].PrvCode.Equals(Constants.C_SECURITY_PRIVILEGE_MODERATOR))
            {
                return Configuration_BSO.GetCustomConfig(ConfigType.global, "build." + BuildAction + ".moderator");
            }
            return true;
        }

    }



}



/// <summary>
/// A class that contains a datpoint along with its associated unique metadata combination
/// </summary>
internal class DataItem_DTO : IEquatable<DataItem_DTO>
{
    /// <summary>
    /// The statistic for this point
    /// </summary>
    internal StatisticalRecordDTO_Create statistic { get; set; }

    /// <summary>
    /// The list of classifications for this point
    /// </summary>
    internal List<ClassificationRecordDTO_Create> classifications { get; set; }

    /// <summary>
    /// the period for this point
    /// </summary>
    internal PeriodRecordDTO_Create period { get; set; }

    /// <summary>
    /// The value for this point
    /// </summary>
    internal string dataValue { get; set; }



    /// <summary>
    /// A sort id - faster than a sort word
    /// </summary>
    internal long sortID { get; set; }

    internal string sortWord { get; set; }

    /// <summary>
    /// Flag to indicate if the data was amended (used for comparison)
    /// </summary>
    internal bool wasAmendment { get; set; }

    /// <summary>
    /// The TD_DATA id
    /// </summary>
    internal long id { get; set; }


    internal string identifier { get; set; }

    internal bool duplicate { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    internal DataItem_DTO()
    {
        statistic = new StatisticalRecordDTO_Create();
        classifications = new List<ClassificationRecordDTO_Create>();
        period = new PeriodRecordDTO_Create();
    }



    /// <summary>
    /// Override of Equals
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Boolean Equals(DataItem_DTO other)
    {
        //Try first to use the sortIds to denote equality, otherwise go into the deeper structure
        if (this.sortID == 0 || other.sortID == 0)
        {
            //This is by far the faster algorithm, but will only work if the identifier fields have been populated. Otherwise it does a deep comparison of both objects.
            //Try to ensure any object has the identifier field populated.
            if (this.identifier != null && other.identifier != null)
                return this.identifier.Equals(other.identifier);

            //No identifiers, deep comparison needed:

            //The statistic of this data item must match the other
            if (!this.statistic.Equals(other.statistic)) return false;
            //The period of this data item must match the other
            if (!this.period.Equals(other.period)) return false;
            //There must be no differing classifications for each DataItem_DTO
            if (this.classifications.Except(other.classifications).Count() > 0) return false;
            //Each corresponding classification must have the same variable 
            foreach (var cls in this.classifications)
            {
                ClassificationRecordDTO_Create clsOther = other.classifications.Where(x => x.Code == cls.Code).FirstOrDefault();
                if (clsOther == null) return false;
                VariableRecordDTO_Create vrbOther = clsOther.Variable.FirstOrDefault();
                if (vrbOther == null) return false;
                VariableRecordDTO_Create vrbThis = cls.Variable.FirstOrDefault();
                if (vrbThis == null) return false;
                //Corresponding variables must be the same
                if (!vrbThis.Equals(vrbOther)) return false;
            }
        }
        else
        {
            if (this.sortID != other.sortID) return false;
        }

        return true;
    }

    /// <summary>
    /// Hashcode for Equals override
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        // Try first to use the sortIds to denote equality, otherwise go into the deeper structure
        if (this.sortID > 0) return this.sortID.GetHashCode();

        if (this.identifier != null)
            return this.identifier.GetHashCode();

        int hashValuePeriod = this.period.GetHashCode();
        int hashValueStatistic = this.statistic.GetHashCode();
        int hashValue = 0;
        foreach (var cls in this.classifications)
        {
            if (cls.Variable.Count > 0)
            {
                hashValue = hashValue ^ cls.Variable[0].Code.GetHashCode();
                hashValue = hashValue ^ cls.Variable[0].Value.GetHashCode();
            }
        }
        hashValue = hashValue ^ this.period.GetHashCode();
        hashValue = hashValue ^ this.statistic.GetHashCode();

        return hashValue;
    }
}

/// <summary>
/// 
/// </summary>
internal class DimensionDetail_DTO
{
    /// <summary>
    /// 
    /// </summary>
    internal string key { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal string value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal DimensionValue_DTO dimensionValue { get; set; }
}

/// <summary>
/// 
/// </summary>
internal class DimensionValue_DTO
{
    /// <summary>
    /// 
    /// </summary>
    internal List<DimensionDetail_DTO> details { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal string value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal string code { get; set; }

    /// <summary>
    /// 
    /// </summary>
    internal DimensionType dimType { get; set; }


    /// <summary>
    /// 
    /// </summary>
    internal DimensionValue_DTO()
    {
        this.details = new List<DimensionDetail_DTO>();
    }
}


internal struct ClassificationVariable
{
    internal int clsId;
    internal int vrbId;

    internal ClassificationVariable(int cls, int vrb)
    {
        clsId = cls;
        vrbId = vrb;
    }

}

/// <summary>
/// 
/// </summary>
enum DimensionType { CLASSIFICATION, PERIOD, STATISTIC }
