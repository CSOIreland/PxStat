
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API;
using PxParser.Resources.Parser;
using PxStat.Data;
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


        /// <summary>
        /// Gets a list of DataItem_DTO from the metadata of a Matrix
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="getIds"></param>
        /// <returns></returns>
        internal List<DataItem_DTO> GetExistingDataItems(Matrix theMatrixData, Specification theSpec, bool getIds = false)
        {
            List<dynamic> dimensions = new List<dynamic>();

            pxFileMatrix = theMatrixData;
            pxSpec = theSpec;

            //Get a list of dimensions from the VALUES of the px file (via MainSpec.MainValues)
            // A dimension is any Statistic, classification or period
            List<DimensionValue_DTO> dtoList = new List<DimensionValue_DTO>();
            foreach (var pair in theSpec.MainValues)
            {
                dtoList = getDimensionCodes(theMatrixData, dtoList, pair, theSpec);
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
                dto.dataValue = theMatrixData.Cells[counter].Value.ToString();
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
            foreach (var item in graph)
            {
                //Simply a list of classification id /variable id pairs
                List<ClassificationVariable> classVars = new List<ClassificationVariable>();

                DataRow tdRow = tdData.NewRow();


                ClassificationVariable clsVar = new ClassificationVariable(0, 0);
                //Start off the recursive function and get one data row
                populateDataItemForDataTables(ref tdRow, ref classVars, item, getIds);

                tdRow["TDT_MTR_ID"] = theMatrixData.MainSpec.MatrixId;
                tdRow["TDT_IX"] = counter;
                tdRow["TDT_VALUE"] = theMatrixData.Cells[counter].Value.ToString();

                tdData.Rows.Add(tdRow);

                //We create a cell row for each item in the classVars struct but we relate it to main table
                foreach (ClassificationVariable cv in classVars)
                {
                    DataRow cellRow = cellData.NewRow();
                    cellRow["DTC_TDT_IX"] = counter;
                    cellRow["DTC_MTR_ID"] = theMatrixData.MainSpec.MatrixId;
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
                List<DataItem_DTO> sortedByCls = new List<Build.DataItem_DTO>();


                //foreach (var cls in theSpec.Classification)
                for (int i = theSpec.Classification.Count - 1; i >= 0; i--)
                {
                    foreach (var vrb in theSpec.Classification[i].Variable)
                    {

                        IEnumerable<DataItem_DTO> sortedOneCls = sortedWithData.Where(f => f.classifications[i].Variable.Contains(vrb));
                        sortedByCls.AddRange(sortedOneCls);
                    }
                    sortedWithData = sortedByCls;
                    sortedByCls = new List<Build.DataItem_DTO>();
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
            List<DataItem_DTO> sortedByStat = new List<Build.DataItem_DTO>();
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

            string statName = (theSpec.ContentVariable != null ? theSpec.ContentVariable : Label.Get("default.statistic")).ToUpper();
            // headerString.Add(new PxQuotedValue(Bracket(statName) + Label.Get("default.csv.code")).ToPxString());
            headerString.Add(new PxQuotedValue(statName).ToPxString());

            //headerString.Add(new PxQuotedValue(theSpec.Frequency.Value.ToUpper() + Bracket(theSpec.Frequency.Code) + Label.Get("default.csv.code")).ToPxString());
            headerString.Add(new PxQuotedValue(theSpec.Frequency.Code).ToPxString());


            foreach (var cls in theSpec.Classification)
            {
                //headerString.Add(new PxQuotedValue(cls.Value.ToUpper() + Bracket(cls.Code.ToUpper()) + Label.Get("default.csv.code")).ToPxString());
                headerString.Add(new PxQuotedValue(cls.Code).ToPxString());
            }

            headerString.Add(new PxQuotedValue(Label.Get("default.csv.value")).ToPxString());
            return (string.Join(",", headerString.ToArray()));
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
            this.details = new List<Build.DimensionDetail_DTO>();
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
}
