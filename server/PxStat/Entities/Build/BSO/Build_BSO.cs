
using API;
using FluentValidation.Results;
using Newtonsoft.Json;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        /// <summary>
        /// class variable
        /// </summary>
        List<string> sortWords;

        /// <summary>
        /// class variable
        /// </summary>
        Matrix pxFileMatrix;
        Specification pxSpec;

        /// <summary>
        /// Converts the Cells of a Matrix to SPC order, irrespective of the order on the px file
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <returns></returns>
        internal Matrix ConvertCellsOrderToSPC(Matrix theMatrixData)
        {

            List<DataItem_DTO> sortedSpc = GetLabelledData(theMatrixData);

            theMatrixData.Cells = GetNewCells(sortedSpc);

            return theMatrixData;
        }

        /// <summary>
        /// Get an ordered list of cells from a list of DataItem_DTO
        /// </summary>
        /// <param name="sortedData"></param>
        /// <returns></returns>
        internal IList<dynamic> GetNewCells(List<DataItem_DTO> sortedData)
        {
            //Replace the existing cells with the sorted cell data
            List<dynamic> newCells = new List<dynamic>();
            foreach (var v in sortedData)
            {
                double outDouble;
                if (Double.TryParse(v.dataValue, out outDouble))
                {
                    PxDoubleValue newpx = new PxDoubleValue(Convert.ToDouble(v.dataValue));
                    newCells.Add(newpx);
                }
                else
                {
                    PxStringValue newpx = new PxStringValue(v.dataValue);
                    newCells.Add(newpx);
                }

            }
            return newCells;
        }

        /// <summary>
        /// Get the existing data as a list of DataItem_DTO, i.e. the data with its associated classifications, statistic, variables
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <returns></returns>
        internal List<DataItem_DTO> GetLabelledData(Matrix theMatrixData)
        {
            pxFileMatrix = theMatrixData;
            List<dynamic> dimensions = new List<dynamic>();

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

            //Get a list of Dimension combinations based on the VALUES of the matrix main spec
            //There number of these should be exactly the same as the number of data cells
            sortWords = new List<string>();
            int wcount = 0;
            getSortWords(dtoList, dtoList.FirstOrDefault(), 0, ref wcount, "");


            //associate the data from the cells to each fully qualified dimension combination
            List<DataItem_DTO> sortedWithData = new List<DataItem_DTO>();
            int counter = 0;
            foreach (string sw in sortWords)
            {
                DataItem_DTO dtoData = new DataItem_DTO();
                dtoData.sortWord = sw;
                dtoData.dataValue = theMatrixData.Cells[counter].ToString();
                sortedWithData.Add(dtoData);
                counter++;
            }

            //Now sort the matrix cells using SPC sorting 
            // return sortSPC(theMatrixData.MainSpec, sortedWithData);

            return sortedWithData;
        }


        internal List<DataItem_DTO> GetMatrixDataItems(Matrix theMatrix, string lngIsoCode)
        {



            if (lngIsoCode != null)
            {
                pxSpec = theMatrix.GetSpecFromLanguage(lngIsoCode);
                if (pxSpec == null) pxSpec = theMatrix.MainSpec;
                pxSpec.Frequency = theMatrix.MainSpec.Frequency;
            }
            else
                pxSpec = theMatrix.MainSpec;

            List<DimensionValue_DTO> dimensions = new List<DimensionValue_DTO>();


            DimensionValue_DTO sVal = new DimensionValue_DTO();
            sVal.dimType = DimensionType.STATISTIC;
            foreach (StatisticalRecordDTO_Create stat in pxSpec.Statistic)
            {
                DimensionDetail_DTO detail = new DimensionDetail_DTO();
                detail.key = stat.Code;
                detail.value = stat.Value;
                detail.dimensionValue = sVal;
                sVal.details.Add(detail);
            }
            dimensions.Add(sVal);


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
            dimensions.Add(pVal);

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
                dimensions.Add(cVal);

            }

            var graph = CartesianProduct(dimensions[0].details.ToArray());

            for (int i = 1; i < dimensions.Count; i++)
            {
                graph = CartesianProduct(graph.ToArray(), dimensions[i].details.ToArray());
            }

            List<DataItem_DTO> itemList = new List<DataItem_DTO>();

            int counter = 0;
            foreach (var item in graph)
            {
                DataItem_DTO dto = new DataItem_DTO();
                populateDataItem(ref dto, item, true);
                dto.sortWord = dto.sortWord + counter;
                dto.dataValue = theMatrix.Cells[counter].TdtValue.ToString();
                itemList.Add(dto);
                counter++;
            }

            return itemList;
        }

        /// <summary>
        /// Gets a list of DataItem_DTO from the metadata of a Matrix
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="getIds"></param>
        /// <returns></returns>
        internal List<DataItem_DTO> GetExistingDataItems(Matrix theMatrix, Specification theSpec, bool getIds = false, bool newData = false)
        {
            List<dynamic> dimensions = new List<dynamic>();

            pxFileMatrix = theMatrix;
            pxSpec = theSpec;

            //Get a list of dimensions from the VALUES of the px file (via MainSpec.MainValues)
            // A dimension is any Statistic, classification or period
            List<DimensionValue_DTO> dtoList = new List<DimensionValue_DTO>();
            foreach (var pair in theSpec.MainValues)
            {
                dtoList = getDimensionCodes(theMatrix, dtoList, pair, theSpec);
            }

            //If there is no specified STATISTIC in the VALUES then the statistic must be inferred from the CONTENTS
            if ((dtoList.Where(f => f.dimType == DimensionType.STATISTIC)).Count() == 0)
            {
                DimensionValue_DTO statDim = new DimensionValue_DTO();
                statDim.code = "0";
                statDim.value = theSpec.Contents;
                statDim.dimType = DimensionType.STATISTIC;

                foreach (var stat in theSpec.Statistic)
                {
                    DimensionDetail_DTO dtoDetail = new DimensionDetail_DTO();
                    dtoDetail.key = stat.Code;
                    dtoDetail.value = stat.Value;
                    dtoDetail.dimensionValue = statDim;
                    statDim.details.Add(dtoDetail);

                }
                dtoList.Add(statDim);
            }

            var graph = CartesianProduct(dtoList[0].details.ToArray());

            for (int i = 1; i < dtoList.Count; i++)
            {
                graph = CartesianProduct(graph.ToArray(), dtoList[i].details.ToArray());
            }

            List<DataItem_DTO> itemList = new List<DataItem_DTO>();

            int counter = 0;
            foreach (var item in graph)
            {
                DataItem_DTO dto = new DataItem_DTO();
                populateDataItem(ref dto, item, getIds);
                dto.sortWord = dto.sortWord + counter;
                dto.dataValue = newData ? Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE") : theMatrix.Cells[counter].Value.ToString();
                itemList.Add(dto);
                counter++;
            }

            return itemList;
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
                            break;
                        case DimensionType.PERIOD:
                            dto.period.Code = v.key;
                            dto.period.Value = v.value;
                            if (getIds)
                            {
                                PeriodRecordDTO_Create newPer = pxSpec.Frequency.Period.Where(x => x.Code == v.key).FirstOrDefault();
                                dto.period.FrequencyPeriodId = newPer.FrequencyPeriodId;
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
                            }
                            cls.ClassificationId = newCls.ClassificationId;
                            cls.Value = newCls.Value;
                            cls.Variable = new List<VariableRecordDTO_Create>();
                            cls.Variable.Add(vrb);

                            dto.classifications.Add(cls);
                            break;

                    }
                    dto.sortWord = dto.sortWord + v.dimensionValue.dimType + '/' + v.key + '/' + v.value + '~';
                }

            }
        }


        /*public static async Task LoopAsync(IEnumerable<string> thingsToLoop)
{
    List<Task> listOfTasks = new List<Task>();

    foreach (var thing in thingsToLoop)
    {
        listOfTasks.Add(DoAsync(thing));
    }

    await Task.WhenAll(listOfTasks);
}*/

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
            int trancheSize = Convert.ToInt32(Utility.GetCustomConfig("APP_BULKCOPY_TRANCHE_MULTIPLIER")) * Convert.ToInt32(ConfigurationManager.AppSettings["API_ADO_BULKCOPY_BATCHSIZE"]);
            Log.Instance.Debug(graph.Count().ToString() + " rows of data to be uploaded.");
            int counter = 0;

            //We only load data for the specification corresponding to the default language.
            Specification dataSpec = theMatrixData.MainSpec;
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
                var v = theMatrixData.Cells[counter].Value.ToString();
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
        /// Recursive function create a list of sort words. There should be the same number of these as cells in the matrix.
        /// Each item is a combination of the relevant dimensions that define that data point.
        /// </summary>
        /// <param name="dtoList"></param>
        /// <param name="dto"></param>
        /// <param name="dimCounter"></param>
        /// <param name="wordCounter"></param>
        /// <param name="line"></param>
        private void getSortWords(List<DimensionValue_DTO> dtoList, DimensionValue_DTO dto, int dimCounter, ref int wordCounter, string line)
        {
            foreach (var detail in dto.details)
            {
                if (dimCounter >= dtoList.Count - 1)
                {
                    //We've finished creating a sort word, now complete it and store it in the sortWords list
                    wordCounter++;
                    sortWords.Add(line + dto.dimType + '/' + detail.key + '/' + detail.value + '~' + wordCounter);

                }
                else
                {
                    //The sort word isn't finished yet, so we have to fetch the next dimension and run that recursively through this function
                    dimCounter++;
                    string oldLine = line;
                    line = line + dto.dimType + '/' + detail.key + '/' + detail.value + '~';
                    DimensionValue_DTO dtoNext = dtoList.ElementAt(dimCounter);
                    getSortWords(dtoList, dtoNext, dimCounter, ref wordCounter, line);
                    //return from recursion, step back.
                    dimCounter--;
                    line = oldLine;
                }
            }
        }




        /// <summary>
        /// Sort the list of sortword/data objects by SPC (Statistic, Period, Classification) order
        /// The general method is 1. Calculate the expected sort word for the first item on the sort order
        /// 2. Get all objects with that word
        /// 3. Do that for each item in that dimension
        /// 4. The data is now sorted by the dimension
        /// 5. Repeat for each dimension (but in reverse order)
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="sortedWithData"></param>
        /// <returns></returns>
        internal List<DataItem_DTO> sortSPC(Specification theSpec, List<DataItem_DTO> sortedWithData, bool sortByClassifications = false)
        {
            /*
             * The sorting happens in reverse order. So we sort from the last classification to the first, then period, then statistic.
             * Under normal circmstances we don't sort by classfication because we assume the data is self sorted.
             * However, an execption to this rule is for PxBuild Update. When we sort the individual data items sent by the user,
             * there is no guarantee that the data is pre-sorted in any sense. Thus there is an optional parameter to sort by classification.
             */

            if (sortByClassifications)
            {
                List<DataItem_DTO> sortedByCls = new List<DataItem_DTO>();


                //foreach (var cls in theSpec.Classification)
                for (int i = theSpec.Classification.Count - 1; i >= 0; i--)
                {
                    foreach (var vrb in theSpec.Classification[i].Variable)
                    {

                        IEnumerable<DataItem_DTO> sortedOneCls = sortedWithData.Where(f => f.classifications[i].Variable.Contains(vrb));
                        sortedByCls.AddRange(sortedOneCls);
                    }
                    sortedWithData = sortedByCls;
                    sortedByCls = new List<DataItem_DTO>();
                }


            }

            //Sort by period
            List<DataItem_DTO> sortedByPeriod = new List<DataItem_DTO>();
            foreach (var period in theSpec.Frequency.Period)
            {

                IEnumerable<DataItem_DTO> sortedOnePeriod = sortedWithData.Where(f => f.period.Code == period.Code); // only works when a fully populated dataset is sent in
                sortedByPeriod.AddRange(sortedOnePeriod);
            }

            //Sort by statistic
            List<DataItem_DTO> sortedByStat = new List<DataItem_DTO>();
            foreach (var stat in theSpec.Statistic)
            {

                IEnumerable<DataItem_DTO> sortedOneStat = sortedByPeriod.Where(f => f.statistic.Code == stat.Code);
                sortedByStat.AddRange(sortedOneStat);
            }

            return sortedByStat;

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


            if (lngIsoCode == null) lngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");

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


        /// <summary>
        /// Update and return a matrix based on specific DTO parameters
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="DTO"></param>
        /// <param name="Ado"></param>
        /// <param name="addedData"></param>
        /// <returns></returns>
        internal Matrix UpdateMatrixFromDto(Matrix theMatrixData, BuildUpdate_DTO DTO, ADO Ado, bool addedData = true, bool newPeriodsOnly = false)
        {

            List<DataItem_DTO> requestItems = new List<DataItem_DTO>();


            Matrix matrixNewMetadata = new Matrix(DTO);
            if (DTO.Dimension.Count > 0)
            {
                matrixNewMetadata.OtherLanguageSpec = new List<Matrix.Specification>();
                foreach (var dimension in DTO.Dimension)
                {
                    matrixNewMetadata.OtherLanguageSpec.Add(new Matrix.Specification(dimension.LngIsoCode, DTO));

                }

            }

            Matrix.Specification theSpec = theMatrixData.GetSpecFromLanguage(DTO.LngIsoCode);


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

            Dictionary<string, string> stats = new Dictionary<string, string>();
            Dictionary<string, string> periods = new Dictionary<string, string>();
            Dictionary<string, string> classifications = new Dictionary<string, string>();
            Dictionary<string, string> variables = new Dictionary<string, string>();

            List<StatisticalRecordDTO_Create> sList = new List<StatisticalRecordDTO_Create>();

            foreach (var stat in theMatrixData.MainSpec.Statistic)
            {
                stats.Add(stat.Code, stat.Value);
                StatisticalRecordDTO_Create newStat = new StatisticalRecordDTO_Create();
                newStat.Code = stat.Code;
                newStat.Value = stat.Value;
                newStat.Decimal = stat.Decimal;
                newStat.Unit = stat.Unit;
                sList.Add(newStat);
            }

            //Periods may have been added or removed. Furthermore, the same periods may have different values for different languages
            // First we create a list for the specification we're using for validation
            foreach (var dim in DTO.Dimension)
            {
                if (dim.LngIsoCode == theSpec.Language)
                {
                    foreach (var per in theSpec.Frequency.Period)
                    {
                        periods.Add(per.Code, per.Value);
                    }
                    if (dim.Frequency != null)
                    {
                        foreach (var per in dim.Frequency.Period)
                        {
                            if (!periods.ContainsKey(per.Code))
                            {
                                periods.Add(per.Code, per.Value);
                            }
                        }
                    }

                }

            }


            foreach (var cls in theSpec.Classification)
            {
                classifications.Add(cls.Code, cls.Value);
                foreach (var vrb in cls.Variable)
                {
                    variables.Add(cls.Code + vrb.Code, vrb.Value);
                }
            }



            //Get the new data and metadata from the csv input in the DTO
            if (DTO.PxData != null)
                requestItems = GetInputObjectsJson(stats, periods, classifications, variables, theSpec, DTO);



            //validate the individual dimensions
            if (requestItems != null)
            {
                Build_Update_VLD validator = new Build_Update_VLD(theSpec, requestItems);
                ValidationResult res = validator.Validate(DTO);
                if (!res.IsValid)
                {
                    return null;
                }
            }

            //Get the current and new periods for each specification
            List<PeriodRecordDTO_Create> allPeriods = GetCurrentAndNewPeriods(theSpec, requestItems);


            //Sort the existing data in SPC order
            Build_BSO pBso = new Build_BSO();
            theMatrixData = pBso.ConvertCellsOrderToSPC(theMatrixData);




            //Create a list of Data_Item_DTO where periods object does not intersect with allPeriods (This is the new period that is only in the dimension part of the DTO)
            //Then tag the new data and get it ready for sorting

            if (!addedData)
            {
                if (!newPeriodsOnly)
                {
                    dynamic existingPeriods = theMatrixData.MainSpec.Frequency.Period;
                    List<DataItem_DTO> newList = new List<DataItem_DTO>();
                    var diffPeriods = periods.Where(p => !allPeriods.Any(p2 => p2.Code == p.Key));
                    List<PeriodRecordDTO_Create> newPeriods = new List<PeriodRecordDTO_Create>();
                    foreach (var p in diffPeriods)
                    {
                        newPeriods.Add(new PeriodRecordDTO_Create() { Code = p.Key, Value = p.Value });
                    }

                    Matrix tempMatrix = theMatrixData;
                    tempMatrix.MainSpec.Frequency.Period = newPeriods;
                    requestItems = pBso.GetExistingDataItems(tempMatrix, tempMatrix.MainSpec, false, true);

                    theMatrixData.MainSpec.Frequency.Period = existingPeriods;
                    foreach (var v in diffPeriods)
                    {
                        if (allPeriods.Where(x => x.Code == v.Key).Count() == 0)
                            allPeriods.Add(new PeriodRecordDTO_Create { Code = v.Key, Value = v.Value });
                    }
                }
                else
                {

                    allPeriods = DTO.Dimension[0].Frequency.Period;
                    theMatrixData.MainSpec.Frequency.Period = allPeriods;
                    theSpec = theMatrixData.MainSpec;
                    requestItems = pBso.GetExistingDataItems(theMatrixData, theSpec, false, true);
                }
            }

            //Get a List<DataItem_DTO> of the new requestItems
            if (requestItems != null)
                requestItems = tagNewData(theSpec, allPeriods, requestItems);

            var existingData = new List<DataItem_DTO>();
            if (newPeriodsOnly)
            {
                existingData = requestItems;
            }
            else
                //Get the existing data items as a list of DataItem_DTO with a sort word - !! - use CartesianProduct function..            
                existingData = pBso.GetExistingDataItems(theMatrixData, theSpec);// 

            List<DataItem_DTO> allData;
            //..and merge the new data with the existing data
            if (requestItems != null)
                allData = MergeData(requestItems, existingData);
            else
                allData = existingData;



            theMatrixData = updatePeriods(theMatrixData, theSpec, DTO.Dimension.ToList());

            //Merge the Metadata here...
            theMatrixData = mergeMetadata(theMatrixData, matrixNewMetadata);

            //Should we get a new copyright object for the Matrix?
            if (matrixNewMetadata.Copyright.CprCode != null)
            {
                Copyright_BSO cBso = new Copyright_BSO();
                theMatrixData.Copyright = cBso.Read(matrixNewMetadata.Copyright.CprCode);
            }


            //Sort the merged data in SPC order
            allData = pBso.sortSPC(theSpec, allData, true);

            //Set the Cells to the merged and sorted data
            theMatrixData.Cells = pBso.GetNewCells(allData);


            List<dynamic> cells = new List<dynamic>();
            foreach (var c in theMatrixData.Cells)
            {
                dynamic cl = new ExpandoObject();
                cl.TdtValue = c.Value.ToString() == null ? Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE") : c.Value.ToString();
                cells.Add(cl);

            }
            theMatrixData.Cells = cells;

            return theMatrixData;
        }

        /// <summary>
        /// Get the csv items expressed as a list of DataItem_DTO
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="periods"></param>
        /// <param name="classifications"></param>
        /// <param name="variables"></param>
        /// <param name="theSpec"></param>
        /// <param name="DTO"></param>
        /// <returns></returns>
        private List<DataItem_DTO> GetInputObjectsJson(Dictionary<string, string> stats, Dictionary<string, string> periods, Dictionary<string, string> classifications, Dictionary<string, string> variables, Specification theSpec, BuildUpdate_DTO DTO)
        {
            List<DataItem_DTO> buildList = new List<DataItem_DTO>();

            try
            {
                foreach (var item in DTO.PxData.DataItems)
                {

                    Dictionary<string, string> readData = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                    DataItem_DTO readItem = new DataItem_DTO();
                    readItem.dataValue = readData[Utility.GetCustomConfig("APP_CSV_VALUE")];
                    readItem.period.Code = readData[theSpec.Frequency.Code];
                    readItem.period.Value = periods[readItem.period.Code];
                    readItem.statistic.Code = readData[Utility.GetCustomConfig("APP_CSV_STATISTIC")];
                    //readItem.statistic.Code = readData[theSpec.ContentVariable];
                    readItem.statistic.Value = theSpec.Statistic.Where(x => x.Code == readItem.statistic.Code).FirstOrDefault().Value;

                    foreach (var clsDict in classifications)
                    {
                        ClassificationRecordDTO_Create cls = new ClassificationRecordDTO_Create();
                        cls.Code = clsDict.Key;
                        cls.Value = clsDict.Value;
                        List<VariableRecordDTO_Create> vrbList = new List<VariableRecordDTO_Create>();
                        VariableRecordDTO_Create vrb = new VariableRecordDTO_Create();
                        vrb.Value = readData[cls.Code];
                        vrb.Code = vrb.Value;
                        vrbList.Add(vrb);
                        cls.Variable = vrbList;
                        readItem.classifications.Add(cls);
                    }

                    buildList.Add(readItem);
                }
            }
            catch (Exception ex)
            {
                throw new UnmatchedParametersException(ex);
            }

            return buildList;
        }

        private Matrix mergeMetadata(Matrix existingMatrix, Matrix amendedMatrix)
        {
            existingMatrix.MainSpec = mergeSpecsMetadata(existingMatrix.MainSpec, amendedMatrix.GetSpecFromLanguage(existingMatrix.MainSpec.Language));



            if (existingMatrix.OtherLanguageSpec == null) return existingMatrix;

            List<Specification> otherSpecs = new List<Specification>();
            foreach (Specification spec in existingMatrix.OtherLanguageSpec)
            {
                spec.Source = existingMatrix.MainSpec.Source;

                otherSpecs.Add(mergeSpecsMetadata(spec, amendedMatrix.GetSpecFromLanguage(spec.Language)));
            }

            existingMatrix.OtherLanguageSpec = otherSpecs;

            return existingMatrix;
        }

        private Specification mergeSpecsMetadata(Specification existingSpec, Specification amendedSpec)
        {
            existingSpec.Title = amendedSpec.Title;
            existingSpec.Contents = amendedSpec.Contents;
            if (amendedSpec.Frequency != null)
            {
                existingSpec.Frequency.Value = amendedSpec.Frequency.Value != null ? amendedSpec.Frequency.Value : existingSpec.Frequency.Value;
            }

            existingSpec.NotesAsString = amendedSpec.NotesAsString != null ? amendedSpec.NotesAsString : existingSpec.NotesAsString;
            existingSpec.ContentVariable = amendedSpec.ContentVariable != null ? amendedSpec.ContentVariable : existingSpec.ContentVariable;

            if (amendedSpec.Classification != null)
            {
                foreach (ClassificationRecordDTO_Create cls in existingSpec.Classification)
                {
                    var newcls = amendedSpec.Classification.Where(x => x.Code == cls.Code).FirstOrDefault();
                    if (newcls != null)
                        cls.GeoUrl = newcls.GeoUrl;
                }
            }

            return existingSpec;
        }

        /// <summary>
        /// Add the correct list of periods to the Matrix with the correct version for each language
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="theSpec"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private Matrix updatePeriods(Matrix theMatrix, Specification theSpec, List<Dimension_DTO> dimensions)
        {
            List<Specification> specs = new List<Specification>();
            Specification matrixSpec = theMatrix.MainSpec;

            theMatrix.MainSpec.Frequency.Period = getPeriodsForSpec(theMatrix.MainSpec, dimensions);

            if (theMatrix.OtherLanguageSpec != null)
            {
                foreach (Specification spec in theMatrix.OtherLanguageSpec)
                {

                    spec.Frequency.Period = getPeriodsForSpec(spec, dimensions);

                    specs.Add(spec);
                }
                theMatrix.OtherLanguageSpec = specs;
            }
            return theMatrix;
        }

        /// <summary>
        /// Get a list of periods for a specification that include the new periods in the request
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        private List<PeriodRecordDTO_Create> getPeriodsForSpec(Specification spec, List<Dimension_DTO> dimensions)
        {
            List<PeriodRecordDTO_Create> periods = new List<PeriodRecordDTO_Create>();

            foreach (var dim in dimensions)
            {
                if (dim.LngIsoCode == spec.Language)
                {
                    periods = spec.Frequency.Period;
                    if (dim.Frequency != null)
                    {
                        foreach (var dimPeriod in dim.Frequency.Period)
                        {
                            if (!periods.Any(x => x.Code == dimPeriod.Code && x.Value == dimPeriod.Value))
                            {
                                periods.Add(new PeriodRecordDTO_Create() { Code = dimPeriod.Code, Value = dimPeriod.Value });
                            }
                        }
                    }
                }

            }

            return periods;
        }


        /// <summary>
        /// Merge and sort the old data and new data
        /// New data is inserted in the correct position
        /// Updated data is replaced in the existing data by its equivalent in the new data
        /// New periods are added to the periods in the existing data periods
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        private List<DataItem_DTO> MergeData(List<DataItem_DTO> newData, List<DataItem_DTO> existingData)
        {
            //Rules:
            //If an existing item has a null period code then it's dummy data and should be removed.
            //If something is in the new but not in the existing, add the new
            //If something is in both the new and the existing, replace the old with the new
            //If something is in the existing but not in the new then leave it alone

            List<DataItem_DTO> merged = new List<DataItem_DTO>();

            List<DataItem_DTO> intersection = newData.Intersect(existingData).ToList();

            //First we can now remove any dummy data. This is identifiable by having a period value of null.
            existingData.RemoveAll(x => x.period.Code == null);

            merged = existingData;

            //In the new data but not in the old (additions) or in the old but not the new (leave alone)
            var newExceptExisiting = newData.Except(existingData);
            if (newExceptExisiting != null)
            {
                if (newExceptExisiting.Count() > 0)
                    merged.AddRange(newExceptExisiting);
            }

            //updates
            merged = merged.Except(intersection).ToList();
            merged.AddRange(intersection);

            return merged;
        }

        /// <summary>
        /// Attaches a sort word to each data item. This is based on the existing sort order of the px file.
        /// The sort word is the same format as that of the main data. This enables us to sort old and new data using the same standards
        /// </summary>
        /// <param name="theMatrixData"></param>
        private List<DataItem_DTO> tagNewData(Specification theSpec, List<PeriodRecordDTO_Create> allPeriods, List<DataItem_DTO> rows)
        {
            List<DataItem_DTO> sortedWithData = new List<DataItem_DTO>();
            foreach (var row in rows)
            {
                row.sortWord = getSortWord(theSpec, allPeriods, row).ToSortString();
            }

            return rows;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="periods"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private DataItem_DTO getSortWord(Specification theSpec, List<PeriodRecordDTO_Create> periods, DataItem_DTO item)
        {
            //First we must complete the DataItem_DTO to get the data other than just the codes
            //We do this by looking up the values based on the codes that were supplied
            //For an individual item, there will be:
            //One statistic
            //One period
            //Many classifications - each classification having one variable
            if (theSpec.Statistic.Count == 1)
            {
                item.statistic = theSpec.Statistic[0];
            }
            else
            {
                item.statistic = theSpec.Statistic.Where(x => x.Code == item.statistic.Code).FirstOrDefault();
                item.period = periods.Where(x => x.Code == item.period.Code).FirstOrDefault();
            }

            foreach (var cls in item.classifications)
            {
                ClassificationRecordDTO_Create newCls = new ClassificationRecordDTO_Create();
                newCls = theSpec.Classification.Where(x => x.Code == cls.Code).FirstOrDefault();
                cls.Value = newCls.Value;
                foreach (var vrb in cls.Variable)
                {
                    vrb.Value = newCls.Variable.Where(x => x.Code == vrb.Code).FirstOrDefault().Value;
                }
            }
            return item;
        }

        private List<PeriodRecordDTO_Create> GetCurrentAndNewPeriods(Specification spec, List<DataItem_DTO> requestItems)
        {
            List<PeriodRecordDTO_Create> periods = new List<PeriodRecordDTO_Create>();

            //First, the existing periods
            foreach (var period in spec.Frequency.Period)
            {
                if (period.Code != Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE"))
                {
                    periods.Add(period);
                }
            }

            if (requestItems == null) return periods;

            //Now we add on any new periods that have been passed in as part of the request
            foreach (DataItem_DTO item in requestItems)
            {

                if (periods.Where(x => x.Code == item.period.Code).Count() == 0)
                {
                    PeriodRecordDTO_Create period = new PeriodRecordDTO_Create();
                    period.Code = item.period.Code;
                    period.Value = item.period.Value;
                    periods.Add(period);
                }
            }
            return periods;
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
    /// A sort word
    /// </summary>
    internal string sortWord { get; set; }

    /// <summary>
    /// Flag to indicate if the data was amended (used for comparison)
    /// </summary>
    internal bool wasAmendment { get; set; }

    /// <summary>
    /// The TD_DATA id
    /// </summary>
    internal long id { get; set; }

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
    /// Generate a sort string for the data point based on its metadata
    /// </summary>
    /// <returns></returns>
    internal string ToSortString()
    {
        //return the appropriate sort word
        string sort = "";
        sort = sort + DimensionType.STATISTIC.ToString() + '/' + statistic.Code + '/' + statistic.Value + '~';
        sort = sort + DimensionType.PERIOD + '/' + period.Code + '/' + period.Value + '~';
        foreach (var cls in this.classifications)
        {
            foreach (var vrb in cls.Variable)
            {
                sort = sort + DimensionType.CLASSIFICATION + '/' + vrb.Code + '/' + vrb.Value + '~';
            }
        }
        return sort;
    }

    /// <summary>
    /// Override of Equals
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public Boolean Equals(DataItem_DTO other)
    {
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

        return true;
    }

    /// <summary>
    /// Hashcode for Equals override
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {

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
