using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using API;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxParser.Resources.Parser;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Resources.PxParser;
using PxStat.System.Settings;
using PxStat.Template;
using static PxStat.Data.Matrix;

namespace PxStat.Build
{
    /// <summary>
    /// Updates an existing px file with new data and period metadata. This may only be used for adding or removing periods.
    /// </summary>
    internal class Build_BSO_Update : BaseTemplate_Read<BuildUpdate_DTO, Build_VLD_Update>
    {
        /// <summary>
        /// 
        /// </summary>
        List<int> divisors = new List<int>();



        /// <summary>
        /// 
        /// </summary>
        List<DataItem_DTO> requestItems;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal Build_BSO_Update(JSONRPC_API request) : base(request, new Build.Build_VLD_Update())
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// 
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




            //Get this matrix from the px file , but we also need to pass in the Timeval stuff
            //The "" bit is temporary until we make the parameters optional (Currently this interferes with existing overloads)
            Matrix theMatrixData = new Matrix(PxDoc, DTO.FrqCodeTimeval ?? "", DTO.FrqValueTimeval ?? "");



            Matrix matrixNewMetadata = new Matrix(DTO);
            Request.parameters.GrpCode = Utility.GetCustomConfig("APP_DEFAULT_GROUP");
            Request.parameters.source = Utility.GetCustomConfig("APP_DEFAULT_SOURCE");
            //Get the Specifications for the new matrix:
            if (DTO.Dimension.Count > 0)
            {
                matrixNewMetadata.OtherLanguageSpec = new List<Matrix.Specification>();
                foreach (var dimension in DTO.Dimension)
                {
                    matrixNewMetadata.OtherLanguageSpec.Add(new Matrix.Specification(dimension.LngIsoCode, DTO));
                    //matrixNewMetadata.OtherLanguageSpec.Add(new Matrix.Specification());
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
                    Response.error = Label.Get("error.validation");
                    return false;
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
                requestItems = GetInputObjectsJson(stats, periods, classifications, variables, theSpec);

            //validate the individual dimensions
            if (requestItems != null)
            {
                Build_Update_VLD validator = new Build_Update_VLD(theSpec, requestItems);
                ValidationResult res = validator.Validate(DTO);
                if (!res.IsValid)
                {
                    Response.error = Label.Get("error.validation");
                    return false;
                }
            }

            //Get the current and new periods for each specification
            List<PeriodRecordDTO_Create> allPeriods = GetCurrentAndNewPeriods(theSpec, requestItems);


            //Sort the existing data in SPC order
            Build_BSO pBso = new Build_BSO();
            theMatrixData = pBso.ConvertCellsOrderToSPC(theMatrixData);


            //Get a List<DataItem_DTO> of the new requestItems
            if (requestItems != null)
                requestItems = tagNewData(theSpec, allPeriods, requestItems);

            //Get the existing data items as a list of DataItem_DTO with a sort word - !! - use CartesianProduct function..            
            var existingData = pBso.GetExistingDataItems(theMatrixData, theSpec);// 

            List<DataItem_DTO> allData;
            //..and merge the new data with the existing data
            if (requestItems != null)
                allData = MergeData(requestItems, existingData);
            else
                allData = existingData;

            //Ensure that any new periods are reflected in the matrix



            theMatrixData = updatePeriods(theMatrixData, theSpec, DTO.Dimension.ToList());

            //Merge the Metadata here...
            theMatrixData = mergeMetadata(theMatrixData, matrixNewMetadata);

            //Sort the merged data in SPC order
            allData = pBso.sortSPC(theSpec, allData, true);

            //Set the Cells to the merged and sorted data
            theMatrixData.Cells = pBso.GetNewCells(allData);

            List<dynamic> cells = new List<dynamic>();
            foreach (var c in theMatrixData.Cells)
            {
                dynamic cl = new ExpandoObject();
                cl.TdtValue = c.Value.ToString() == Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE") ? null : c.Value.ToString();
                cells.Add(cl);

            }
            theMatrixData.Cells = cells;

            if (DTO.FrmType == DatasetFormat.Px)
            {
                List<dynamic> resultPx = new List<dynamic>();
                resultPx.Add(theMatrixData.GetPxObject(true).ToString());
                Response.data = resultPx;



                return true;
            }

            else if (DTO.FrmType == DatasetFormat.JsonStat)
            {

                //Return the metadata and data, using one json-stat object for each specification
                List<JRaw> jsons = new List<JRaw>();
                List<string> languages = new List<string>();
                if (theMatrixData.Languages != null)
                {
                    foreach (var lang in theMatrixData.Languages)
                    {
                        languages.Add(lang.ToPxValue());
                    }
                }
                else languages.Add(theMatrixData.MainSpec.Language);

                foreach (var lang in languages)
                {
                    JsonStat json = theMatrixData.GetJsonStatObject(lang, false);
                    jsons.Add(new JRaw(Serialize.ToJson(json)));
                }

                Response.data = jsons;
                return true;
            }

            return false;
        }

        private Matrix mergeMetadata(Matrix existingMatrix, Matrix amendedMatrix)
        {
            existingMatrix.MainSpec = mergeSpecsMetadata(existingMatrix.MainSpec, amendedMatrix.GetSpecFromLanguage(existingMatrix.MainSpec.Language));

            if (existingMatrix.OtherLanguageSpec == null) return existingMatrix;

            List<Specification> otherSpecs = new List<Specification>();
            foreach (Specification spec in existingMatrix.OtherLanguageSpec)
            {
                otherSpecs.Add(mergeSpecsMetadata(spec, amendedMatrix.GetSpecFromLanguage(spec.Language)));
            }

            existingMatrix.OtherLanguageSpec = otherSpecs;

            return existingMatrix;
        }

        private Specification mergeSpecsMetadata(Specification existingSpec, Specification amendedSpec)
        {
            existingSpec.Title = amendedSpec.Title;
            existingSpec.Frequency.Value = amendedSpec.Frequency != null ? amendedSpec.Frequency.Value : existingSpec.Frequency.Value;
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

        private void updateMetadata(ref Matrix theMatrix, BuildUpdate_DTO dto)
        {
            string cprValue = "";
            Copyright_ADO cAdo = new Copyright_ADO();
            try
            {
                ADO Ado = new ADO("defaultConnection");
                Copyright_DTO_Read dtoCopyright = new Copyright_DTO_Read();
                dtoCopyright.CprCode = dto.CprCode;
                var result = cAdo.Read(Ado, dtoCopyright);
                if (result.hasData)
                    cprValue = result.data[0].CprValue;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Ado.Dispose();
            }
            if (DTO.MtrCode != null)
                theMatrix.Code = DTO.MtrCode;
            if (DTO.FrqCodeTimeval == null)
            {
                DTO.FrqCodeTimeval = theMatrix.MainSpec.Frequency.Code;
            }
            theMatrix.MainSpec.Frequency.Code = DTO.FrqCodeTimeval;
            theMatrix.MainSpec.Source = cprValue;

            Dimension_DTO dimDtoMain = DTO.getDimensionForLanguage(theMatrix.MainSpec.Language);

            theMatrix.MainSpec.Title = dimDtoMain.MtrTitle != null ? dimDtoMain.MtrTitle : theMatrix.MainSpec.Title;

            theMatrix.MainSpec.NotesAsString = dimDtoMain.MtrNote != null ? dimDtoMain.MtrNote : theMatrix.MainSpec.NotesAsString;

            theMatrix.MainSpec.ContentVariable = dimDtoMain.StatisticLabel != null ? dimDtoMain.StatisticLabel : theMatrix.MainSpec.ContentVariable;

            if (dimDtoMain.FrqValue == null)
                dimDtoMain.FrqValue = theMatrix.MainSpec.Frequency.Value;

            theMatrix.MainSpec = UpdateValuePairs(theMatrix.MainSpec, theMatrix.MainSpec.Frequency.Value, dimDtoMain.FrqValue);

            if (dimDtoMain.FrqValue != null)
                theMatrix.MainSpec.Frequency.Value = dimDtoMain.FrqValue;

            if (theMatrix.OtherLanguageSpec != null)
            {
                foreach (var spec in theMatrix.OtherLanguageSpec)
                {
                    spec.Frequency.Code = DTO.FrqCodeTimeval;
                    spec.Source = cprValue;
                    Dimension_DTO dimDto = DTO.getDimensionForLanguage(spec.Language);
                    spec.Title = dimDto.MtrTitle;
                    //Can't change spec here, but we need to do the equivalent of MainSpec change value pairs, i.e.
                    //theMatrix.MainSpec = UpdateValuePairs(theMatrix.MainSpec, theMatrix.MainSpec.Frequency.Value, dimDtoMain.FrqValue);
                    if (dimDto.FrqValue != null)
                        spec.Frequency.Value = dimDto.FrqValue;
                    spec.NotesAsString = dimDto.MtrNote;
                    spec.ContentVariable = dimDto.StatisticLabel;
                }
            }
        }

        /// <summary>
        /// Changes a key in MainValues of a specification where necessary
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="key"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        private Specification UpdateValuePairs(Specification spec, string key, string newKey)
        {
            List<KeyValuePair<string, IList<IPxSingleElement>>> outList = new List<KeyValuePair<string, IList<IPxSingleElement>>>();
            foreach (var pair in spec.MainValues)
            {
                if (pair.Key == key)
                {
                    KeyValuePair<string, IList<IPxSingleElement>> newPair = new KeyValuePair<string, IList<IPxSingleElement>>(newKey, pair.Value.ToList());
                    outList.Add(newPair);

                }
                else
                    outList.Add(pair);
            }
            spec.MainValues = outList;
            return spec;
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

        private List<DataItem_DTO> GetInputObjectsJson(Dictionary<string, string> stats, Dictionary<string, string> periods, Dictionary<string, string> classifications, Dictionary<string, string> variables, Specification theSpec)
        {
            List<DataItem_DTO> buildList = new List<DataItem_DTO>();

            try
            {
                foreach (var item in DTO.PxData.DataItems)
                {

                    Dictionary<string, string> readData = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                    DataItem_DTO readItem = new DataItem_DTO();
                    readItem.dataValue = readData[Label.Get("default.csv.value")];
                    readItem.period.Code = readData[theSpec.Frequency.Code];
                    readItem.period.Value = periods[readItem.period.Code];
                    readItem.statistic.Value = readData[theSpec.ContentVariable.ToUpper()];
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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        private string WriteDimensionViaParents(Dimension dim)
        {
            string readString = "";

            while (dim.ParentDimension != null)
            {
                readString = dim.Code + ',' + dim.Value + ',' + readString;
                dim = dim.ParentDimension;
            }

            return readString;
        }

        /// <summary>
        /// Get all of the Classification dimensions expressed as a single dimension structure
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <returns></returns>
        private Dimension GetClassificationDimensions(Matrix theMatrixData)
        {
            IEnumerable<ClassificationRecordDTO_Create> reverseCls = theMatrixData.MainSpec.Classification.Reverse();
            Dimension clsD = new Dimension();
            List<Dimension> previousDims = new List<Dimension>();
            Dimension previousDimension = new Dimension();
            List<Dimension> clsDims = new List<Dimension>();
            foreach (var cls in reverseCls)
            {

                clsDims = new List<Dimension>();
                foreach (var vrb in cls.Variable)
                {
                    Dimension vrbDim = new Dimension(cls.Code, vrb.Code);

                    if (previousDims.Count > 0) vrbDim.Dimensions = previousDims;

                    clsDims.Add(vrbDim);
                }

                previousDims = clsDims;

            }
            clsD.Dimensions = clsDims;

            return clsD;
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

    /// <summary>
    /// A Dimension refers to anything with a value and code that can be used for sorting data. It can be for a Statistic, a Period or a classification
    /// It contains a list of items that are below it in the sort order (Dimensions) and the item that is above it in the sort order (ParentDimension)
    /// </summary>
    internal class Dimension
    {
        /// <summary>
        /// Code
        /// </summary>
        internal string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal List<Dimension> Dimensions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal Dimension ParentDimension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        internal Dimension(string code, string value)
        {
            Code = code;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        internal Dimension()
        {
        }
    }



}
