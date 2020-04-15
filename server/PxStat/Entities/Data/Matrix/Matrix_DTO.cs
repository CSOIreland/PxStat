using API;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation.Results;
using Newtonsoft.Json;
using PxParser.Resources.Parser;
using PxStat.Build;
using PxStat.JsonStatSchema;
using PxStat.Resources;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PxStat.Data
{
    /// <summary>
    /// Link
    /// </summary>
    public partial class Link
    {
        [JsonProperty("enclosure", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Enclosure> Enclosure { get; set; }
        [JsonProperty("alternate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Alternate> Alternate { get; set; }
    }



    /// <summary>
    /// Enclosure
    /// </summary>
    public partial class Enclosure
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("href", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }
    }

    public partial class Alternate
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("href", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }
    }

    /// <summary>
    /// Matrix class
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// class property
        /// </summary>
        internal Release_DTO Release { get; set; }

        /// <summary>
        /// class propery
        /// </summary>
        private List<string> Reasons { get; set; }


        private PxUpload_DTO pxUploadDto;

        /// <summary>
        /// Used for serialization/deserialization
        /// </summary>
        public Matrix() { }

        /// <summary>
        /// class
        /// </summary>
        public class Filters
        {
            /// <summary>
            /// Cosntructor
            /// </summary>
            public Filters()
            {
                Metrics = new List<StatisticalRecordDTO_Create>();
                Times = new List<PeriodRecordDTO_Create>();
                Classifications = new List<ClassificationRecordDTO_Create>();
            }

            /// <summary>
            /// class property
            /// </summary>
            public List<StatisticalRecordDTO_Create> Metrics { get; set; }

            /// <summary>
            /// class property
            /// </summary>
            public List<PeriodRecordDTO_Create> Times { get; set; }

            /// <summary>
            /// class property
            /// </summary>
            public List<ClassificationRecordDTO_Create> Classifications { get; set; }
        }



        /// <summary>
        /// For a valid Matrix, get the specification for a specific language
        /// </summary>
        /// <param name="lngIsoCode"></param>
        internal Specification GetSpecFromLanguage(string lngIsoCode)
        {
            if (lngIsoCode != null && this.OtherLanguageSpec == null && this.MainSpec.Language != lngIsoCode)
                return null;

            Matrix.Specification theSpec;


            if (lngIsoCode == null) lngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");

            if (this.MainSpec != null)
            {
                if (this.MainSpec.Language == lngIsoCode)
                    return this.MainSpec;
                else
                {
                    theSpec = this.OtherLanguageSpec.Where(x => x.Language == lngIsoCode).FirstOrDefault();
                }
            }
            else
            {
                theSpec = this.OtherLanguageSpec.Where(x => x.Language == lngIsoCode).FirstOrDefault();

            }

            return theSpec;
        }

        /// <summary>
        /// Specification for a specific language
        /// </summary>
        public class Specification
        {


            private PxUpload_DTO PxUploadDto { get; set; }

            internal bool requiresResponse { get; set; }
            /// <summary>
            /// Blank Constructor
            /// </summary>
            public Specification()
            {
            }

            public long GetRequiredDatapointCount()
            {
                if (this.Classification == null || this.Statistic == null || this.Frequency.Period == null || this.Classification == null) return -1;
                if (this.Classification.Count == 0 || this.Frequency.Period.Count == 0 || this.Statistic.Count == 0 || this.Classification.Count == 0) return -1;
                long points = this.Statistic.Count * this.Frequency.Period.Count;
                foreach (var cls in this.Classification)
                {
                    points *= cls.Variable.Count;
                }

                return points;
            }

            /// <summary>
            /// Constructor based on a px document and domain
            /// </summary>
            /// <param name="doc"></param>
            /// <param name="domain"></param>
            public Specification(PxDocument doc, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, PxUpload_DTO pxUploadDto = null)
            {
                this.PxUploadDto = pxUploadDto;
                SetMultipleLanguagesProperties(doc, domain, null);
            }

            /// <summary>
            /// Constructor based on px document, domain and language code
            /// </summary>
            /// <param name="doc"></param>
            /// <param name="domain"></param>
            /// <param name="otherlanguage"></param>
            public Specification(PxDocument doc, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, string otherlanguage, PxUpload_DTO pxUploadDto = null)
            {
                this.PxUploadDto = pxUploadDto;
                SetMultipleLanguagesProperties(doc, domain, otherlanguage);
            }

            public Specification(PxDocument doc, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, string otherlanguage, string frqCodeTimeval, string frqValueTimeval)
            {

                SetMultipleLanguagesProperties(doc, domain, otherlanguage, frqCodeTimeval, frqValueTimeval);
            }

            /// <summary>
            /// cosntructor based on DTO input
            /// </summary>
            /// <param name="lngIsoCode"></param>
            /// <param name="dto"></param>
            internal Specification(string lngIsoCode, Build_DTO dto)
            {
                Language = lngIsoCode;
                Contents = dto.matrixDto.MtrTitle;
                NotesAsString = dto.matrixDto.MtrNote;


                //Get the dimension for this particular language
                var dimension = (from dim in dto.DimensionList where dim.LngIsoCode == lngIsoCode select dim).FirstOrDefault();

                if (dimension == null) return;

                CopyrightUrl = dimension.CprValue;
                Source = dimension.CprValue;

                if (dimension.Frequency != null)
                {
                    var periods = dimension.Frequency.Period != null ? dimension.Frequency.Period : new List<PeriodRecordDTO_Create>();
                    Frequency = dimension.Frequency;
                    Frequency.Period = periods;
                }
                if (Frequency.Code == null)
                    Frequency.Code = dto.FrqCode;

                Statistic = dimension.Statistics;
                Classification = dimension.Classifications;

                NotesAsString = dimension.MtrNote;

                Contents = dimension.Contents;

                Title = dimension.MtrTitle;

                ContentVariable = dimension.StatisticLabel;


            }
            //BuildUpdate_DTO
            /// <summary>
            /// cosntructor based on DTO input
            /// </summary>
            /// <param name="lngIsoCode"></param>
            /// <param name="dto"></param>
            internal Specification(string lngIsoCode, BuildUpdate_DTO dto)
            {



                //Get the dimension for this particular language
                var dimension = (from dim in dto.Dimension where dim.LngIsoCode == lngIsoCode select dim).FirstOrDefault();

                Language = lngIsoCode;
                Contents = dimension.MtrTitle;
                NotesAsString = dimension.MtrNote;


                if (dimension == null) return;

                CopyrightUrl = dimension.CprValue;
                Source = dimension.CprValue;

                if (dimension.Frequency != null)
                    Frequency = dimension.Frequency;

                if (Frequency.Code == null)
                    Frequency.Code = dimension.FrqCode;

                Statistic = dimension.Statistics;
                Classification = dimension.Classifications;

                NotesAsString = dimension.MtrNote;



                Title = dimension.MtrTitle;

                ContentVariable = dimension.StatisticLabel;



            }



            /// <summary>
            /// Consttructor based on matrixDTO
            /// </summary>
            /// <param name="ado"></param>
            /// <param name="matrixDto"></param>
            internal Specification(ADO ado, Matrix_DTO matrixDto)
            {

                Language = matrixDto.LngIsoCode;
                Contents = matrixDto.MtrTitle;
                NotesAsString = matrixDto.MtrNote;
                CopyrightUrl = matrixDto.CprUrl;
                Source = matrixDto.CprValue;
                Title = matrixDto.MtrTitle;
                ContentVariable = Label.Get("default.statistic");

                Frequency = new FrequencyRecordDTO_Create() { Code = matrixDto.FrqCode, Value = matrixDto.FrqValue };

                var matrixAdo = new Matrix_ADO(ado);
                var periodData = matrixAdo.ReadPeriodByRelease(matrixDto.RlsCode, matrixDto.LngIsoCode, matrixDto.FrqCode);
                Frequency.Period = new List<PeriodRecordDTO_Create>();
                foreach (var element in periodData)
                {
                    var period = new PeriodRecordDTO_Create() { Code = element.PrdCode, Value = element.PrdValue };
                    Frequency.Period.Add(period);
                }

                var statisticData = matrixAdo.ReadStatisticByRelease(matrixDto.RlsCode, matrixDto.LngIsoCode);
                Statistic = new List<StatisticalRecordDTO_Create>();
                foreach (var element in statisticData)
                {
                    var statistic = new StatisticalRecordDTO_Create()
                    {
                        Code = element.SttCode,
                        Value = element.SttValue,
                        Unit = element.SttUnit,
                        Decimal = element.SttDecimal

                    };
                    Statistic.Add(statistic);
                }

                var classificationData = matrixAdo.ReadClassificationByRelease(matrixDto.RlsCode, matrixDto.LngIsoCode);
                Classification = new List<ClassificationRecordDTO_Create>();
                foreach (var element in classificationData)
                {
                    var classification = new ClassificationRecordDTO_Create()
                    {
                        Code = element.ClsCode,
                        Value = element.ClsValue,
                        GeoFlag = DataAdaptor.ReadBool(element.ClsGeoFlag),
                        GeoUrl = DataAdaptor.ReadString(element.ClsGeoUrl)
                    };
                    Classification.Add(classification);
                    classification.Variable = new List<VariableRecordDTO_Create>();
                    var variableData = matrixAdo.ReadVariableByRelease(matrixDto.RlsCode, matrixDto.LngIsoCode, classification.Code);
                    foreach (var var in variableData)
                    {
                        var variable = new VariableRecordDTO_Create()
                        {
                            //GeoFlagCode = 
                            //GeoUrl
                            Code = var.VrbCode,
                            Value = var.VrbValue,
                        };
                        classification.Variable.Add(variable);
                    }
                }
            }



            /// <summary>
            /// class variable
            /// </summary>
            public IList<KeyValuePair<string, IList<IPxSingleElement>>> TimeVals { get; internal set; }

            /// <summary>
            /// class variable
            /// </summary>
            public FrequencyRecordDTO_Create Frequency { get; internal set; }

            /// <summary>
            /// class variable
            /// </summary>
            public IList<StatisticalRecordDTO_Create> Statistic;

            /// <summary>
            /// class variable
            /// </summary>
            public IList<ClassificationRecordDTO_Create> Classification;

            /// <summary>
            /// class property
            /// </summary>
            public IList<KeyValuePair<string, IList<IPxSingleElement>>> Values { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public IList<KeyValuePair<string, IList<IPxSingleElement>>> MainValues { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public IList<string> Notes { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public int MatrixId { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public string MatrixCode { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public string Source { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public string NotesAsString { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public string Contents { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public string Language { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public string Title { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public bool TimeValsIsDefined
            {
                get
                {
                    return TimeVals != null && TimeVals.Count > 0;
                }
            }

            /// <summary>
            ///  class property
            /// </summary>
            public IList<KeyValuePair<string, IList<IPxSingleElement>>> PeriodCodes
            {
                get
                {
                    return TimeValsIsDefined ? TimeVals : Values;
                }
            }

            /// <summary>
            /// class property
            /// </summary>
            public short Decimals { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public string ContentVariable { get; internal set; }

            /// <summary>
            /// class property
            /// </summary>
            public string CopyrightUrl { get; internal set; }


            private void SetMultipleLanguagesProperties(PxDocument doc, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, string language, string frqCodeTimeval, string frqValueTimeval)
            {
                if (!String.IsNullOrEmpty(language))
                {
                    Language = language;
                }

                //temp code until we remove old version. After that, frqCodeTimeval and frqValueTimeval will be optional parameters all the way from matrix creation
                frqCodeTimeval = frqCodeTimeval == "" ? null : frqCodeTimeval;
                frqValueTimeval = frqValueTimeval == "" ? null : frqValueTimeval;

                // Title for main language and Title[by other language] and same with all the others.
                Title = doc.GetStringElementValue("TITLE", language);
                Values = doc.GetMultiValuesWithSubkeys("VALUES", language);

                MatrixCode = doc.GetStringElementValue("MATRIX");

                MainValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatches("VALUES", language);

                Contents = doc.GetStringElementValue("CONTENTS", language);
                TimeVals = doc.GetMultiValuesWithSubkeysIfExist("TIMEVAL", language);
                Source = doc.GetStringElementValue("SOURCE", language);

                //DECIMALS is specific to Matrix and is language invariant but the value is needed in each language specification and the main spec
                Decimals = doc.GetShortElementValue("DECIMALS");
                if (Decimals == -1) Decimals = doc.GetShortElementValue("DECIMAL");

                // NOTEX takes over NOTE
                Notes = doc.GetListOfStringValuesIfExist("NOTEX", language);
                if (Notes == null)
                {
                    Notes = doc.GetListOfStringValuesIfExist("NOTE", language);
                }
                // Concatenate notes into one string
                NotesAsString = Notes == null ? "" : string.Join("", Notes.Select(e => e.ToString()));

                var maps = doc.GetSingleElementWithSubkeysIfExist("MAP", language);
                var codes = doc.GetMultiValuesWithSubkeysIfExist("CODES", language);
                var precisions = doc.GetSingleElementWithSubkeysIfExist("PRECISION", language);
                var units = doc.GetSingleElementWithSubkeys("UNITS", language);
                ContentVariable = doc.GetStringValueIfExist("CONTVARIABLE", language);
                var stubs = doc.GetListOfElementsIfExist("STUB", language);
                var headings = doc.GetListOfElementsIfExist("HEADING", language);


                // get the Statistical Products, Time and Dimensions
                Statistic = GetStatisticalProduct(doc, language, codes, units, precisions);

                //otherwise we'll atempt to fill time values from the FrqValue DTO item (user will choose it)
                //  if (PxUploadDto == null)
                Frequency = GetFrequency(headings, stubs, domain);

                //We may have to change things regarding the Frequency. This only happens for the build.

                if (frqCodeTimeval != null && frqValueTimeval != null)
                {

                    if (Frequency != null)
                    {
                        //We have a frequency and if we also have a FrqCode from the user, we'll change the FrqCode value
                        if (frqCodeTimeval != null) Frequency.Code = frqCodeTimeval;
                    }
                    else//No Timeval was defined in the px file and we got not Frquency so we try to either ascertain it from the FrqCode or send back to the user for clarification
                    {
                        //Not enough information, so we must ask the user
                        if (frqValueTimeval == null) this.requiresResponse = true;
                        else //We might be able to get the Frequency from the FrqValue
                        {
                            Frequency = GetFrequency(doc, frqCodeTimeval, frqValueTimeval);
                            Frequency_BSO fBso = new Frequency_BSO();

                            if (!fBso.ReadAll().Contains(frqCodeTimeval)) Frequency = null;
                            //No joy, so we must go back to the user.
                            if (Frequency == null) this.requiresResponse = true;
                        }
                    }

                }

                if (Frequency == null) this.requiresResponse = true;

                Classification = GetClassification(domain, codes, stubs, headings, Contents, maps);

                if (String.IsNullOrEmpty(ContentVariable))
                {
                    //ContentVariable = Contents;
                    ContentVariable = Utility.GetCustomConfig("APP_CSV_STATISTIC");
                }
            }


            /// <summary>
            /// Set properties for multiple languages
            /// </summary>
            /// <param name="doc"></param>
            /// <param name="domain"></param>
            /// <param name="language" - this is the language of the Specification></param>
            private void SetMultipleLanguagesProperties(PxDocument doc, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, string language)
            {
                if (!String.IsNullOrEmpty(language))
                {
                    Language = language;
                }

                // Title for main language and Title[by other language] and same with all the others.
                Title = doc.GetStringElementValue("TITLE", language);
                Values = doc.GetMultiValuesWithSubkeys("VALUES", language);

                MatrixCode = doc.GetStringElementValue("MATRIX");

                MainValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatches("VALUES", language);

                Contents = doc.GetStringElementValue("CONTENTS", language);
                TimeVals = doc.GetMultiValuesWithSubkeysIfExist("TIMEVAL", language);
                Source = doc.GetStringElementValue("SOURCE", language);

                //DECIMALS is specific to Matrix and is language invariant but the value is needed in each language specification and the main spec
                Decimals = doc.GetShortElementValue("DECIMALS");
                if (Decimals == -1) Decimals = doc.GetShortElementValue("DECIMAL");

                // NOTEX takes over NOTE
                Notes = doc.GetListOfStringValuesIfExist("NOTEX", language);
                if (Notes == null)
                {
                    Notes = doc.GetListOfStringValuesIfExist("NOTE", language);
                }
                // Concatenate notes into one string
                NotesAsString = Notes == null ? "" : string.Join("", Notes.Select(e => e.ToString()));

                var maps = doc.GetSingleElementWithSubkeysIfExist("MAP", language);
                var codes = doc.GetMultiValuesWithSubkeysIfExist("CODES", language);
                var precisions = doc.GetSingleElementWithSubkeysIfExist("PRECISION", language);
                var units = doc.GetSingleElementWithSubkeys("UNITS", language);
                ContentVariable = doc.GetStringValueIfExist("CONTVARIABLE", language);
                var stubs = doc.GetListOfElementsIfExist("STUB", language);
                var headings = doc.GetListOfElementsIfExist("HEADING", language);


                // get the Statistical Products, Time and Dimensions
                Statistic = GetStatisticalProduct(doc, language, codes, units, precisions);

                //otherwise we'll atempt to fill time values from the FrqValue DTO item (user will choose it)
                //  if (PxUploadDto == null)
                Frequency = GetFrequency(headings, stubs, domain);

                //We may have to change things regarding the Frequency. This only happens for the build.
                if (PxUploadDto != null)
                {

                    if (Frequency != null)
                    {
                        //We have a frequency and if we also have a FrqCode from the user, we'll change the FrqCode value
                        if (PxUploadDto.FrqCodeTimeval != null) Frequency.Code = PxUploadDto.FrqCodeTimeval;
                    }
                    else//No Timeval was defined in the px file and we got no Frquency so we try to either ascertain it from the FrqCode or send back to the user for clarification
                    {
                        //Not enough information, so we must ask the user
                        if (PxUploadDto.FrqValueTimeval == null) this.requiresResponse = true;
                        else //We might be able to get the Frequency from the FrqValue
                        {
                            Frequency = GetFrequency(doc, PxUploadDto.LngIsoCode, domain);
                            Frequency_BSO fBso = new Frequency_BSO();

                            if (!fBso.ReadAll().Contains(PxUploadDto.FrqCodeTimeval)) Frequency = null;
                            //No joy, so we must go back to the user.
                            if (Frequency == null) this.requiresResponse = true;
                        }
                    }
                }



                Classification = GetClassification(domain, codes, stubs, headings, Contents, maps);

                if (String.IsNullOrEmpty(ContentVariable))
                {
                    //ContentVariable = Contents;
                    ContentVariable = Label.Get("default.statistic");
                }
            }

            private FrequencyRecordDTO_Create GetFrequency(PxDocument doc, string frqCodeTimeval, string frqValueTimeval)
            {

                FrequencyRecordDTO_Create fr = new FrequencyRecordDTO_Create();
                fr.Period = new List<PeriodRecordDTO_Create>();


                //Here we must get a translated version of FrqValueTimeval. FrqValueTimeval was supplied in one language, but we must get the corresponding version for this Specification's language
                fr.Value = TranslateValue(doc, frqValueTimeval, Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE"), Language);

                fr.Value = fr.Value == null ? frqValueTimeval : fr.Value;

                var vals = Values.Where(x => x.Key == fr.Value).FirstOrDefault();

                //we need to take this from a translated version of FrqValue rather than the supplied one.

                IEnumerable<IEnumerable<IPxSingleElement>> periodValues =
                   from c in Values
                   where c.Key == fr.Value
                   select c.Value;

                //Return a null at this stage if we can't create a frquency
                if (periodValues.Count() == 0)
                {
                    return null;
                }

                var periods = periodValues.ElementAt(0);
                foreach (var v in periods)
                {
                    var periodRecord = new PeriodRecordDTO_Create() { Code = v.SingleValue, Value = v.SingleValue };
                    fr.Period.Add(periodRecord);

                }
                fr.Code = frqCodeTimeval;
                return fr;

            }


            private FrequencyRecordDTO_Create GetFrequency(PxDocument doc, string language, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain)
            {

                FrequencyRecordDTO_Create fr = new FrequencyRecordDTO_Create();
                fr.Period = new List<PeriodRecordDTO_Create>();


                //Here we must get a translated version of FrqValueTimeval. FrqValueTimeval was supplied in one language, but we must get the corresponding version for this Specification's language
                fr.Value = TranslateValue(doc, PxUploadDto.FrqValueTimeval, PxUploadDto.LngIsoCode, Language);

                fr.Value = fr.Value == null ? PxUploadDto.FrqValueTimeval : fr.Value;

                var vals = Values.Where(x => x.Key == fr.Value).FirstOrDefault();

                //we need to take this from a translated version of FrqValue rather than the supplied one.

                IEnumerable<IEnumerable<IPxSingleElement>> periodValues =
                   from c in Values
                   where c.Key == fr.Value
                   select c.Value;

                //Return a null at this stage if we can't create a frquency
                if (periodValues.Count() == 0)
                {
                    return null;
                }

                var periods = periodValues.ElementAt(0);
                foreach (var v in periods)
                {
                    var periodRecord = new PeriodRecordDTO_Create() { Code = v.SingleValue, Value = v.SingleValue };
                    fr.Period.Add(periodRecord);

                }
                fr.Code = PxUploadDto.FrqCodeTimeval;
                return fr;

            }
            /// <summary>
            /// Translates a [VALUES] item in px from one language version to another
            /// </summary>
            /// <param name="doc"></param>
            /// <param name="value"></param>
            /// <param name="lngIsoCodeFrom"></param>
            /// <param name="lngIsoCodeTo"></param>
            /// <returns></returns>
            private string TranslateValue(PxDocument doc, string value, string lngIsoCodeFrom, string lngIsoCodeTo)
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

                var otherValues = doc.GetMultiValuesWithSubkeysOnlyIfLanguageMatches("VALUES", lngIsoCodeTo);

                if (otherValues == null) return value;

                var otherVal = otherValues.ElementAt(thisPos);


                return otherVal.Key == null ? value : otherVal.Key;
            }


            /// <summary>
            /// Get a Frequency record
            /// </summary>
            /// <param name="headings"></param>
            /// <param name="stubs"></param>
            /// <param name="domain"></param>
            /// <returns></returns>
            private FrequencyRecordDTO_Create GetFrequency(IList<IPxSingleElement> headings, IList<IPxSingleElement> stubs, IList<KeyValuePair<string, IList<IPxSingleElement>>> domain)
            {
                // typically the period is defined in the headings, but can also be in stubs
                // when this happens we should have a timeval to helps us make the right call
                // therefore we now check timeval first

                string frequencyValue = null;
                if (TimeValsIsDefined)
                {
                    frequencyValue = PeriodCodes.ElementAt(0).Key;
                }


                if (frequencyValue == null) return null;

                string aFrequencyCode;

                // if time vals exist use them for the code, otherwise make the period code and value the same
                // also make the frequency code and value the same
                if (TimeValsIsDefined)
                {
                    aFrequencyCode = PeriodCodes.FirstOrDefault(p => p.Key == frequencyValue).Value.First().SingleValue;
                }
                else
                {
                    aFrequencyCode = frequencyValue;
                }



                var frequencyRecord = new FrequencyRecordDTO_Create() { Code = aFrequencyCode, Value = frequencyValue };
                frequencyRecord.Period = new List<PeriodRecordDTO_Create>();

                // that use the corresponding values that are left to get from values the correspondent period values
                IEnumerable<IEnumerable<IPxSingleElement>> periodValues =
                    from c in Values
                    where c.Key == frequencyValue
                    select c.Value;

                IEnumerable<IPxSingleElement> thePeriodValues = periodValues.First();
                IEnumerable<IPxSingleElement> thePeriodCodes;

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
                    var c = thePeriodCodes.ElementAt(i + (TimeValsIsDefined ? 1 : 0));
                    var periodRecord = new PeriodRecordDTO_Create() { Code = c.SingleValue, Value = v.SingleValue };
                    frequencyRecord.Period.Add(periodRecord);
                    ++i;
                }

                return frequencyRecord;
            }

            /// <summary>
            /// Get classification data
            /// </summary>
            /// <param name="domain"></param>
            /// <param name="codes"></param>
            /// <param name="stubs"></param>
            /// <param name="headings"></param>
            /// <param name="contents"></param>
            /// <param name="maps"></param>
            /// <returns></returns>
            private IList<ClassificationRecordDTO_Create> GetClassification(IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, IList<KeyValuePair<string, IList<IPxSingleElement>>> codes, IList<IPxSingleElement> stubs, IList<IPxSingleElement> headings, string contents, IList<KeyValuePair<string, IList<IPxSingleElement>>> maps)
            {

                List<ClassificationRecordDTO_Create> retValue = new List<ClassificationRecordDTO_Create>();

                // In case the time is in the stubs we exclude it in the where clause to avoid having time duplicated!
                IEnumerable<string> dimensionInStubs = Enumerable.Empty<string>();

                if (stubs != null && Frequency != null)
                {
                    dimensionInStubs =
                    from v in stubs
                    where v.SingleValue != Frequency.Value
                    select (string)v.SingleValue;
                }


                IEnumerable<string> dimensionsInHeadings = Enumerable.Empty<string>();
                if (headings != null && Frequency != null)
                {
                    dimensionsInHeadings =
                                        from v in headings
                                        where v.SingleValue != Frequency.Value
                                        select (string)v.SingleValue;
                }

                // remove Statistics 
                List<string> dimensionValues = dimensionInStubs.Concat(dimensionsInHeadings).ToList<string>();
                HashSet<string> statisticValues = new HashSet<string>(Statistic.Select(x => x.Value));

                dimensionValues.RemoveAll(v => statisticValues.Contains(v));

                dimensionValues.RemoveAll(v => v == ContentVariable || v == contents);


                LoopClassifications(domain, codes, retValue, dimensionValues, maps);

                // if we have no dimensions we mock up a default dimension
                if (retValue.Count == 0)
                {
                    return MockClassification();
                }

                return retValue;
            }

            /// <summary>
            /// Find all classifications and associated variables
            /// </summary>
            /// <param name="domain"></param>
            /// <param name="codes"></param>
            /// <param name="retValue"></param>
            /// <param name="dimensionValues"></param>
            /// <param name="maps"></param>
            private void LoopClassifications(IList<KeyValuePair<string, IList<IPxSingleElement>>> domain, IList<KeyValuePair<string, IList<IPxSingleElement>>> codes, List<ClassificationRecordDTO_Create> retValue, IEnumerable<string> dimensionValues, IList<KeyValuePair<string, IList<IPxSingleElement>>> maps)
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

                    var classificationRecord = new ClassificationRecordDTO_Create() { Code = code, Value = value };
                    if (maps != null && maps.Any())
                    {
                        var mapLines = maps.Where(m => m.Key == value).Select(m => m.Value).FirstOrDefault();
                        classificationRecord.GeoUrl = "";
                        if (mapLines != null && mapLines.Any())
                        {
                            classificationRecord.GeoUrl = string.Join("", mapLines.Select(e => e.ToPxValue()));
                            classificationRecord.GeoFlag = true;
                        }
                    }

                    classificationRecord.Variable = new List<VariableRecordDTO_Create>();

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
                        // if codes do not exist we assign default progressive number
                        dynamic variableCode;
                        if (variableCodes != null)
                        {
                            variableCode = variableCodes.ElementAt(i).SingleValue;
                        }
                        else
                        {
                            variableCode = i.ToString();
                        }

                        var variableRecord = new VariableRecordDTO_Create() { Code = variableCode, Value = variableValue.SingleValue };
                        classificationRecord.Variable.Add(variableRecord);
                        ++i;
                    }
                    //If there's no code, get one based on the hash of the classification plus its variable
                    if (code.Length == 0)
                    {
                        dimensionCounter++;

                        int hash = classificationRecord.GetHashCode();
                        foreach (var vrb in classificationRecord.Variable)
                            hash = hash ^ vrb.Code.GetHashCode();
                        classificationRecord.Code = Utility.GetMD5(hash.ToString()) + dimensionCounter.ToString();
                    }

                    retValue.Add(classificationRecord);
                }
            }

            /// <summary>
            /// Create blank classification
            /// </summary>
            /// <returns></returns>
            private IList<ClassificationRecordDTO_Create> MockClassification()
            {
                return new List<ClassificationRecordDTO_Create>() {
                    new ClassificationRecordDTO_Create {
                        Code = "0",
                        Value = Label.Get("default.classification"),
                        Variable = new List<VariableRecordDTO_Create>() { new VariableRecordDTO_Create()
                            {
                             Code ="0"  , Value = Label.Get("default.variable")
                            }
                        }
                    }
                };
            }

            /// <summary>
            /// Get Statistic
            /// </summary>
            /// <param name="doc"></param>
            /// <param name="language"></param>
            /// <param name="codes"></param>
            /// <param name="units"></param>
            /// <param name="precisions"></param>
            /// <returns></returns>
            private IList<StatisticalRecordDTO_Create> GetStatisticalProduct(PxDocument doc, string language, IList<KeyValuePair<string, IList<IPxSingleElement>>> codes, IList<KeyValuePair<string, IList<IPxSingleElement>>> units, IList<KeyValuePair<string, IList<IPxSingleElement>>> precisions)
            {
                IList<StatisticalRecordDTO_Create> retValue = new List<StatisticalRecordDTO_Create>();

                if (!String.IsNullOrEmpty(ContentVariable))
                {
                    var statisticalIndicatorValues = Values.FirstOrDefault(i => i.Key == ContentVariable).Value;
                    var statisticalIndicatorCodes = codes.FirstOrDefault(i => i.Key == ContentVariable).Value;


                    int index = 0;
                    foreach (var value in statisticalIndicatorValues)
                    {
                        var code = statisticalIndicatorCodes.ElementAt(index).SingleValue;
                        string aUnit = units.FirstOrDefault(i => i.Key == value.SingleValue).Value.First().SingleValue;

                        var statisticalRecord = new StatisticalRecordDTO_Create() { Code = code, Unit = aUnit, Value = value.SingleValue, Decimal = Decimals };
                        if (precisions != null)
                        {
                            KeyValuePair<string, IList<IPxSingleElement>> precisionValue = precisions.FirstOrDefault(i => i.Key == value.SingleValue);

                            if (precisionValue.Key != null && !String.IsNullOrEmpty(precisionValue.Key))
                            {
                                statisticalRecord.Decimal = (short)precisionValue.Value.FirstOrDefault().SingleValue;
                            }
                        }

                        retValue.Add(statisticalRecord);
                        ++index;
                    }
                }
                else
                {

                    // if there is only one statistical product we have no subkeys
                    var unit = doc.GetStringElementValue("UNITS", language);

                    var statisticalRecord = new StatisticalRecordDTO_Create() { Unit = unit, Value = Contents, Decimal = Decimals };
                    statisticalRecord.Code = this.MatrixCode;
                    if (precisions != null)
                    {
                        KeyValuePair<string, IList<IPxSingleElement>> precisionValue = precisions.FirstOrDefault(i => i.Key == Contents);

                        if (precisionValue.Key != null && !String.IsNullOrEmpty(precisionValue.Key))
                        {
                            statisticalRecord.Decimal = (short)precisionValue.Value.FirstOrDefault().SingleValue;
                        }
                    }

                    retValue.Add(statisticalRecord);
                }

                return retValue;
            }
        }



        /// <summary>
        /// Get a Px file as a StringBuilder object from the current Matrix
        /// </summary>
        /// <param name="forceStatisticEntry" - make sure there is at Statistic/Contvariable entry even if there's only one statistic></param>
        /// <returns></returns>
        internal StringBuilder GetPxObject(bool forceStatisticEntry = false)
        {

            var sb = new StringBuilder();
            var baseKeywordList = new List<IPxKeywordElement>();
            var dataKeywordList = new List<IPxKeywordElement>();
            bool contVariable = MainSpec.Statistic.Count > 1;

            baseKeywordList.Add(CreatePxElement("CHARSET", Utility.GetCustomConfig("APP_PX_DEFAULT_CHARSET")));
            baseKeywordList.Add(CreatePxElement("AXIS-VERSION", Utility.GetCustomConfig("APP_PX_DEFAULT_AXIS_VERSION")));


            baseKeywordList.Add(CreatePxElement("LANGUAGE", this.TheLanguage));
            if (OtherLanguages != null)
            {
                List<string> langs = OtherLanguages.ToList<string>();
                langs.Insert(0, MainSpec.Language);
                var langsList = new PxListOfValues(langs);
                baseKeywordList.Add(CreatePxElementUnquoted("LANGUAGES", langsList.ToPxQuotedString()));

            }

            baseKeywordList.Add(CreatePxElement("CREATION-DATE", (DateTime.Now).ToString(Utility.GetCustomConfig("APP_PX_DATE_TIME_FORMAT"))));

            baseKeywordList.Add(CreatePxElement("OFFICIAL-STATISTICS", this.IsOfficialStatistic ? Utility.GetCustomConfig("APP_PX_TRUE") : Utility.GetCustomConfig("APP_PX_FALSE")));

            baseKeywordList.Add(CreatePxElement("DECIMALS", this.MainSpec.Statistic.Min(x => x.Decimal)));

            baseKeywordList.Add(CreatePxElement("MATRIX", Code));


            //Separate out the stub and heading items
            Dictionary<string, PxListOfValues> stub = getStubDefault(MainSpec, MainSpec.ContentVariable, forceStatisticEntry);
            Dictionary<string, PxListOfValues> heading = getHeadingDefault(MainSpec);

            //Get the count of values in the heading
            int headingCount = 1;
            foreach (var v in heading)
            {
                headingCount = headingCount * v.Value.Values.Count;
            }

            //Create the entries that vary according to language
            var mainSpecDoc = new PxDocument(GetKeywordsForSpec(MainSpec, stub, heading, null, forceStatisticEntry));  // replace main spec with individual  specs in a loop

            List<PxDocument> otherSpecDocs = new List<PxDocument>();

            if (OtherLanguageSpec != null)
            {
                foreach (var spec in OtherLanguageSpec)
                {
                    stub = getStubDefault(spec, spec.ContentVariable, forceStatisticEntry);
                    heading = getHeadingDefault(spec);
                    otherSpecDocs.Add(new PxDocument(GetKeywordsForSpec(spec, stub, heading, spec.Language, forceStatisticEntry)));
                }
            }


            //Get the data cells. Null values will be given a default string value, i.e. APP_PX_CONFIDENTIAL_VALUE
            if (Cells != null)
            {
                string defaultVal = Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE");
                List<string> dataCells = new List<string>();
                foreach (var c in Cells)
                {
                    var v = c.GetType();
                    double result;
                    string sval = Convert.ToString(c.TdtValue);
                    if (string.IsNullOrEmpty(sval) || sval.Equals(defaultVal))
                    {
                        dataCells.Add('"' + defaultVal + '"');
                    }
                    else if (Double.TryParse(Convert.ToString(c.TdtValue), out result))
                    {
                        dataCells.Add(c.TdtValue);
                    }
                    else dataCells.Add(c.TdtValue);

                }

                if (dataCells.Count > 0)
                {
                    var codesList = new PxListOfValues(dataCells);
                    dataKeywordList.Add(CreatePxElementUnquoted("DATA", codesList.ToPxDataString(headingCount)));

                }
            }

            var baseDoc = new PxDocument(baseKeywordList);
            var dataDoc = new PxDocument(dataKeywordList);


            sb.Append(baseDoc.ToPxString());
            sb.Append(mainSpecDoc.ToPxString());
            foreach (var doc in otherSpecDocs)
                sb.Append(doc.ToPxString());
            sb.Append(dataDoc.ToPxString());

            return sb;
        }



        /// <summary>
        /// Get the required PxKeywordElements from the current Matrix
        /// </summary>
        /// <param name="theSpec"></param>
        /// <param name="stub"></param>
        /// <param name="heading"></param>
        /// <param name="lngIsoCode"></param>
        /// <param name="forceStatisticEntry" - make sure there is at Statistic/Contvariable entry even if there's only one statistic></param>
        /// <returns></returns>
        private List<IPxKeywordElement> GetKeywordsForSpec(Specification theSpec, Dictionary<string, PxListOfValues> stub, Dictionary<string, PxListOfValues> heading, string lngIsoCode = null, bool forceStatisticEntry = false)
        {
            var keywordList = new List<IPxKeywordElement>();
            bool contVariable = (theSpec.Statistic.Count > 1 || forceStatisticEntry);


            string lngTag = lngIsoCode != null ? "[" + lngIsoCode + "]" : "";


            if (this.Release != null)
            {
                keywordList.Add(CreatePxElement("SUBJECT-AREA" + lngTag, !String.IsNullOrEmpty(this.Release.PrcValue) ? this.Release.PrcValue : Label.Get("default.subject-area")));
                keywordList.Add(CreatePxElement("SUBJECT-CODE" + lngTag, !String.IsNullOrEmpty(this.Release.PrcValue) ? this.Release.PrcCode.ToString() : Label.Get("default.subject-code"))); ////
            }
            else
            {
                keywordList.Add(CreatePxElement("SUBJECT-AREA" + lngTag, Label.Get("default.subject-area")));
                keywordList.Add(CreatePxElement("SUBJECT-CODE" + lngTag, Label.Get("default.subject-code")));
            }

            keywordList.Add(CreatePxElement("DESCRIPTION" + lngTag, theSpec.Title)); /////

            if (theSpec.Title != null)
                keywordList.Add(CreatePxElement("TITLE" + lngTag, theSpec.Title));
            else
                keywordList.Add(CreatePxElement("TITLE" + lngTag, getContentString(theSpec))); ////

            keywordList.Add(CreatePxElement("CONTENTS" + lngTag, theSpec.Contents)); ////

            //Units
            var distinctUnits = theSpec.Statistic.Select(o => o.Unit).Distinct();
            if (!contVariable)
                keywordList.Add(CreatePxElement("UNITS" + lngTag, distinctUnits.ElementAt(0)));
            else
            {
                keywordList.Add(CreatePxElement("UNITS" + lngTag, Utility.GetCustomConfig("APP_PX_DEFAULT_UNITS")));
            }


            //Create the Stub entries
            List<string> stubs = new List<string>();
            foreach (KeyValuePair<string, PxListOfValues> pair in stub)
            {
                stubs.Add(pair.Key);
            }
            var stubsList = new PxListOfValues(stubs);
            keywordList.Add(CreatePxElementUnquoted("STUB" + lngTag, stubsList.ToPxQuotedString())); ////


            //Create the Heading entries
            List<string> headings = new List<string>();
            foreach (KeyValuePair<string, PxListOfValues> pair in heading)
            {
                headings.Add(pair.Key);
            }
            var headingsList = new PxListOfValues(headings);
            keywordList.Add(CreatePxElementUnquoted("HEADING" + lngTag, headingsList.ToPxQuotedString())); ////

            //CONTVARIABLE
            if (contVariable)
            {
                keywordList.Add(CreatePxElement("CONTVARIABLE" + lngTag, theSpec.ContentVariable)); ////
            }

            //Create the VALUE tags for stub (Statistic, then Time)
            foreach (KeyValuePair<string, PxListOfValues> pair in stub)
            {
                keywordList.Add(CreatePxElementUnquoted("VALUES" + lngTag, pair.Key, pair.Value.ToPxQuotedString("VALUES", Convert.ToInt32(Utility.GetCustomConfig("APP_PX_MULTILINE_CHAR_LIMIT"))))); ////
            }



            //Create the VALUE tags for heading (All other dimensions in the order they're stored in the database)
            foreach (KeyValuePair<string, PxListOfValues> pair in heading)
            {
                keywordList.Add(CreatePxElementUnquoted("VALUES" + lngTag, pair.Key, pair.Value.ToPxQuotedString("VALUES", Convert.ToInt32(Utility.GetCustomConfig("APP_PX_MULTILINE_CHAR_LIMIT"))))); ////
            }




            //Create the Timeval
            List<string> periods = new List<string>();
            if (theSpec.Frequency.Period != null)
            {

                foreach (var period in theSpec.Frequency.Period)
                {
                    periods.Add(period.Code);
                }

            }
            else periods.Add(Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE"));

            if (periods.Count > 0)
            {
                var periodList = new PxListOfValues(periods);

                keywordList.Add(CreatePxElementUnquoted("TIMEVAL" + lngTag, theSpec.Frequency.Value, theSpec.Frequency.Code + ',' + periodList.ToPxQuotedString())); ////

            }

            //Create the CODEs
            foreach (var cls in theSpec.Classification)
            {
                List<string> codes = new List<string>();
                foreach (var v in cls.Variable)
                {
                    codes.Add(v.Code);
                }
                if (codes.Count > 0)
                {
                    var codesList = new PxListOfValues(codes);
                    keywordList.Add(CreatePxElementUnquoted("CODES" + lngTag, cls.Value, codesList.ToPxQuotedString("CODES", Convert.ToInt32(Utility.GetCustomConfig("APP_PX_MULTILINE_CHAR_LIMIT"))))); ////

                }

            }

            //if (contVariable && distinctUnits.Count() > 1)
            if (contVariable)// && theSpec.Statistic.Count > 1)
            {
                List<string> statistics = new List<string>();
                foreach (var stat in theSpec.Statistic)
                {
                    statistics.Add(stat.Code);
                }
                if (statistics.Count > 0)
                {
                    var statsList = new PxListOfValues(statistics);
                    keywordList.Add(CreatePxElementUnquoted("CODES" + lngTag, theSpec.ContentVariable, statsList.ToPxQuotedString()));  ////
                }

            }

            //Domain
            foreach (var c in theSpec.Classification)
            {
                keywordList.Add(CreatePxElement("DOMAIN" + lngTag, c.Value, new PxStringValue(c.Code))); ////

            }



            //Create the MAP data
            foreach (var c in theSpec.Classification)
            {
                if (!String.IsNullOrEmpty(c.GeoUrl))
                    keywordList.Add(CreatePxElement("MAP" + lngTag, c.Value, new PxStringValue(c.GeoUrl))); ////
            }

            //Create the Units

            if (contVariable)
            {
                foreach (var stat in theSpec.Statistic)
                {
                    keywordList.Add(CreatePxElement("UNITS" + lngTag, stat.Value, stat.Unit)); ////
                }

            }



            //Refperiod and Baseperiod
            if (contVariable)
            {
                foreach (var stat in theSpec.Statistic)
                {
                    keywordList.Add(CreatePxElement("REFPERIOD" + lngTag, stat.Value, " ")); ////
                }
                foreach (var stat in theSpec.Statistic)
                {
                    keywordList.Add(CreatePxElement("BASEPERIOD" + lngTag, stat.Value, " ")); ////
                }
            }

            keywordList.Add(CreatePxElement("SOURCE" + lngTag, theSpec.Source));

            keywordList.Add(CreatePxElement("NOTEX" + lngTag, theSpec.NotesAsString));

            if (MainSpec.Statistic.Count > 1)
            {

                foreach (var stat in theSpec.Statistic)
                {
                    if (stat.Decimal.ToString() != "0")
                    {
                        List<string> statistics = new List<string>();
                        statistics.Add(theSpec.ContentVariable);

                        statistics.Add(stat.Value);
                        var statsList = new PxListOfValues(statistics);

                        IPxKeywordElement pxOut = CreatePxElementUnquotedIndividually("PRECISION" + lngTag, statsList.ToPxQuotedString(), stat.Decimal.ToString());

                        keywordList.Add(pxOut); ////
                    }
                }



            }

            return keywordList;
        }

        /// <summary>
        /// Returns the Content string for the Sepcification
        /// </summary>
        /// <param name="theSpec"></param>
        /// <returns></returns>
        private string getContentString(Specification theSpec)
        {
            string content = theSpec.Contents;
            IEnumerable<string> clsList = new List<string>();
            clsList = theSpec.Classification.Select(f => f.Value);
            if (clsList.Count() > 0) content = content + " (" + String.Join(",", clsList) + ")";

            return content;
        }

        /// <summary>
        /// Gets the Stub data
        /// </summary>
        /// <param name="theSpec"></param>
        /// <param name="contVariableStatistic"></param>
        /// <returns></returns>
        private Dictionary<string, PxListOfValues> getStubDefault(Specification theSpec, string contVariableStatistic, bool forceStatisticEntry = false)
        {
            Dictionary<string, PxListOfValues> stub = new Dictionary<string, PxListOfValues>();
            List<string> periods = new List<string>();
            List<string> statistics = new List<string>();

            Dictionary<string, PxListOfValues> headingDict = new Dictionary<string, PxListOfValues>();

            if (MainSpec.Statistic.Count > 1 || forceStatisticEntry)
            {
                foreach (var s in theSpec.Statistic)
                {
                    statistics.Add(s.Value);
                }
                headingDict.Add(contVariableStatistic, new PxListOfValues(statistics));


            }

            if (theSpec.Frequency.Period != null)
            {

                foreach (var p in theSpec.Frequency.Period)
                {
                    periods.Add(p.Value);
                }
                headingDict.Add(theSpec.Frequency.Value, new PxListOfValues(periods));
            }
            else //Blank px file without period information - insert placeholder value based on frequency
            {
                periods.Add(theSpec.Frequency.Value);
                headingDict.Add(theSpec.Frequency.Value, new PxListOfValues(periods));
            }




            return headingDict;
        }

        /// <summary>
        /// Gets the Heading data
        /// </summary>
        /// <param name="theSpec"></param>
        /// <returns></returns>
        private Dictionary<string, PxListOfValues> getHeadingDefault(Specification theSpec)
        {
            Dictionary<string, PxListOfValues> stub = new Dictionary<string, PxListOfValues>();
            List<string> periods = new List<string>();
            List<string> statistics = new List<string>();

            Dictionary<string, PxListOfValues> stubDict = new Dictionary<string, PxListOfValues>();

            foreach (var c in theSpec.Classification)
            {
                List<string> variables = new List<string>();
                foreach (var v in c.Variable)
                {
                    variables.Add(v.Value);
                }
                stubDict.Add(c.Value, new PxListOfValues(variables));
            }

            return stubDict;
        }


        /// <summary>
        /// CreatePxElement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IPxKeywordElement CreatePxElement(string key, string value)
        {
            return new PxKeywordElement(new PxKey(key), new PxQuotedValue(value));
        }

        /// <summary>
        /// CreatePxElement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IPxKeywordElement CreatePxElement(string key, string subKey, string value)
        {
            return new PxKeywordElement(new PxKey(key, new PxSubKey(subKey)), new PxQuotedValue(value));
        }

        /// <summary>
        /// Use CreatePxElementUnquoted when the elements on the right hand side of the '=' already have their own quotes, i.e. 
        /// when we have invoked ToPxQuotedString
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IPxKeywordElement CreatePxElementUnquoted(string key, string value)
        {
            return new PxKeywordElement(new PxKey(key), new PxStringValue(value));
        }

        /// <summary>
        /// CreatePxElementUnquoted
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IPxKeywordElement CreatePxElementUnquoted(string key, string subKey, string value)
        {

            PxKeywordElement pk = new PxKeywordElement(new PxKey(key, new PxSubKey(subKey)), new PxStringValue(value));

            return pk;
        }

        /// <summary>
        /// CreatePxElementUnquotedIndividually
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IPxKeywordElement CreatePxElementUnquotedIndividually(string key, string subKey, string value)
        {

            PxKeywordElement pk = new PxKeywordElement(new PxKey(key, (new PxSubKey(subKey))), new PxStringValue(value));
            return pk;
        }

        /// <summary>
        /// CreatePxElement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IPxKeywordElement CreatePxElement(string key, short value)
        {
            return new PxKeywordElement(new PxKey(key), new PxLiteralValue(value));
        }

        /// <summary>
        /// CreatePxElement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IPxKeywordElement CreatePxElement(string key, int value)
        {
            return new PxKeywordElement(new PxKey(key), new PxLiteralValue(value));
        }

        /// <summary>
        /// class variable
        /// </summary>
        List<string> headerString;

        /// <summary>
        /// Add a classification
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="clsLines"></param>
        /// <param name="clsCounter"></param>
        /// <param name="line"></param>
        internal void appendClassification(ClassificationRecordDTO_Create cls, List<string> clsLines, int clsCounter, string line, string lngIsoCode = null)
        {
            Specification theSpec;
            if (lngIsoCode != null)
            {
                theSpec = this.GetSpecFromLanguage(lngIsoCode);
                if (theSpec == null) theSpec = this.MainSpec;
            }
            else
            {
                theSpec = this.MainSpec;
            }

            foreach (var vrb in cls.Variable)
            {

                if (clsCounter >= (theSpec.Classification.Count - 1))
                {
                    clsLines.Add(line + ',' + new PxQuotedValue(vrb.Code).ToPxString() + ',' + new PxQuotedValue(vrb.Value).ToPxString());

                }
                else
                {
                    //append the variable code plus variable value to the line
                    //get the next classification and recursively call this function again with clsCounter++
                    clsCounter++;
                    string oldLine = line;
                    line = line + ',' + new PxQuotedValue(vrb.Code).ToPxString() + ',' + new PxQuotedValue(vrb.Value).ToPxString();


                    ClassificationRecordDTO_Create nextCls = theSpec.Classification.ElementAt(clsCounter);
                    appendClassification(nextCls, clsLines, clsCounter, line, lngIsoCode);

                    //we're finished with the classification so back to where we were...
                    clsCounter--;
                    line = oldLine;

                }
            }
        }

        internal string GetXlsxObject(string lngIsoCode = null, CultureInfo ci = null)
        {
            Xlsx xl = new Xlsx();
            return xl.GetXlsx(this, GetMatrixSheet(lngIsoCode), lngIsoCode, ci);

        }

        internal string GetCsvObject(string lngIsoCode = null, bool indicateBlankSymbols = false, CultureInfo ci = null)
        {
            Xlsx xl = new Xlsx();
            return xl.GetCsv(GetMatrixSheet(lngIsoCode, indicateBlankSymbols), "\"", ci);
        }

        internal List<List<XlsxValue>> GetMatrixSheet(string lngIsoCode = null, bool indicateBlankSymbols = false)
        {

            List<List<XlsxValue>> rowLists = new List<List<XlsxValue>>();

            lngIsoCode = lngIsoCode == null ? Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE") : lngIsoCode;

            Specification theSpec;
            if (lngIsoCode != null)
            {
                theSpec = this.GetSpecFromLanguage(lngIsoCode);
                if (theSpec == null) theSpec = this.MainSpec;
                theSpec.Frequency = MainSpec.Frequency;
            }
            else
                theSpec = this.MainSpec;

            List<XlsxValue> rowList = new List<XlsxValue>();
            rowList.Add(new XlsxValue() { Value = Utility.GetCustomConfig("APP_CSV_STATISTIC") });
            rowList.Add(new XlsxValue() { Value = Utility.GetCustomConfig("APP_CSV_STATISTIC") + Utility.GetCustomConfig("APP_CSV_DIVIDER") + (theSpec.ContentVariable != null ? theSpec.ContentVariable : Label.Get("default.statistic")) });
            rowList.Add(new XlsxValue() { Value = theSpec.Frequency.Code });
            rowList.Add(new XlsxValue() { Value = theSpec.Frequency.Code + Utility.GetCustomConfig("APP_CSV_DIVIDER") + theSpec.Frequency.Value });

            foreach (var cls in theSpec.Classification)
            {
                rowList.Add(new XlsxValue() { Value = cls.Code });
                rowList.Add(new XlsxValue() { Value = cls.Code + Utility.GetCustomConfig("APP_CSV_DIVIDER") + cls.Value });
            }

            rowList.Add(new XlsxValue() { Value = Label.Get("xlsx.unit", lngIsoCode) });
            rowList.Add(new XlsxValue() { Value = Label.Get("xlsx.value", lngIsoCode) });

            rowLists.Add(rowList);

            ClassificationRecordDTO_Create cDto = theSpec.Classification.FirstOrDefault();
            List<string> clsLines = new List<string>();
            int cellCounter = 0;

            List<List<string>> clsList = new List<List<string>>();



            IEnumerable<ValueElement> cells;


            cells = Cells.Select(c => (ValueElement)c.TdtValue);



            if (cells.Count() > 0)

            {
                Build_BSO bBso = new Build_BSO();
                List<DataItem_DTO> matrixItems = bBso.GetMatrixDataItems(this, lngIsoCode);

                foreach (var dto in matrixItems)
                {
                    List<XlsxValue> rowlist = new List<XlsxValue>
                    {
                        new XlsxValue() { Value = dto.statistic.Code },
                        new XlsxValue() { Value = dto.statistic.Value },
                        new XlsxValue() { Value = dto.period.Code },
                        new XlsxValue() { Value = dto.period.Value }
                    };
                    foreach (var cls in dto.classifications)
                    {
                        rowlist.Add(new XlsxValue() { Value = cls.Variable[0].Code });
                        rowlist.Add(new XlsxValue() { Value = cls.Variable[0].Value });
                    }
                    rowlist.Add(new XlsxValue() { Value = dto.statistic.Unit });
                    dynamic cell = Cells[cellCounter];
                    string emptyValue = indicateBlankSymbols ? Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE") : "";
                    rowlist.Add(new XlsxValue() { Value = cell.TdtValue.ToString() == Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE") ? emptyValue : cell.TdtValue.ToString(), DataType = CellValues.Number });

                    cellCounter++;
                    rowLists.Add(rowlist);
                }
            }

            return rowLists;
        }

        /// <summary>
        /// Get a matrix from a Cube DTO
        /// </summary>
        /// <param name="cubeDto"></param>
        /// <returns></returns>
        internal Matrix ApplySearchCriteria(CubeQuery_DTO cubeDto)
        {
            List<ClassificationRecordDTO_Create> filteredClassifications = new List<ClassificationRecordDTO_Create>();
            AppliedFilters = new Filters();


            foreach (var dimension in cubeDto.jStatQuery.Dimensions)
            {
                if (DimensionExistinList(dimension.Value.Id, cubeDto.Role.Metric))
                {
                    ApplyStatFilter(dimension.Value);
                }
                else if (DimensionExistinList(dimension.Value.Id, cubeDto.Role.Time))
                {
                    ApplyTimeFilter(dimension.Value);
                }
                else if (FilterHasValues(dimension.Value))
                {
                    filteredClassifications.AddRange(GetClassFilter(dimension.Value));
                }
            }


            if (filteredClassifications.Count > 1)
            {
                foreach (var filteredClassification in filteredClassifications)
                {
                    var selectedClassification = this.MainSpec.Classification.Where(c => c.Value == filteredClassification.Value && filteredClassification.Code == c.Code).First();
                    if (selectedClassification != null)
                    {
                        selectedClassification = filteredClassification;
                    }
                }
            }

            return this;
        }


        /// <summary>
        /// Get a matrix from a Cube DTO
        /// </summary>
        /// <param name="cubeDto"></param>
        /// <returns></returns>
        internal Matrix ApplySearchCriteria(Cube_DTO_Read cubeDto)
        {
            List<ClassificationRecordDTO_Create> filteredClassifications = new List<ClassificationRecordDTO_Create>();
            AppliedFilters = new Filters();

            if (cubeDto.dimension != null)
            {
                foreach (var dimension in cubeDto.dimension)
                {
                    if (DimensionExistinList(dimension, cubeDto.role.Metric))
                    {
                        ApplyStatFilter(dimension);
                    }
                    else if (DimensionExistinList(dimension, cubeDto.role.Time))
                    {
                        ApplyTimeFilter(dimension);
                    }
                    else if (FilterHasValues(dimension))
                    {
                        filteredClassifications.AddRange(GetClassFilter(dimension));
                    }
                }
            }

            if (filteredClassifications.Count > 1)
            {
                foreach (var filteredClassification in filteredClassifications)
                {
                    var selectedClassification = this.MainSpec.Classification.Where(c => c.Value == filteredClassification.Value && filteredClassification.Code == c.Code).First();
                    if (selectedClassification != null)
                    {
                        selectedClassification = filteredClassification;
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// GetClassFilter
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        private IList<ClassificationRecordDTO_Create> GetClassFilter(Dimension dimension)
        {

            IList<ClassificationRecordDTO_Create> result
                = this.MainSpec.Classification
                .Where(w => w.Code == dimension.Id).ToList();

            foreach (var c in result)
            {
                c.ClassificationFilterWasApplied = true;
                c.Variable = c.Variable
                    .Where(w => dimension.Category.Index.Value.StringArray.Contains(w.Code)).ToList();
            }
            return result;
        }

        private IList<ClassificationRecordDTO_Create> GetClassFilter(JsonQuery.Dimension dimension)
        {

            IList<ClassificationRecordDTO_Create> result
                = this.MainSpec.Classification
                .Where(w => w.Code == dimension.Id).ToList();

            foreach (var c in result)
            {
                c.ClassificationFilterWasApplied = true;
                c.Variable = c.Variable
                    .Where(w => dimension.Category.Index.Contains(w.Code)).ToList();
            }
            return result;
        }

        /// <summary>
        /// ApplyTimeFilter
        /// </summary>
        /// <param name="dimension"></param>
        private void ApplyTimeFilter(Dimension dimension)
        {
            if (FilterHasValues(dimension))
            {
                this.MainSpec.Frequency.Period =
                this.MainSpec.Frequency.Period
                    .Where(w => dimension.Category.Index.Value.StringArray.Contains(w.Code)).ToList();

                TimeFilterWasApplied = true;
            }
        }

        private void ApplyTimeFilter(JsonQuery.Dimension dimension)
        {
            if (FilterHasValues(dimension))
            {
                this.MainSpec.Frequency.Period =
                this.MainSpec.Frequency.Period
                    .Where(w => dimension.Category.Index.Contains(w.Code)).ToList();

                TimeFilterWasApplied = true;
            }
        }

        /// <summary>
        /// FilterHasValues
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        private static bool FilterHasValues(Dimension dimension)
        {
            return dimension != null
                            && dimension.Category != null
                            && dimension.Category.Index != null
                            && dimension.Category.Index.HasValue
                            && dimension.Category.Index.Value.StringArray.Count > 0;
        }

        private static bool FilterHasValues(JsonQuery.Dimension dimension)
        {
            return dimension != null
                            && dimension.Category != null
                            && dimension.Category.Index != null
                            && dimension.Category.Index.Count > 0;

        }


        /// <summary>
        /// ApplyStatFilter
        /// </summary>
        /// <param name="dimension"></param>
        private void ApplyStatFilter(Dimension dimension)
        {
            if (FilterHasValues(dimension))
            {
                this.MainSpec.Statistic =
                    this.MainSpec.Statistic
                        .Where(w => dimension.Category.Index.Value.StringArray.Contains(w.Code)).ToList();

                StatFilterWasApplied = true;
            }
        }

        private void ApplyStatFilter(JsonQuery.Dimension dimension)
        {
            if (FilterHasValues(dimension))
            {
                this.MainSpec.Statistic =
                    this.MainSpec.Statistic
                        .Where(w => dimension.Category.Index.Contains(w.Code)).ToList();

                StatFilterWasApplied = true;
            }
        }




        /// <summary>
        /// DimensionExistinList
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool DimensionExistinList(Dimension dimension, List<string> list)
        {
            return list == null ? false : list.Contains(dimension.Id);
        }

        private bool DimensionExistinList(string dimId, List<string> list)
        {
            return list == null ? false : list.Contains(dimId);
        }


        /// <summary>
        /// class property
        /// </summary>
        public Specification MainSpec { get; set; }

        /// <summary>
        /// class property
        /// </summary>
        public Filters AppliedFilters { get; set; }

        /// <summary>
        /// class property
        /// </summary>
        public IList<Specification> OtherLanguageSpec { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="language"></param>
        public Matrix(PxDocument doc, PxUpload_DTO dto = null)
        {

            this.pxUploadDto = dto;
            LoadMatrixPropertiesFromPx(doc);
        }

        public Matrix(PxDocument doc, string frqCodeTimeval, string frqValueTimeval)
        {

            //Do something here so that the TimeVal values are passed in
            LoadMatrixPropertiesFromPx(doc, frqCodeTimeval, frqValueTimeval);
        }


        /// <summary>
        /// The matrix constructor will load all the metadata from the db when instances specification
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="releaseDto"></param>
        /// <param name="language"></param>
        public Matrix(ADO ado, Release_DTO releaseDto, string language)
        {
            LoadMatrixPropertiesFromDB(ado, releaseDto, language);
        }



        ////BuildData.PxUpdate_DTO
        public Matrix(Build.BuildUpdate_DTO pxDto)
        {
            Copyright_BSO bso = new Copyright_BSO();
            this.Copyright = bso.Read(pxDto.CprCode);
        }

        public Matrix(Build_DTO pxDto)
        {
            this.Code = pxDto.matrixDto.MtrCode;
            this.CreationDate = (DateTime.Now).ToString(Utility.GetCustomConfig("APP_PX_DATE_TIME_FORMAT"));
            this.TheLanguage = pxDto.matrixDto.LngIsoCode;
            this.FormatType = Constants.C_SYSTEM_PX_NAME;
            this.IsOfficialStatistic = pxDto.matrixDto.MtrOfficialFlag;
            Copyright_BSO bso = new Copyright_BSO();
            this.Copyright = bso.Read(pxDto.CprCode);
        }

        internal void SortMatrixMetadataSpc()
        {
            this.MainSpec = SortSpecSpc(this.MainSpec);
            if (this.OtherLanguageSpec != null)
            {
                IList<Specification> otherSpecs = new List<Specification>();
                foreach (Specification spec in this.OtherLanguageSpec)
                {
                    otherSpecs.Add(SortSpecSpc(spec));
                }
                this.OtherLanguageSpec = otherSpecs;
            }


        }

        /// <summary>
        /// Ensures that the equivalent dimension codes are the same across all languages
        /// </summary>
        internal void AlignDimensionCodes()
        {
            if (this.OtherLanguageSpec == null) return;

            List<Specification> specs = new List<Specification>();
            foreach (Specification spc in this.OtherLanguageSpec)
            {
                specs.Add(AlignTwoSpecs(this.MainSpec, spc));
            }
            if (specs.Count > 0)
                this.OtherLanguageSpec = specs;

        }

        /// <summary>
        /// Aligns the dimension codes of any two dimensions
        /// </summary>
        /// <param name="refSpec"></param>
        /// <param name="changeSpec"></param>
        /// <returns></returns>
        private Specification AlignTwoSpecs(Specification refSpec, Specification changeSpec)
        {
            int i = 0;
            foreach (var cls in refSpec.Classification)
            {
                changeSpec.Classification[i].Code = cls.Code;
                int j = 0;
                foreach (var vrb in cls.Variable)
                {
                    changeSpec.Classification[i].Variable[j].Code = vrb.Code;
                    j++;
                }
                i++;
            }
            i = 0;
            foreach (var spc in refSpec.Statistic)
            {
                changeSpec.Statistic[i].Code = spc.Code;
                i++;
            }
            i = 0;
            foreach (var prd in refSpec.Frequency.Period)
            {
                changeSpec.Frequency.Period[i].Code = prd.Code;
            }
            return changeSpec;
        }

        private Specification SortSpecSpc(Specification spec)
        {
            if (spec.Statistic != null)
                spec.Statistic = spec.Statistic.OrderBy(x => x.Code).ToList<StatisticalRecordDTO_Create>();
            if (spec.Frequency != null)
            {
                if (spec.Frequency.Period != null)
                    spec.Frequency.Period = spec.Frequency.Period.OrderBy(x => x.Code).ToList<PeriodRecordDTO_Create>();
            }
            if (spec.Classification == null) return spec;
            spec.Classification = spec.Classification.OrderBy(x => x.Code).ToList<ClassificationRecordDTO_Create>();
            foreach (ClassificationRecordDTO_Create cls in spec.Classification)
            {
                if (cls.Variable != null)
                    cls.Variable = cls.Variable.OrderBy(x => x.Code).ToList<VariableRecordDTO_Create>();
            }

            return spec;
        }



        /// <summary>
        /// Get matrix properties from the database
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="releaseDto"></param>
        /// <param name="languageCode"></param>
        private void LoadMatrixPropertiesFromDB(ADO ado, Release_DTO releaseDto, string languageCode)
        {
            // start by reading the Matrx for tha release and language
            this.Copyright = new Copyright_DTO_Create();
            ReleaseCode = releaseDto.RlsCode;

            CreationDate = releaseDto.RlsLiveDatetimeFrom.ToString(Utility.GetCustomConfig("APP_PX_DATE_TIME_FORMAT"));

            var matrixDto = Matrix_ADO.GetMatrixDTO(new Matrix_ADO(ado).ReadByRelease(ReleaseCode, languageCode));
            Code = matrixDto.MtrCode;
            FormatVersion = matrixDto.FrmVersion;
            IsOfficialStatistic = matrixDto.MtrOfficialFlag;
            FormatType = matrixDto.FrmType;
            TheLanguage = matrixDto.LngIsoCode;
            Release = releaseDto;
            Copyright.CprCode = matrixDto.CprCode;
            Copyright.CprValue = matrixDto.CprValue;
            Copyright.CprUrl = matrixDto.CprUrl;

            MainSpec = new Specification(ado, matrixDto);

            var result = new ReasonRelease_ADO().Read(ado, new ReasonRelease_DTO_Read() { RlsCode = this.Release.RlsCode, LngIsoCode = languageCode });
            Reasons = new List<string>();

            if (result.hasData)
            {
                foreach (var r in result.data)
                {
                    Reasons.Add(r.RsnValueExternal);
                }
            }
        }


        /// <summary>
        /// Get a json stat object where we want to flag if each datapoint was amended. This returns an extra WasAmended property for each datapoint
        /// In cases where there are multiple Specifications (and hence multiple languages) in a Matrix, we can choose which specification to output
        /// </summary>
        /// <param name="doStatus"></param>
        /// <returns></returns>
        public JsonStat GetJsonStatObject(bool doStatus = false, bool showData = true, string lngIsoCode = null, CultureInfo ci = null)
        {

            Specification spec = new Specification();

            if (lngIsoCode != null)
            {
                if (this.MainSpec.Language.Equals(lngIsoCode))
                    spec = MainSpec;
                else
                {
                    spec = (OtherLanguageSpec.Where(x => x.Language.Equals(lngIsoCode))).FirstOrDefault();
                }
                if (spec == null) return null;
            }
            else
                spec = this.MainSpec;

            var jsStat = new JsonStat();
            jsStat.Id = new List<string>();
            jsStat.Role = new Role();
            jsStat.Size = new List<long>();


            jsStat.Version = Version.The20;
            jsStat.Class = Class.Dataset;


            if (lngIsoCode != null)
            {
                // notes
                jsStat.Note = new List<string>();
                if (!string.IsNullOrEmpty(spec.NotesAsString))
                {
                    jsStat.Note.Add(spec.NotesAsString);
                }
            }


            jsStat.Label = spec.Contents;



            // notes
            jsStat.Note = new List<string>();
            if (!string.IsNullOrEmpty(spec.NotesAsString))
            {
                jsStat.Note.Add(spec.NotesAsString);
            }

            // the release note is now appended to the other notes from the px file
            if (this.Release != null)
            {
                if (!string.IsNullOrEmpty(this.Release.CmmValue))
                {
                    jsStat.Note.Add(this.Release.CmmValue);
                }

                string urlBase = ConfigurationManager.AppSettings["APP_URL"] + "/" + Utility.GetCustomConfig("APP_COOKIELINK_RELEASE") + '/' + this.Release.RlsCode.ToString();

                jsStat.Link = new Link()
                {
                    Alternate = new List<Alternate>() {
                    new Alternate() { Href = urlBase, Type = Utility.GetCustomConfig("APP_JSON_MIMETYPE") },
                    new Alternate() { Href = urlBase, Type = Utility.GetCustomConfig("APP_PX_MIMETYPE") },
                    new Alternate() { Href = urlBase, Type = Utility.GetCustomConfig("APP_CSV_MIMETYPE") }
                }
                };

            }

            jsStat.Extension = new Dictionary<string, object>();
            jsStat.Extension.Add("matrix", this.Code);
            jsStat.Extension.Add("reasons", Reasons);

            Language_BSO languageBSO = new Language_BSO();
            Language_DTO_Create language = languageBSO.Read(spec.Language);

            jsStat.Extension.Add("language",
                new
                {
                    code = language.LngIsoCode,
                    name = language.LngIsoName
                }
                );

            if (this.Release != null)
            {
                jsStat.Extension.Add("contact",
                    new
                    {
                        name = Release.GrpContactName,
                        email = Release.GrpContactEmail,
                        phone = Release.GrpContactPhone
                    }
                );

                jsStat.Extension.Add("subject",
                    new
                    {
                        code = Release.SbjCode,
                        value = Release.SbjValue
                    });

                jsStat.Extension.Add("product",
                    new
                    {
                        code = Release.PrcCode,
                        value = Release.PrcValue
                    });
            }



            var anonymousCopyrightType = new
            {
                name = this.Copyright.CprValue,
                code = this.Copyright.CprCode,
                href = this.Copyright.CprUrl

            };
            jsStat.Extension.Add("official", this.IsOfficialStatistic);
            jsStat.Extension.Add("copyright", anonymousCopyrightType);

            if (this.Release != null)
            {

                jsStat.Extension.Add("exceptional", Release.RlsExceptionalFlag);

                jsStat.Extension.Add("reservation", Release.RlsReservationFlag);
                jsStat.Extension.Add("archive", Release.RlsArchiveFlag);
                jsStat.Extension.Add("analytical", Release.RlsAnalyticalFlag);
                jsStat.Extension.Add("dependency", Release.RlsDependencyFlag);

                jsStat.Updated = DataAdaptor.ConvertToString(Release.RlsLiveDatetimeFrom);
            }

            // cells
            if (Cells == null)
            {
                Cells = new List<dynamic>();
            }

            if (!showData)
            {
                Cells = new List<dynamic>();
            }


            jsStat.Id.Add(Utility.GetCustomConfig("APP_CSV_STATISTIC"));

            jsStat.Dimension = new Dictionary<string, Dimension>();
            var statDimension = new Dimension()
            {
                Label = spec.ContentVariable != null ? spec.ContentVariable : Label.Get("default.statistic"),
                Category = new Category()
                {
                    Index = spec.Statistic.Select(v => v.Code).ToList(),
                    Label = spec.Statistic.ToDictionary(v => v.Code, v => v.Value),
                    Unit = spec.Statistic.ToDictionary(v => v.Code, v => new Unit() { Decimals = v.Decimal, Label = v.Unit, Position = Position.End })
                }
            };

            jsStat.Dimension.Add(Utility.GetCustomConfig("APP_CSV_STATISTIC"), statDimension);
            jsStat.Role.Metric = new List<string> { Utility.GetCustomConfig("APP_CSV_STATISTIC") };

            jsStat.Size.Add(spec.Statistic.Count);

            // time role
            jsStat.Id.Add(spec.Frequency.Code);
            var timeDimension = new Dimension()
            {
                Label = spec.Frequency.Value,
                Category = new Category()
                {
                    Index = spec.Frequency.Period.Select(v => v.Code).ToList(),
                    Label = spec.Frequency.Period.ToDictionary(v => v.Code, v => v.Value)
                }
            };
            jsStat.Dimension.Add(spec.Frequency.Code, timeDimension);
            jsStat.Role.Time = new List<string> { spec.Frequency.Code };
            jsStat.Size.Add(spec.Frequency.Period.Count);

            // other dimensions
            foreach (var aDimension in spec.Classification)
            {
                var theDimension = new Dimension()
                {
                    Label = aDimension.Value,
                    Category = new Category()
                    {
                        Index = aDimension.Variable.Select(v => v.Code).ToList(),
                        Label = aDimension.Variable.ToDictionary(v => v.Code, v => v.Value)
                    }
                };

                jsStat.Dimension.Add(aDimension.Code, theDimension);
                jsStat.Id.Add(aDimension.Code);
                jsStat.Size.Add(aDimension.Variable.Count);

                if (aDimension.GeoFlag && !string.IsNullOrEmpty(aDimension.GeoUrl))
                {
                    if (jsStat.Role.Geo == null)
                    {
                        jsStat.Role.Geo = new List<string>();
                    }
                    jsStat.Role.Geo.Add(aDimension.Code);

                    theDimension.Link = new Link()
                    {
                        Enclosure = new List<Enclosure>() { new Enclosure() { Href = aDimension.GeoUrl, Type = Utility.GetCustomConfig("APP_GEO_MIMETYPE") } }
                    };
                }
            }

            if (doStatus)
            {
                jsStat.Status = new Status() { UnionArray = Cells.Select(c => c.WasAmendment ? "true" : "false").ToList() };

                List<string> theList = new List<string>();
                for (int i = 0; i < Cells.Count; ++i)
                {
                    theList.Add(i % 2 == 0 ? "true" : "false");
                }

            }


            jsStat.Value = new JsonStatValue() { AnythingArray = Cells.Select(c => (ValueElement)c.TdtValue).ToList() };

            return jsStat;
        }

        protected void LoadMatrixPropertiesFromPx(PxDocument doc, string frqCodeTimeval, string frqValueTimeval)
        {
            this.Copyright = new Copyright_DTO_Create();
            Copyright.CprValue = doc.GetStringElementValue("SOURCE");
            Copyright_BSO cBso = new Copyright_BSO();

            Copyright = cBso.ReadFromValue(Copyright.CprValue);



            FormatType = Utility.GetCustomConfig("APP_PX_DEFAULT_FORMAT");

            // The Matrix Code identifies the data cube
            Code = doc.GetStringElementValue("MATRIX");

            // As the Px Document represents a px file, this represents the version of the px format specification, typically a year where the format was introduced
            FormatVersion = doc.GetStringElementValue("AXIS-VERSION");

            string officialStat = doc.GetStringValueIfExist("OFFICIAL-STATISTICS");
            if (string.IsNullOrEmpty(officialStat))
            {
                officialStat = Utility.GetCustomConfig("APP_PX_DEFAULT_OFFICIAL_STATISTIC");
            }

            IsOfficialStatistic = officialStat.ToUpper() == Utility.GetCustomConfig("APP_PX_TRUE");

            // The data, common to all the different languages
            Cells = doc.GetData("DATA");

            // we will use this to obtain the codes of the dimensions (optional)
            var domain = doc.GetSingleElementWithSubkeysIfExist("DOMAIN");


            // The Main language of the Px File (other languages might exist)
            TheLanguage = doc.GetStringValueIfExist("LANGUAGE");

            if (String.IsNullOrEmpty(TheLanguage))
            {
                TheLanguage = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
            }



            MainSpec = new Specification(doc, domain, null, frqCodeTimeval, frqValueTimeval);

            MainSpec.Language = TheLanguage;

            Languages = doc.GetListOfElementsIfExist("LANGUAGES");

            if (Languages != null && Languages.Count > 0)
            {
                OtherLanguages = Languages.Where(t => t.SingleValue != TheLanguage).Select(t => t.SingleValue as string);

                OtherLanguageSpec = new List<Specification>();

                foreach (var otherlanguage in OtherLanguages)
                {
                    OtherLanguageSpec.Add(new Specification(doc, domain, otherlanguage, frqCodeTimeval, frqValueTimeval));
                }
            }
        }


        /// <summary>
        /// Load the matrix from the px file
        /// </summary>
        /// <param name="doc"></param>
        protected void LoadMatrixPropertiesFromPx(PxDocument doc)
        {
            this.Copyright = new Copyright_DTO_Create();
            FormatType = Utility.GetCustomConfig("APP_PX_DEFAULT_FORMAT");

            // The Matrix Code identifies the data cube
            Code = doc.GetStringElementValue("MATRIX");

            Copyright.CprValue = doc.GetStringElementValue("SOURCE");
            Copyright_BSO cBso = new Copyright_BSO();
            Copyright = cBso.ReadFromValue(Copyright.CprValue);



            // As the Px Document represents a px file, this represents the version of the px format specification, typically a year where the format was introduced
            FormatVersion = doc.GetStringElementValue("AXIS-VERSION");

            string officialStat = doc.GetStringValueIfExist("OFFICIAL-STATISTICS");
            if (string.IsNullOrEmpty(officialStat))
            {
                officialStat = Utility.GetCustomConfig("APP_PX_DEFAULT_OFFICIAL_STATISTIC");
            }

            IsOfficialStatistic = officialStat.ToUpper() == Utility.GetCustomConfig("APP_PX_TRUE");

            // The data, common to all the different languages
            Cells = doc.GetData("DATA");

            // we will use this to obtain the codes of the dimensions (optional)
            var domain = doc.GetSingleElementWithSubkeysIfExist("DOMAIN");


            // The Main language of the Px File (other languages might exist)
            TheLanguage = doc.GetStringValueIfExist("LANGUAGE");

            if (String.IsNullOrEmpty(TheLanguage))
            {
                TheLanguage = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
            }



            MainSpec = new Specification(doc, domain, pxUploadDto);

            MainSpec.Language = TheLanguage;

            Languages = doc.GetListOfElementsIfExist("LANGUAGES");

            if (Languages != null && Languages.Count > 0)
            {
                OtherLanguages = Languages.Where(t => t.SingleValue != TheLanguage).Select(t => t.SingleValue as string);

                OtherLanguageSpec = new List<Specification>();

                foreach (var otherlanguage in OtherLanguages)
                {
                    OtherLanguageSpec.Add(new Specification(doc, domain, otherlanguage, pxUploadDto));
                }
            }
        }


        public ValidationResult ValidationResult;
        /// <summary>
        /// class property
        /// </summary>
        public bool IsOfficialStatistic { get; internal set; }

        public Copyright_DTO_Create Copyright { get; set; }

        /// <summary>
        /// class property
        /// </summary>
        public string Code { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public string FormatType { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public string FormatVersion { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public IList<dynamic> Cells { get; internal set; }

        public IList<KeyValuePair<string, IList<IPxSingleElement>>> Domain { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public string TheLanguage { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public IList<IPxSingleElement> Languages { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public IEnumerable<string> OtherLanguages { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public int ReleaseCode { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public string CreationDate { get; private set; }

        /// <summary>
        /// class property
        /// </summary>
        public bool StatFilterWasApplied { get; internal set; }

        /// <summary>
        /// class property
        /// </summary>
        public bool TimeFilterWasApplied { get; internal set; }

        /// <summary>
        /// Tests if the language exists in the system
        /// </summary>
        internal static bool LanguageIsSupported(ADO ado, string language)
        {
            if (String.IsNullOrEmpty(language))
                return false;
            var languageAdo = new Language_ADO(ado);
            return languageAdo.Exists(language);
        }

        /// <summary>
        /// Tests if the px version is supported
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="type"></param>
        /// <param name="axisVersion"></param>
        /// <returns></returns>
        internal static bool AxisVersionIsSupported(ADO ado, string type, string axisVersion)
        {
            var supportedAxisVersion = SettingsData.ReadFormatRecord(ado, type);//, axisVersion);

            var version = supportedAxisVersion.Where(v => v.Version.Equals(axisVersion, StringComparison.InvariantCultureIgnoreCase) && v.Type.Equals(type, StringComparison.InvariantCultureIgnoreCase));

            int versionInt = 0;
            if (int.TryParse(axisVersion, out versionInt))
            {
                var maxVersion = supportedAxisVersion.Max(x => x.Version);
                if (int.TryParse(maxVersion, out versionInt))
                {
                    if (Convert.ToInt32(axisVersion) <= Convert.ToInt32(maxVersion))
                        return true;
                    else
                        return false;
                }
            }


            return false;
        }

        internal static bool JsonStatVersionIsSupported(ADO ado, string type, string axisVersion)
        {
            var supportedAxisVersion = SettingsData.ReadFormatRecord(ado, type);

            var version = supportedAxisVersion.Where(v => v.Version.Equals(axisVersion, StringComparison.InvariantCultureIgnoreCase) && v.Type.Equals(type, StringComparison.InvariantCultureIgnoreCase));

            if (version.FirstOrDefault().Type == type && version.FirstOrDefault().Version == axisVersion)
                return true;


            return false;
        }

        /// <summary>
        /// tests if the source (Copyright) is supported
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static bool SourceIsSupported(ADO ado, string source)
        {

            var supportedSources = SettingsData.ReadCopyrightRecord(ado, source);

            var matchedSources = supportedSources.Where(v => v.CprValue.Equals(source, StringComparison.InvariantCultureIgnoreCase));

            return matchedSources.Any();

        }


    }

    public class Signature_DTO
    {
        public string MtrInput { get; set; }
        public string FrqValueTimeval { get; set; }
        public string FrqCodeTimeval { get; set; }


    }

    /// <summary>
    /// class
    /// </summary>
    public class PxUpload_DTO
    {
        [NoTrim]
        [NoHtmlStrip]
        public string MtrInput { get; set; }
        public bool Overwrite { get; set; }
        public string GrpCode { get; set; }
        public string Signature { get; set; }
        public string CprCode { get; set; }
        public string FrqValueTimeval { get; set; }
        public string FrqCodeTimeval { get; set; }
        public string LngIsoCode { get; set; }

        internal Signature_DTO GetSignatureDTO()
        {
            return new Signature_DTO
            {
                MtrInput = this.MtrInput,
                FrqCodeTimeval = this.FrqCodeTimeval,
                FrqValueTimeval = this.FrqValueTimeval

            };



        }



        /// <summary>
        /// Used for serialization/deserialization
        /// </summary>
        public PxUpload_DTO() { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parameters"></param>
        public PxUpload_DTO(dynamic parameters)
        {
            GrpCode = parameters["GrpCode"];

            if (parameters["MtrInput"] != null)
            {
                MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);
            }

            if (parameters.Overwrite != null)
                this.Overwrite = parameters.Overwrite;

            if (parameters.Signature != null)
                this.Signature = parameters.Signature;

            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;
            else
                this.CprCode = Utility.GetCustomConfig("APP_DEFAULT_SOURCE");

            if (parameters.FrqValueTimeval != null)
                this.FrqValueTimeval = parameters.FrqValueTimeval;

            if (parameters.FrqCodeTimeval != null)
                this.FrqCodeTimeval = parameters.FrqCodeTimeval;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
        }
    }

    /// <summary>
    /// class
    /// </summary>
    internal class Matrix_DTO_Read
    {
        public string MtrCode { get; set; }

        public int RlsCode { get; set; }

        public string LngIsoCode { get; set; }

        public Matrix_DTO_Read(dynamic parameters)
        {

            if (parameters.MtrCode != null)
                this.MtrCode = parameters.MtrCode;

            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");

        }
    }

    internal class Matrix_DTO_ReadByProduct
    {
        public string PrcCode { get; internal set; }
        public string LngIsoCode { get; internal set; }



        public Matrix_DTO_ReadByProduct(dynamic parameters)
        {
            if (parameters.PrcCode != null)
                PrcCode = parameters.PrcCode;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
        }
    }

    /// <summary>
    /// class
    /// </summary>
    internal class Matrix_DTO
    {
        public string MtrCode { get; set; }
        public string MtrTitle { get; set; }
        public string LngIsoCode { get; set; }
        public string CprValue { get; set; }
        public bool MtrOfficialFlag { get; set; }
        public string FrmType { get; set; }
        public string FrmVersion { get; set; }
        public string MtrNote { get; set; }
        public string MtrInput { get; internal set; }
        public string FrqCode { get; internal set; }
        public string FrqValue { get; internal set; }
        public string CprCode { get; internal set; }
        public string LngIsoName { get; internal set; }
        public int RlsCode { get; internal set; }
        public DateTime DtgUpdateDatetime { get; internal set; }
        public string CcnUsernameUpdate { get; internal set; }
        public DateTime DtgCreateDatetime { get; internal set; }
        public string CcnUsername { get; internal set; }
        public string CprUrl { get; internal set; }
    }


    /// <summary>
    /// class
    /// </summary>
    internal class Statistic_DTO
    {
        public List<StatisticalRecordDTO_Create> Statistic { get; set; }
        public Statistic_DTO(dynamic parameters)
        {
            if (parameters.Statistic != null)
            {
                Statistic = new List<StatisticalRecordDTO_Create>();
                foreach (var stat in parameters.Statistic)
                {
                    Statistic.Add(new StatisticalRecordDTO_Create(stat));
                }
            }
        }
    }

    /// <summary>
    /// class
    /// </summary>
    public class StatisticalRecordDTO_Create : IEquatable<StatisticalRecordDTO_Create>
    {

        public int StatisticalProductId { get; set; }

        public string Value { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }

        public short Decimal { get; set; }

        public StatisticalRecordDTO_Create(dynamic stat)
        {
            if (stat.SttCode != null)
                this.Code = stat.SttCode;
            if (stat.SttValue != null)
                this.Value = stat.SttValue;
            if (stat.SttUnit != null)
                this.Unit = stat.SttUnit;
            if (stat.SttDecimal != null)
            {
                int check = stat.SttDecimal;
                if (check > 32767 || check < -32767)
                {
                    throw new FormatException();
                }
                this.Decimal = stat.SttDecimal;
            }
        }

        /// <summary>
        /// blank constructor
        /// </summary>
        public StatisticalRecordDTO_Create() { }

        /// <summary>
        /// Tests if two statistic records have the same Decimal and Unit
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        internal bool IsEquivalent(StatisticalRecordDTO_Create comp)
        {

            if (this.Decimal != comp.Decimal) return false;
            if (this.Unit != comp.Unit) return false;
            return true;
        }


        /// <summary>
        /// Tests if two statistical records are entirely similar
        /// </summary>
        /// <param name="otherStatistic"></param>
        /// <returns></returns>
        public Boolean Equals(StatisticalRecordDTO_Create otherStatistic)
        {
            if (!otherStatistic.GetType().Equals(typeof(StatisticalRecordDTO_Create))) return false;
            var other = (StatisticalRecordDTO_Create)otherStatistic;
            if (!IsEquivalent(other)) return false;
            if (!this.Value.Equals(other.Value)) return false;

            return true;
        }

        /// <summary>
        /// Get the hash for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Get the hash codes
            int hashValue = this.Value.GetHashCode();
            // Calculate the hash code for the object.
            return hashValue;
        }
    }

    /// <summary>
    /// Class
    /// </summary>
    public class ClassificationRecordDTO_Create : IEquatable<ClassificationRecordDTO_Create>
    {
        public int ClassificationId { get; set; }

        public string Value { get; set; }
        public string Code { get; set; }

        public bool GeoFlag { get; set; }

        public string GeoUrl { get; set; }

        public List<VariableRecordDTO_Create> Variable;

        public ClassificationRecordDTO_Create() { }
        public ClassificationRecordDTO_Create(dynamic cls)
        {
            if (cls.ClsCode != null)
                this.Code = cls.ClsCode;
            if (cls.ClsValue != null)
                this.Value = cls.ClsValue;
            if (cls.ClsGeoUrl != null)
            {
                if (cls.ClsGeoUrl != "")
                    this.GeoFlag = true;
                if (this.GeoFlag)
                    this.GeoUrl = cls.ClsGeoUrl;
            }

            if (cls.Variable != null)
            {
                this.Variable = new List<VariableRecordDTO_Create>();
                foreach (var vrb in cls.Variable)
                {
                    VariableRecordDTO_Create vrc = new VariableRecordDTO_Create(vrb);

                    this.Variable.Add(vrc);
                }

            }


        }

        /// <summary>
        /// Tests if two Classifications are entirely equal in terms of code and value
        /// </summary>
        /// <param name="otherClassification"></param>
        /// <returns></returns>
        public Boolean Equals(ClassificationRecordDTO_Create otherClassification)
        {
            if (!otherClassification.GetType().Equals(typeof(ClassificationRecordDTO_Create))) return false;
            var other = (ClassificationRecordDTO_Create)otherClassification;
            if (!this.Value.Equals(other.Value) || !this.Code.Equals(other.Code)) return false;

            return true;
        }



        public override int GetHashCode()
        {
            // Get the hash codes
            int hashValue = this.Value.GetHashCode();
            int hashCode = this.Code.GetHashCode();
            // Calculate the hash code for the object.
            return hashValue ^ hashCode;
        }


        public bool ClassificationFilterWasApplied { get; set; }
        /// <summary>
        /// Tests if the language invariant parts of a ClassificationRecordDTO_Create are equal
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        internal bool IsEquivalent(ClassificationRecordDTO_Create comp)
        {
            if (this.Code != comp.Code) return false;

            //Create two lists with just the variable codes
            var q1 = from vars in this.Variable select vars.Code;
            var q2 = from vars in comp.Variable select vars.Code;

            //Is there a difference between the two variable lists? If so, return false;
            int interCount = (q1.Intersect(q2)).Count();
            if (interCount != q1.Count() && interCount != q2.Count()) return false;

            return true;
        }




    }


    /// <summary>
    /// class
    /// </summary>
    public class VariableRecordDTO_Create : IEquatable<VariableRecordDTO_Create>
    {


        public int ClassificationVariableId { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }

        public int Id { get; set; }

        public VariableRecordDTO_Create() { }
        public VariableRecordDTO_Create(dynamic vrb)
        {
            if (vrb.VrbCode != null)
                this.Code = vrb.VrbCode;
            if (vrb.VrbValue != null)
                this.Value = vrb.VrbValue;
        }

        /// <summary>
        /// Get a hash code for the object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Get the hash codes
            int hashValue = this.Value.GetHashCode();
            int hashCode = this.Code.GetHashCode();
            // Calculate the hash code for the object.
            return hashValue ^ hashCode;
        }


        /// <summary>
        /// Tests if two variables are equal in terms of code and value
        /// </summary>
        /// <param name="otherVariable"></param>
        /// <returns></returns>
        public Boolean Equals(VariableRecordDTO_Create otherVariable)
        {
            if (!otherVariable.GetType().Equals(typeof(VariableRecordDTO_Create))) return false;
            var other = (VariableRecordDTO_Create)otherVariable;
            return this.Value.Equals(other.Value) && this.Code.Equals(other.Code);
        }
    }

    /// <summary>
    /// class
    /// </summary>
    public class FrequencyRecordDTO_Create : IEquatable<FrequencyRecordDTO_Create>
    {
        public List<PeriodRecordDTO_Create> Period { get; set; }

        public int FrequencyId { get; set; }

        public string Value { get; set; }

        public string Code { get; set; }


        /// <summary>
        /// Get a hash for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Get the hash codes
            int hashValue = this.Value.GetHashCode();
            int hashCode = this.Code.GetHashCode();
            // Calculate the hash code for the object.
            return hashValue ^ hashCode;
        }

        /// <summary>
        /// Tests if two frequencies are equal in terms of code and value
        /// </summary>
        /// <param name="otherFrequency"></param>
        /// <returns></returns>
        public Boolean Equals(FrequencyRecordDTO_Create otherFrequency)
        {
            if (!otherFrequency.GetType().Equals(typeof(FrequencyRecordDTO_Create))) return false;
            var other = (FrequencyRecordDTO_Create)otherFrequency;
            return this.Value.Equals(other.Value) && this.Code.Equals(other.Code);
        }
    }

    /// <summary>
    /// class
    /// </summary>
    public class PeriodRecordDTO_Create : IEquatable<PeriodRecordDTO_Create>
    {
        public int FrequencyPeriodId { get; set; }

        public string Value { get; set; }
        public string Code { get; set; }


        /// <summary>
        /// Get a hash of this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Get the hash codes
            int hashValue = this.Value.GetHashCode();
            int hashCode = this.Code.GetHashCode();
            // Calculate the hash code for the object.
            return hashValue ^ hashCode;
        }

        /// <summary>
        /// Tests if two periods are equal in terms of code and value
        /// </summary>
        /// <param name="otherPeriod"></param>
        /// <returns></returns>
        public Boolean Equals(PeriodRecordDTO_Create otherPeriod)
        {
            if (!otherPeriod.GetType().Equals(typeof(PeriodRecordDTO_Create))) return false;
            var other = (PeriodRecordDTO_Create)otherPeriod;
            return this.Value.Equals(other.Value) && this.Code.Equals(other.Code);
        }


    }

    /// <summary>
    /// class
    /// </summary>
    public class Matrix_DTO_ReadHistory
    {
        public DateTime DateFrom { get; internal set; }
        public DateTime DateTo { get; internal set; }
        public string LngIsoCode { get; internal set; }

        public Matrix_DTO_ReadHistory(dynamic parameters)
        {
            if (parameters.DateFrom != null)
                DateFrom = parameters.DateFrom;
            if (parameters.DateTo != null)
                DateTo = parameters.DateTo;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
        }
    }
}
