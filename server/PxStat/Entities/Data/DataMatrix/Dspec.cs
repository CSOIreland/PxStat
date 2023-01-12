using API;
using FluentValidation.Results;
using Newtonsoft.Json;
using PxParser.Resources.Parser;
using PxStat.DataStore;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxStat.Data
{
    /// <summary>
    /// The Dspec class contains the data that is language dependent
    /// Each matrix must have at least one Dspec
    /// </summary>
    /// 

    public class Dspec : IDspec
    {
        //A list of the dimensions for this spec
        public ICollection<StatDimension> Dimensions { get; set; } = new List<StatDimension>();
        //The Contents of the dataset
        public string Contents { get; set; }
        //The name of the statistic dimension if required
        public string ContentVariable { get; set; }
        //The url for copyright
        public string CopyrightUrl { get; set; }
        //Number of decimal places for values
        public short Decimals { get; set; }
        //The iso language code for this spec
        public string Language { get; set; }
        //The relevant Matrix Code
        public string MatrixCode { get; set; }
        //The relevant Matrix Id
        public int MatrixId { get; set; }
        //The Notes field
        public ICollection<string> Notes { get; set; }
        //The publishing source
        public string Source { get; set; }
        //The title of the dataset
        public string Title { get; set; }
        //To flag whether or not time values have been defined
        public bool TimeValsDefined { get; set; }
        //Time Values
        public ICollection<KeyValuePair<string, ICollection<string>>> TimeVals { get; set; }
        //General values
        public ICollection<KeyValuePair<string, ICollection<string>>> Values { get; set; }
        //Main values
        public ICollection<KeyValuePair<string, ICollection<string>>> MainValues { get; internal set; }
        //Notes as string
        public string NotesAsString { get; set; }
        //Validation errors
        public List<ValidationFailure> ValidationErrors { get; set; }
        //Time values defined flag
        public bool TimeValsIsDefined
        {
            get
            {
                return TimeVals != null && TimeVals.Count > 0;
            }
        }
        //Period codes
        public ICollection<KeyValuePair<string, ICollection<string>>> PeriodCodes
        {
            get
            {
                return TimeValsIsDefined ? TimeVals : Values;
            }
        }

        internal bool requiresResponse { get; set; }

        public int GetCellCount()
        {
            int counter = 1;
            foreach (var dim in this.Dimensions)
            {
                counter *= dim.Variables.Count;
            }
            return counter;
        }


        public string GetNotesAsString()
        {
            string output = "";
            if (this.Notes != null)
            {
                int counter = 0;
                foreach (string note in Notes)
                {
                    output = output + note;
                    counter++;
                    if (counter < Notes.Count) output = output + Environment.NewLine;
                }
            }
            return output;
        }
        //The upload DTO
        private IUpload_DTO UploadDto { get; set; }

        private int Sequence = 1;

        /// <summary>
        /// Constructor based on a px document and domain
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="domain"></param>
        public Dspec(IDocument doc, ICollection<KeyValuePair<string, ICollection<string>>> domain, IUpload_DTO uploadDto, IMetaData metaData)
        {
            this.UploadDto = uploadDto;
            SetMultipleLanguagesPropertiesFromPxDocument(doc, ConvertFactory.Convert(domain), null, metaData);
        }

        /// <summary>
        /// Constructor based on px document, domain and language code
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="domain"></param>
        /// <param name="otherlanguage"></param>
        public Dspec(IDocument doc, ICollection<KeyValuePair<string, ICollection<string>>> domain, string otherlanguage, IUpload_DTO uploadDto, IMetaData metaData)
        {
            this.UploadDto = uploadDto;
            SetMultipleLanguagesPropertiesFromPxDocument(doc, ConvertFactory.Convert(domain), otherlanguage, metaData);
        }


        public Dspec()
        {
        }

        public Dspec(IUpload_DTO uploadDto)
        {
            this.UploadDto = uploadDto;
        }

        public Dictionary<int,int> GetPrecisionOrdinals(IDmatrix matrix)
        {
            Dictionary<int, int> precisionOrdinals = new Dictionary<int, int>();
            var statDim =this.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault();
            if (statDim == null) return precisionOrdinals;
            //Create a query spec to return the ordinals corresponding to the precision variables in the stats dimension
            IDspec qspec = new Dspec() { Dimensions = new List<StatDimension>() };
            qspec.Dimensions.Add(new StatDimension()
            {
                Lambda = statDim.Lambda,
                Sequence = statDim.Sequence,
                Code = statDim.Code,
                Value = statDim.Value,
                Role = statDim.Role,
                Variables = new List<IDimensionVariable>()
            });
            
            foreach (var vrb in statDim.Variables)
            {
                if (vrb.Decimals > 0)
                {
                    qspec.Dimensions.ToList()[0].Variables = new List<IDimensionVariable>();
                    qspec.Dimensions.ToList()[0].Variables.Add(new DimensionVariable() { Code = vrb.Code, Value = vrb.Value, Sequence = vrb.Sequence, Decimals = vrb.Decimals });
                    //If precision is specified, then because each statistic variable can have a different precision, 
                    //we must get a list of cells ordinals that apply to each precision.
                    //We do this by running a fractal query for each specified precision

                    var precisionList = (GetOrdinalsForPrecision(matrix, qspec, this.Language ));
                    foreach (var i in precisionList)
                    {
                        if (!precisionOrdinals.ContainsKey(i.Key))
                            precisionOrdinals.Add(i.Key, i.Value);
                    }
                }
            }
            return precisionOrdinals;
        }

        //For each cell - what stat variable pertains to it and what precision should it have?
        public Dictionary<int, int> GetOrdinalsForPrecision(IDmatrix matrix, IDspec qspec, string lngIsoCode)
        {
            DataReader dr = new DataReader();
            Dictionary<int, int> precisionOrdinals = new Dictionary<int, int>();
            List<int> foundOrdinals = dr.RunFractalQueryOrdinalsOnly(matrix, qspec, lngIsoCode);
            int places = qspec.Dimensions.ToList()[0].Variables[0].Decimals;
            foreach (int i in foundOrdinals)
            {
                precisionOrdinals.Add(i, places);
            }
            return precisionOrdinals;
        }

        private void SetMapErrors(string url, string language)
        {
            string code = "";
            List<string> pList = url.Split('/').ToList<string>();
            if (pList.Count > 0) code = pList.Last().Replace("\"", String.Empty);
            else
            {
                if (this.ValidationErrors == null)
                    this.ValidationErrors = new List<ValidationFailure>();
                this.ValidationErrors.Add(new ValidationFailure("MapError", String.Format(Label.Get("px.maps.invalid-map-format", language), url)));

            }
            using (GeoMap_BSO gBso = new GeoMap_BSO())
            {
                if (!gBso.Read(code).hasData)
                {
                    if (this.ValidationErrors == null)
                        this.ValidationErrors = new List<ValidationFailure>();
                    this.ValidationErrors.Add(new ValidationFailure("MapError", String.Format(Label.Get("px.maps.map-not-found", language), url)));

                }
            }
        }

        private void ValidateMappingData(IList<KeyValuePair<string, IList<IPxSingleElement>>> maps, string language)
        {
            if (language == null) language = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
            foreach (var map in maps)
            {
                foreach (var val in map.Value)
                {
                    SetMapErrors(val.ToString(), language);
                }
            }
            //Ensure the same classfication is not referenced for maps more than once in the px file
            if (maps.Select(t => new { a = t.Key }).ToList().GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList().Count > 0)
            {
                if (this.ValidationErrors == null)
                    this.ValidationErrors = new List<ValidationFailure>();
                this.ValidationErrors.Add(new ValidationFailure("MapError", String.Format(Label.Get("error.geomap.inconsistent-map-data", language), "")));
            }
        }

        /// <summary>
        /// Get Statistic dimension from PxDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="language"></param>
        /// <param name="codes"></param>
        /// <param name="units"></param>
        /// <param name="precisions"></param>
        /// <returns></returns>
        private StatDimension GetStatisticalDimension(IDocument doc, string language, IList<KeyValuePair<string, IList<IPxSingleElement>>> codes, IList<KeyValuePair<string, IList<IPxSingleElement>>> units, IList<KeyValuePair<string, IList<IPxSingleElement>>> precisions, IMetaData metaData)
        {
            var statDimension = new StatDimension();
            statDimension.Role = Constants.C_DATA_DIMENSION_ROLE_STATISTIC;
            statDimension.Code = metaData.GetCsvStatistic();
            statDimension.Sequence = Sequence;
            Sequence++;

            if (!String.IsNullOrEmpty(ContentVariable))
            {
                statDimension.Value = ContentVariable;
                var statisticalIndicatorValues = Values.FirstOrDefault(i => i.Key == ContentVariable).Value;
                var statisticalIndicatorCodes = codes.FirstOrDefault(i => i.Key == ContentVariable).Value;

                int index = 0;
                foreach (var value in statisticalIndicatorValues)
                {
                    var c = statisticalIndicatorCodes.ElementAt(index).SingleValue;
                    string aUnit = units.FirstOrDefault(i => i.Key == value).Value.First().SingleValue;
                    DimensionVariable d = new DimensionVariable();
                    d.Value = value;
                    d.Code = c;
                    d.Unit = aUnit;
                    d.Decimals = Decimals;
                    d.Sequence = index + 1;


                    if (precisions != null)
                    {
                        KeyValuePair<string, IList<IPxSingleElement>> precisionValue = precisions.FirstOrDefault(i => i.Key == d.Value);

                        if (precisionValue.Key != null && !String.IsNullOrEmpty(precisionValue.Key))
                        {
                            d.Decimals = (short)precisionValue.Value.FirstOrDefault().SingleValue;

                        }
                    }
                    statDimension.Variables.Add(d);
                    ++index;
                }
            }
            else
            {

                int sequence = 1;
                // if there is only one statistical dimension we have no subkeys
                var unit = doc.GetStringElementValue("UNITS", language);
                statDimension.Value = Label.Get("default.statistic");// Contents;
                statDimension.Code = metaData.GetCsvStatistic(); //this.MatrixCode;
                DimensionVariable dimensionVariable = new DimensionVariable();
                dimensionVariable.Code = this.MatrixCode;
                dimensionVariable.Value = Contents;
                dimensionVariable.Sequence = sequence;
                dimensionVariable.Unit = unit;
                dimensionVariable.Decimals = Decimals;

                if (precisions != null)
                {
                    KeyValuePair<string, IList<IPxSingleElement>> precisionValue = precisions.FirstOrDefault(i => i.Key == Contents);

                    if (precisionValue.Key != null && !String.IsNullOrEmpty(precisionValue.Key))
                    {
                        dimensionVariable.Decimals = (short)precisionValue.Value.FirstOrDefault().SingleValue;
                    }
                }
                statDimension.Variables.Add((dimensionVariable));
            }
            return statDimension;
        }

        /// <summary>
        /// Translates a [VALUES] item in px from one language version to another
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="value"></param>
        /// <param name="lngIsoCodeFrom"></param>
        /// <param name="lngIsoCodeTo"></param>
        /// <returns></returns>
        private string TranslateValue(IDocument doc, string value, string lngIsoCodeFrom, string lngIsoCodeTo, bool isMain = false)
        {
            //Translation request is trivial, we don't need to translate
            if (lngIsoCodeTo == null) return null;

            //Get a translation
            var mainValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatchesNullable("VALUES", lngIsoCodeFrom);


            //No translation, get the version from the top language
            if (mainValues == null) mainValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatchesNullable("VALUES", null);



            if (mainValues == null) return null;


            int thisPos = mainValues.ToList().FindIndex(x => x.Key == value);

            if (thisPos < 0) return value;

            IList<KeyValuePair<string, IList<IPxSingleElement>>> otherValues;
            if (isMain)
            {
                otherValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageIsEmpty("VALUES");//, lngIsoCodeTo);
            }
            else
                otherValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatches("VALUES", lngIsoCodeTo);

            if (otherValues == null) return value;

            var otherVal = otherValues.ElementAt(thisPos);


            return otherVal.Key == null ? value : otherVal.Key;
        }

        /// <summary>
        /// Get the frequency dimension
        /// </summary>
        /// <param name="headings"></param>
        /// <param name="stubs"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        private StatDimension GetFrequencyDimension(IList<IPxSingleElement> headings, IList<IPxSingleElement> stubs, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain)
        {
            // typically the period is defined in the headings, but can also be in stubs
            // when this happens we should have a timeval to helps us make the right call
            // therefore we now check timeval first

            string frequencyValue = null;
            if (TimeValsIsDefined)
            {
                frequencyValue = PeriodCodes.ElementAt(0).Key;
            }
            else
            {
                var periodCode = PeriodCodes.Where(x => x.Key == UploadDto.FrqValueTimeval);
                if (periodCode.Any())
                {
                    frequencyValue = UploadDto.FrqValueTimeval;
                }
            }
            // if (frequencyValue == null) return null;

            string aFrequencyCode;

            // if time vals exist use them for the code, otherwise make the period code and value the same
            // also make the frequency code and value the same
            if (TimeValsIsDefined)
            {
                aFrequencyCode = PeriodCodes.FirstOrDefault(p => p.Key == frequencyValue).Value.First();
            }
            else
            {
                if (UploadDto?.FrqCodeTimeval != null) aFrequencyCode = UploadDto.FrqCodeTimeval;
                else
                    aFrequencyCode = frequencyValue;
            }

            StatDimension statDimension = new StatDimension();
            statDimension.Role = Constants.C_DATA_DIMENSION_ROLE_TIME;
            statDimension.Code = aFrequencyCode;
            statDimension.Value = frequencyValue;

            // that use the corresponding values that are left to get from values the correspondent period values
            IEnumerable<IEnumerable<string>> periodValues =
                from c in Values
                where c.Key == frequencyValue
                select c.Value;

            var pcount = periodValues.Count();
            //We can't ascertain the time dimension and we'll have to return to the user for further information
            if (pcount == 0) return null;
            IEnumerable<string> thePeriodValues = periodValues.First();
            IEnumerable<string> thePeriodCodes;

            if (TimeValsIsDefined)
            {
                thePeriodCodes = PeriodCodes.FirstOrDefault(p => p.Key == frequencyValue).Value;
            }
            else
            {
                thePeriodCodes = thePeriodValues;
            }

            int i = 0;
            foreach (var v in thePeriodValues)
            {
                var test = v.Replace("\"", String.Empty);
                var c = thePeriodCodes.ElementAt(i + (TimeValsIsDefined ? 1 : 0));
                c = c.Replace("\"", String.Empty);
                DimensionVariable d = new DimensionVariable();
                d.Code = c;
                d.Value = test;
                d.Sequence = i + 1;
                statDimension.Variables.Add(d);
                ++i;
            }
            statDimension.Sequence = Sequence;
            Sequence++;
            return statDimension;
        }

        private StatDimension GetFrequencyDimension(IDocument doc, string language, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, string proposedLanguage = null)
        {
            StatDimension statDimension = new StatDimension();
            bool isMain = false;
            if (Language == null && proposedLanguage != null)
            {
                Language = proposedLanguage;
                isMain = true;
            }
            //Here we must get a translated version of FrqValueTimeval. FrqValueTimeval was supplied in one language,
            //but we must get the corresponding version for this Specification's language
            statDimension.Value = TranslateValue(doc, UploadDto.FrqValueTimeval, UploadDto.LngIsoCode, Language, isMain);
            statDimension.Value = statDimension.Value == null ? UploadDto.FrqValueTimeval : statDimension.Value;

            //we need to take this from a translated version of FrqValue rather than the supplied one.
            IEnumerable<IEnumerable<string>> periodValues =
               from c in Values
               select c.Value;

            //Return a null at this stage if we can't create a frquency
            if (periodValues.Count() == 0)
            {
                return null;
            }

            var periods = periodValues.ElementAt(0);
            int sequence = 1;
            foreach (var v in periods)
            {
                DimensionVariable d = new DimensionVariable();
                d.Code = v;
                d.Value = v;
                d.Sequence = sequence;
                statDimension.Variables.Add(d);
                sequence++;
            }
            statDimension.Code = UploadDto.FrqCodeTimeval;
            return statDimension;
        }

        /// <summary>
        /// Find all classifications and associated variables
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="codes"></param>
        /// <param name="result"></param>
        /// <param name="dimensionValues"></param>
        /// <param name="maps"></param>
        private void LoopClassifications(IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, IList<KeyValuePair<string, IList<IPxSingleElement>>> codes, ICollection<StatDimension> result, IEnumerable<string> dimensionValues, IList<KeyValuePair<string, IList<IPxSingleElement>>> maps)
        {
            //We need this to ensure classification codes do not coincidentally end up calculating the same code:
            int dimensionCounter = 0;

            foreach (var value in dimensionValues)
            {
                // if we do not have Domain, code is same as dimension, can also be an iterator...
                string code = "";

                if (domain != null && domain.Any())
                {
                    IEnumerable<IEnumerable<IPxSingleElement>> dimensionCodes =
                                          from v in domain
                                          where v.Key == value
                                          select v.Value;

                    if (dimensionCodes.Any())
                    {
                        code = dimensionCodes.First().First().SingleValue;
                    }

                }
                var statDimension = new StatDimension();
                statDimension.Role = Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION;
                statDimension.Code = code.Replace("\"", String.Empty);
                statDimension.Value = value;
                if (maps != null && maps.Any())
                {
                    var mapLines = maps.Where(m => m.Key == value).Select(m => m.Value).FirstOrDefault();
                    statDimension.GeoUrl = "";
                    if (mapLines != null && mapLines.Any())
                    {
                        statDimension.GeoUrl = string.Join("", mapLines.Select(e => e.ToPxValue()));
                        statDimension.GeoFlag = true;
                    }
                }
                statDimension.Variables = new List<IDimensionVariable>();

                //  create variables
                var variableValues = Values.FirstOrDefault(v => v.Key == value).Value;
                IList<IPxSingleElement> variableCodes = null;
                if (codes != null)
                {
                    variableCodes = codes.FirstOrDefault(v => v.Key == value).Value;
                }

                int i = 0;
                foreach (var variableValue in variableValues)
                {
                    var test = variableValue.Replace("\"", String.Empty);
                    var dimensionVariable = new DimensionVariable();
                    // if codes do not exist we assign default progressive number
                    dynamic variableCode;
                    if (variableCodes != null)
                    {
                        var t = variableCodes.ElementAt(i).ToString().Replace("\"", String.Empty);
                        dimensionVariable.Code = t;
                        variableCode = variableCodes.ElementAt(i).SingleValue;
                    }
                    else
                    {
                        dimensionVariable.Code = i.ToString();
                        variableCode = i.ToString();
                    }
                    dimensionVariable.Value = test;
                    dimensionVariable.Sequence = i + 1;
                    statDimension.Variables.Add(dimensionVariable);
                    ++i;
                }
                //If there's no code, get one based on the hash of the classification plus its variable
                if (code.Length == 0)
                {
                    dimensionCounter++;
                    int hash = statDimension.GetHashCode();
                    foreach (var vrb in statDimension.Variables)
                        hash = hash ^ vrb.Code.GetHashCode();
                    statDimension.Code = Utility.GetMD5(hash.ToString()) + dimensionCounter.ToString();
                }
                statDimension.Sequence = Sequence;
                Sequence++;
                result.Add(statDimension);
            }
        }

        /// <summary>
        /// Create blank classification
        /// </summary>
        /// <returns></returns>
        private ICollection<StatDimension> MockClassification()
        {
            ICollection<StatDimension> result = new List<StatDimension>();
            StatDimension statDimension = new StatDimension();
            statDimension.Code = "0";
            statDimension.Value = Label.Get("default.classification");
            statDimension.Role = Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION;
            statDimension.Sequence = Sequence;
            Sequence++;
            DimensionVariable dimensionVariable = new DimensionVariable();
            dimensionVariable.Code = "0";
            dimensionVariable.Value = Label.Get("default.variable");
            dimensionVariable.Sequence = 1;
            statDimension.Variables.Add(dimensionVariable);
            result.Add(statDimension);
            return result;
        }

        /// <summary>
        /// Get classification dimensions
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="codes"></param>
        /// <param name="stubs"></param>
        /// <param name="headings"></param>
        /// <param name="contents"></param>
        /// <param name="maps"></param>
        /// <returns></returns>
        internal ICollection<StatDimension> GetClassificationDimensions(IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, IList<KeyValuePair<string, IList<IPxSingleElement>>> codes,
            IList<IPxSingleElement> stubs, IList<IPxSingleElement> headings, string contents, IList<KeyValuePair<string, IList<IPxSingleElement>>> maps, StatDimension frequencyDimension)
        {
            ICollection<StatDimension> result = new List<StatDimension>();

            // In case the time is in the stubs we exclude it in the where clause to avoid having time duplicated!
            IEnumerable<string> dimensionInStubs = Enumerable.Empty<string>();
            IEnumerable<string> dimensionsInHeadings = Enumerable.Empty<string>();

            if (stubs != null && frequencyDimension != null)
            {
                dimensionInStubs =
                from v in stubs
                where v.SingleValue != frequencyDimension.Value
                select (string)v.SingleValue;
            }

            if (headings != null && frequencyDimension != null)
            {
                dimensionsInHeadings =
                                    from v in headings
                                    where v.SingleValue != frequencyDimension.Value
                                    select (string)v.SingleValue;
            }

            // remove Statistics 
            List<string> dimensionValues = dimensionInStubs.Concat(dimensionsInHeadings).ToList<string>();
            HashSet<string> statisticValues = new HashSet<string>(Dimensions.Where(d => d.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).Select(x => x.Value));

            //The next line had the effect of restricting dimensions where a statistic had a similar name
            //This is being removed pending testing - Issue #180
            //dimensionValues.RemoveAll(v => statisticValues.Contains(v));

            dimensionValues.RemoveAll(v => v == ContentVariable);// || v == contents);


            LoopClassifications(domain, codes, result, dimensionValues, maps);

            // if we have no dimensions we mock up a default dimension
            if (result.Count == 0)
            {
                return MockClassification();
            }

            return result;
        }

        internal void SetEliminationsByValue(ref ICollection<StatDimension> classification, Dictionary<string, string> eliminations)
        {
            //Update the variables with Elimination data
            foreach (var elim in eliminations)
            {
                var cls = classification.Where(x => x.Value == elim.Key).FirstOrDefault();
                if (cls != null)
                {
                    if (eliminations[cls.Value] != null)
                    {
                        var vrb = cls.Variables.Where(x => x.Value == eliminations[cls.Value]).FirstOrDefault();
                        if (vrb != null)
                            vrb.Elimination = true;
                    }
                    else
                    {
                        foreach (var v in cls.Variables)
                            v.Elimination = false;
                    }
                }
            }
        }

        internal void SetEliminationsByCode(ref ICollection<StatDimension> classification, Dictionary<string, string> eliminations)
        {
            //Update the variables with Elimination data
            foreach (var elim in eliminations)
            {
                var cls = classification.Where(x => x.Code == elim.Key).FirstOrDefault();
                if (cls != null)
                {
                    foreach (var v in cls.Variables)
                        v.Elimination = false;

                    if (eliminations[cls.Code] != null)
                    {
                        var vrb = cls.Variables.Where(x => x.Code == eliminations[cls.Code]).FirstOrDefault();
                        if (vrb != null)
                            vrb.Elimination = true;
                    }

                }
            }
        }

        /// <summary>
        /// Set properties for multiple languages
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="domain"></param>
        /// <param name="language" - this is the language of the Specification></param>
        private void SetMultipleLanguagesPropertiesFromPxDocument(IDocument doc, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, string language, IMetaData metaData)
        {
            string proposedLanguage = doc.GetStringValueIfExist("LANGUAGE");
            if (!String.IsNullOrEmpty(language))
            {
                Language = language;
            }

            // Title for main language and Title[by other language] and same with all the others.
            Title = doc.GetStringElementValue("CONTENTS", language);
            var docValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatches("VALUES", language);
            Values = ConvertFactory.Convert(docValues);
            MatrixCode = doc.GetStringElementValue("MATRIX");
            MainValues = ConvertFactory.Convert(doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatches("VALUES", language));
            Contents = doc.GetStringElementValue("CONTENTS", language);
            string infofile = doc.GetStringValueIfExist("INFOFILE", language);
            var docTimeVals = doc.GetMultiValuesWithSubkeysIfExist("TIMEVAL", language);
            if (docTimeVals != null)
            {
                TimeVals = ConvertFactory.Convert(docTimeVals);
            }
            Source = doc.GetStringElementValue("SOURCE", language);

            //DECIMALS is specific to Matrix and is language invariant but the value is needed in each language specification and the main spec
            Decimals = doc.GetShortElementValue("DECIMALS");
            if (Decimals == -1) Decimals = doc.GetShortElementValue("DECIMAL");

            //Contatenate Notes, Notex and Infofile
            List<string> notex = null;
            var notexCheck = doc.GetListOfStringValuesIfExist("NOTEX", language);
            if (notexCheck != null)
            {
                notex = notexCheck.ToList();
            }
            Notes = doc.GetListOfStringValuesIfExist("NOTE", language);

            // Concatenate notes into one string
            if (Notes != null)
            {
                NotesAsString = string.Join("", Notes.Select(e => e.ToString()));
                if (NotesAsString.Equals("\"\""))
                {
                    Notes = null;
                    NotesAsString = null;
                }

            }
            else
            {
                Notes = new List<string>();
                NotesAsString = "";
            }

            //Get rid of multiple spaces
            //NotesAsString = Regex.Replace(NotesAsString, @"\s+", " ");
            if (notex != null)
                NotesAsString = NotesAsString + string.Join("", notex.Select(e => e.ToString()));

            if (Uri.TryCreate(infofile, UriKind.Absolute, out Uri uri))
            {
                NotesAsString = NotesAsString.Trim() + " " + infofile;
            }
            var maps = doc.GetSingleElementWithSubkeysIfExist("MAP", language);
            if (maps != null) ValidateMappingData(maps, language);
            var codes = doc.GetMultiValuesWithSubkeysIfExist("CODES", language);
            var precisions = doc.GetSingleElementWithSubkeysIfExist("PRECISION", language);
            var units = doc.GetSingleElementWithSubkeys("UNITS", language);
            ContentVariable = doc.GetStringValueIfExist("CONTVARIABLE", language);
            var stubs = doc.GetListOfElementsIfExist("STUB", language);
            var headings = doc.GetListOfElementsIfExist("HEADING", language);

            // get the Statistical Products, Time and Dimensions
            var statDimension = GetStatisticalDimension(doc, language, codes, units, precisions, metaData);

            // statDimension.Code = statDimension.Code != null ? statDimension.Code : metaData.GetCsvStatistic();
            Dimensions.Add(statDimension);

            // If Timevals is null and language is not null then update PxUploadDTO.FrqValueTimeval
            // to reflect value for the language
            if (TimeVals == null && language != null)
            {
                var heading = headings.FirstOrDefault().ToString();
                if (heading != null)
                {
                    UploadDto.FrqValueTimeval = heading;
                }
            }

            //otherwise we'll atempt to fill time values from the FrqValue DTO item (user will choose it)
            //  if (PxUploadDto == null
            var frequencyDimension = GetFrequencyDimension(headings, stubs, domain);
            if (frequencyDimension != null)
            {
                Dimensions.Add(frequencyDimension);
            }

            //We may have to change things regarding the Frequency. This only happens for the build.
            if (UploadDto != null)
            {

                if (frequencyDimension != null)
                {
                    //We have a frequency and if we also have a FrqCode from the user, we'll change the FrqCode value
                    if (UploadDto.FrqCodeTimeval != null) frequencyDimension.Code = UploadDto.FrqCodeTimeval;
                }
                else//No Timeval was defined in the px file and we got no Frquency so we try to either ascertain it from the FrqCode or send back to the user for clarification
                {
                    //Not enough information, so we must ask the user
                    if (UploadDto.FrqValueTimeval == null) this.requiresResponse = true;
                    else //We might be able to get the Frequency from the FrqValue
                    {
                        proposedLanguage = Language ?? proposedLanguage;

                        frequencyDimension = GetFrequencyDimension(doc, UploadDto.LngIsoCode, domain, proposedLanguage);
                        if (frequencyDimension != null)
                        {
                            Dimensions.Add(frequencyDimension);
                        }

                        Frequency_BSO fBso = new Frequency_BSO();
                        if (!fBso.ReadAll().Contains(UploadDto.FrqCodeTimeval)) frequencyDimension = null;
                        //No joy, so we must go back to the user.
                        if (frequencyDimension == null) this.requiresResponse = true;
                    }
                }
            }

            var classificationDimensions = GetClassificationDimensions(domain, codes, stubs, headings, Contents, maps, frequencyDimension);
            foreach (var dimension in classificationDimensions)
            {
                Dimensions.Add(dimension);
            }

            Dictionary<string, string> dictElim = new Dictionary<string, string>();
            foreach (var elim in doc.GetManySingleValuesWithSubkeysOnlyIfLanguageMatches("ELIMINATION", language))
            {
                dictElim.Add(elim.Key, elim.Value.ToPxValue());
            }
            var classification = (ICollection<StatDimension>)Dimensions.Where(s => s.Role == Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION).ToList();
            SetEliminationsByValue(ref classification, dictElim);

            if (String.IsNullOrEmpty(ContentVariable))
            {
                ContentVariable = Label.Get("default.statistic");
            }


            //Sequences are based on heading and stub values
            Dimensions=AllocateSequences(headings, stubs, Dimensions);

        }

        //Set the sequences based on position of the dimension in Values
        private void AllocateSequences(ICollection<KeyValuePair<string, ICollection<string>>> values)
        {
            int sequence = 0;
            //The stats dimension may or may not be in Values. If not then give it a sequence of 1
            var statisticsDimension = this.Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC)).FirstOrDefault();
            var statisticsValue = values.Where(x => x.Key.Equals(statisticsDimension.Value)).FirstOrDefault();
            if (statisticsValue.Equals(new KeyValuePair<string, ICollection<string>>()))
            {

                statisticsDimension.Sequence = ++sequence;
            }

            //Iterate through Values and assign the dimension sequences as they are found
            foreach (var item in values)
            {
                var nextDimension = Dimensions.Where(x => x.Value.Equals(item.Key)).FirstOrDefault();
                nextDimension.Sequence = ++sequence;
            }

            //Sort by dimension to ensure the data is stored in the same order as the sequence
            Dimensions = Dimensions.OrderBy(x => x.Sequence).ToList();
        }

        //Set the sequences based on the heading and stub
        private ICollection<StatDimension> AllocateSequences(IList<IPxSingleElement> heading, IList<IPxSingleElement> stub, ICollection<StatDimension> dimensions)
        {
            List<string> dimCodes = new List<string>();
            List<string> dimValues = new List<string>();
            int sequence = 1;

            //If there's an implied statistic dimension, it gets a sequence of 1
            var dimStatistic = dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC)).FirstOrDefault();
            if (dimStatistic.Variables.Count == 1 )
            {
                List<string> stubsList = stub.Select(x => x.ToPxValue()).ToList();
                List<string>headingsList=heading.Select(x=>x.ToPxValue()).ToList();
                if (!stubsList.Contains(dimStatistic.Value) && !headingsList.Contains(dimStatistic.Value))
                {
                    dimStatistic.Sequence = sequence;
                    dimCodes.Add(dimStatistic.Code);
                    dimValues.Add(dimStatistic.Value);
                }
            }


            foreach (var s in stub)
            {
                var nextDim=dimensions.Where(x => x.Value.Equals(s.ToPxValue())).FirstOrDefault();
                nextDim.Sequence = ++sequence;
                dimCodes.Add(nextDim.Code);
                dimValues.Add(nextDim.Value);
            }

            foreach (var h in heading)
            {
                var nextDim = dimensions.Where(x => x.Value.Equals(h.ToPxValue())).FirstOrDefault();
                nextDim.Sequence = ++sequence;
                dimCodes.Add(nextDim.Code);
                dimValues.Add(nextDim.Value);
            }

            

            //check for dupes in heading/stub
            if (dimCodes.GroupBy(x => x).Any(g => g.Count() > 1))
            {
                //duplicate found!
                throw new Exception("Duplicate value in heading/stub");
            }

            //check for dimensions not in heading/stub
            var notIn = dimensions.Where(x => !dimValues.Any(y => y ==x.Value));
            if(notIn.Any())
            {
                foreach (var dim in notIn)
                {
                    //We make an exception for implied an statistic dimension
                    if(!(dim.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC) && this.ContentVariable==null))
                    //dimension not in heading or stub
                    throw new Exception("Dimension value not found either in the heading or stub");
                }
            }

            return dimensions;
        }




        public Dictionary<string, string> GetEliminationObject()
        {
            Dictionary<string, string> elimList = new Dictionary<string, string>();
            foreach (var cls in this.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION))
            {
                var vrbElim = cls.Variables.Where(x => x.Elimination).FirstOrDefault();
                if (vrbElim != null)
                {
                    elimList.Add(cls.Code, vrbElim.Code);
                }
                else
                {
                    elimList.Add(cls.Code, null);
                }
            }
            return elimList;
        }

        /// <summary>
        /// Get the list of dimensions with speeded up access (uses dictionaries)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetDimensionsAsDictionary()
        {
            Dictionary<string, object> dimensions = new Dictionary<string, object>();
            foreach (var dim in this.Dimensions)
            {
                dim.DictionaryVariables = new Dictionary<string, object>();
                foreach (var vrb in dim.Variables)
                {

                    dim.DictionaryVariables.Add(vrb.Code, vrb);

                }
                dimensions.Add(dim.Code, dim);
            }
            return dimensions;
        }


        public Dspec GetDspecFromPxDocument(IDocument doc, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, string language, IMetaData metaData)
        {
            Dspec dspec = new Dspec(this.UploadDto);
            string proposedLanguage = doc.GetStringValueIfExist("LANGUAGE");
            if (!String.IsNullOrEmpty(language))
            {
                dspec.Language = language;
            }

            // Title for main language and Title[by other language] and same with all the others.
            dspec.Title = doc.GetStringElementValue("CONTENTS", language);
            var docValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatches("VALUES", language);
            dspec.Values = ConvertFactory.Convert(docValues);
            dspec.MatrixCode = doc.GetStringElementValue("MATRIX");
            dspec.MainValues = ConvertFactory.Convert(doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatches("VALUES", language));
            dspec.Contents = doc.GetStringElementValue("CONTENTS", language);
            string infofile = doc.GetStringValueIfExist("INFOFILE", language);
            var docTimeVals = doc.GetMultiValuesWithSubkeysIfExist("TIMEVAL", language);
            if (docTimeVals != null)
            {
                dspec.TimeVals = ConvertFactory.Convert(docTimeVals);
            }
            dspec.Source = doc.GetStringElementValue("SOURCE", language);

            //DECIMALS is specific to Matrix and is language invariant but the value is needed in each language specification and the main spec
            dspec.Decimals = doc.GetShortElementValue("DECIMALS");
            if (dspec.Decimals == -1) dspec.Decimals = doc.GetShortElementValue("DECIMAL");

            //Contatenate Notes, Notex and Infofile
            List<string> notex = null;
            var notexCheck = doc.GetListOfStringValuesIfExist("NOTEX", language);
            if (notexCheck != null)
            {
                notex = notexCheck.ToList();
            }
            dspec.Notes = doc.GetListOfStringValuesIfExist("NOTE", language);


            // Concatenate notes into one string
            if (dspec.Notes != null)
            {
                dspec.NotesAsString = string.Join("", dspec.Notes.Select(e => e.ToString()));
                if (dspec.NotesAsString.Equals("\"\""))
                {
                    dspec.Notes = null;
                    dspec.NotesAsString = null;
                }

            }
            else
            {
                dspec.Notes = new List<string>();
                dspec.NotesAsString = "";
            }

            //Get rid of multiple spaces
            //NotesAsString = Regex.Replace(NotesAsString, @"\s+", " ");
            if (notex != null)
            {
                if (dspec.Notes == null) dspec.Notes = new List<string>();
                dspec.NotesAsString = dspec.NotesAsString + " " + string.Join(" ", notex.Select(e => e.ToString()));
                foreach (var n in notex)
                    dspec.Notes.Add(n);
            }

            if (Uri.TryCreate(infofile, UriKind.Absolute, out Uri uri))
            {
                dspec.NotesAsString = dspec.NotesAsString.Trim() + " " + infofile;
            }
            var maps = doc.GetSingleElementWithSubkeysIfExist("MAP", language);
            if (maps != null) ValidateMappingData(maps, language);
            var codes = doc.GetMultiValuesWithSubkeysIfExist("CODES", language);
            var precisions = doc.GetSingleElementWithSubkeysIfExist("PRECISION", language);
                       
            var units = doc.GetSingleElementWithSubkeys("UNITS", language);
            dspec.ContentVariable = doc.GetStringValueIfExist("CONTVARIABLE", language);
            this.ContentVariable = dspec.ContentVariable;
            var stubs = doc.GetListOfElementsIfExist("STUB", language);
            var headings = doc.GetListOfElementsIfExist("HEADING", language);


            // get the Statistical Products, Time and Dimensions
            var statDimension = dspec.GetStatisticalDimension(doc, language, codes, units, precisions, metaData);

            statDimension.Code = statDimension.Code != null ? statDimension.Code : metaData.GetCsvStatistic();
            if (ContentVariable != null)
            {
                statDimension.Value = ContentVariable;
                statDimension.Code = metaData.GetCsvStatistic();
            }
            dspec.Dimensions.Add(statDimension);

            // If Timevals is null and language is not null then update PxUploadDTO.FrqValueTimeval
            // to reflect value for the language
            if (dspec.TimeVals == null && language != null)
            {
                var heading = headings.FirstOrDefault().ToString();
                if (heading != null)
                {
                    dspec.UploadDto.FrqValueTimeval = heading;
                }
            }

            //otherwise we'll atempt to fill time values from the FrqValue DTO item (user will choose it)
            //  if (PxUploadDto == null
            var frequencyDimension = dspec.GetFrequencyDimension(headings, stubs, domain);
            if (frequencyDimension != null)
            {
                dspec.Dimensions.Add(frequencyDimension);
            }

            //We may have to change things regarding the Frequency. This only happens for the build.
            if (dspec.UploadDto != null)
            {

                if (frequencyDimension != null)
                {
                    //We have a frequency and if we also have a FrqCode from the user, we'll change the FrqCode value
                    if (dspec.UploadDto.FrqCodeTimeval != null)
                    {
                        frequencyDimension.Code = dspec.UploadDto.FrqCodeTimeval;

                        
                    }
                }
                else//No Timeval was defined in the px file and we got no Frquency so we try to either ascertain it from the FrqCode or send back to the user for clarification
                {
                    //Not enough information, so we must ask the user
                    if (dspec.UploadDto.FrqValueTimeval == null) dspec.requiresResponse = true;
                    else //We might be able to get the Frequency from the FrqValue
                    {
                        proposedLanguage = dspec.Language ?? proposedLanguage;

                        frequencyDimension = GetFrequencyDimension(doc, dspec.UploadDto.LngIsoCode, domain, proposedLanguage);
                        if (frequencyDimension != null)
                        {
                            dspec.Dimensions.Add(frequencyDimension);
                        }

                        Frequency_BSO fBso = new Frequency_BSO();
                        if (!fBso.ReadAll().Contains(dspec.UploadDto.FrqCodeTimeval)) frequencyDimension = null;
                        //No joy, so we must go back to the user.
                        if (frequencyDimension == null) dspec.requiresResponse = true;
                    }
                }
            }

            var classificationDimensions = dspec.GetClassificationDimensions(domain, codes, stubs, headings, dspec.Contents, maps, frequencyDimension);
            foreach (var dimension in classificationDimensions)
            {
                dspec.Dimensions.Add(dimension);
            }


            Dictionary<string, string> dictElim = new Dictionary<string, string>();
            foreach (var elim in doc.GetManySingleValuesWithSubkeysOnlyIfLanguageMatches("ELIMINATION", language))
            {
                dictElim.Add(elim.Key, elim.Value.ToPxValue());
            }
            var classification = (ICollection<StatDimension>)dspec.Dimensions.Where(s => s.Role == Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION).ToList();
            SetEliminationsByValue(ref classification, dictElim);


            
            //Sequences need to be allocated based on the position of the dimension in stubs and headings
            if (!dspec.requiresResponse)
                dspec.Dimensions=AllocateSequences(headings, stubs,dspec.Dimensions);

            //dspec.AllocateSequences(dspec.Values);
            if (String.IsNullOrEmpty(dspec.ContentVariable))
            {
                dspec.ContentVariable = Label.Get("default.statistic");
            }


            //Allow for changes in the frequency value if this is build
            if (dspec.UploadDto != null)
            {
                if (frequencyDimension != null)
                {
                    if (dspec.UploadDto.FrqValueTimeval != null)
                    {
                        var timeDim = dspec.Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).FirstOrDefault();
                        timeDim.Value = dspec.UploadDto.FrqValueTimeval;
                    }
                }
            }
                return dspec;
        }

        public void ValidateMaps(bool formatValidationOnly = false)
        {
            foreach (var cls in this.Dimensions)
            {
                if (cls.GeoFlag)
                {
                    //validate that the map has a well formed url
                    if (!Regex.IsMatch(cls.GeoUrl, Utility.GetCustomConfig("APP_REGEX_URL")))
                    {
                        if (this.ValidationErrors == null) this.ValidationErrors = new List<ValidationFailure>();
                        this.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.invalid-format", this.Language)));
                        return;
                    }


                    //If we're not interested in the link between classifications and map entities, only validate that the map exists,
                    //then return with or without a list of validation errors
                    if (formatValidationOnly)
                    {
                        string code = "";
                        List<string> pList = cls.GeoUrl.Split('/').ToList<string>();
                        if (pList.Count > 0) code = pList.Last().Replace("\"", String.Empty);
                        using (GeoMap_BSO gBso = new GeoMap_BSO())
                        {
                            if (!gBso.Read(code).hasData)
                            {
                                this.ValidationErrors = new List<ValidationFailure>();
                                this.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.not-found", this.Language)));
                            }
                            return;
                        }
                    }

                    GeoJson geoJson = new GeoJson();
                    using (GeoMap_BSO gBso = new GeoMap_BSO(new ADO("defaultConnection")))
                    {
                        string geoCode = cls.GeoUrl;
                        //Accept only the rightmost 32 characters
                        if (cls.GeoUrl.Length > 32)
                        {
                            geoCode = cls.GeoUrl.Substring(cls.GeoUrl.Length - 32);
                            string baseUrl = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.api.static") + "/PxStat.Data.GeoMap_API.Read/";
                            if (!cls.GeoUrl.Substring(0, cls.GeoUrl.Length - 32).Equals(baseUrl))
                            {
                                if (this.ValidationErrors == null) this.ValidationErrors = new List<ValidationFailure>();
                                this.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.not-found", this.Language)));
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
                                if (this.ValidationErrors == null) this.ValidationErrors = new List<ValidationFailure>();
                                this.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.json-parse", this.Language)));
                                return;
                            }
                            foreach (var feature in geoJson.Features)
                            {
                                if (!feature.Properties.ContainsKey("code"))
                                {
                                    if (this.ValidationErrors == null) this.ValidationErrors = new List<ValidationFailure>();
                                    this.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.code-tag", this.Language)));
                                    return;
                                }
                            }
                            foreach (var vrb in cls.Variables)
                            {

                                if (!vrb.Elimination)

                                {
                                    if (geoJson.Features.Where(x => x.Properties["code"] == vrb.Code).Count() == 0)
                                    {
                                        if (this.ValidationErrors == null) this.ValidationErrors = new List<ValidationFailure>();
                                        this.ValidationErrors.Add(new ValidationFailure("GeoJson", String.Format(Label.Get("error.geomap.unmapped-variable", this.Language), vrb.Code)));
                                        return;

                                    }
                                }


                            }
                        }
                        else
                        {

                            if (this.ValidationErrors == null) this.ValidationErrors = new List<ValidationFailure>();
                            this.ValidationErrors.Add(new ValidationFailure("GeoJson", Label.Get("error.geomap.not-found", this.Language)));
                            return;

                        }
                    }
                }
            }
        }
    }
}
