using API;
using PxStat.Data;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Build
{
    /// <summary>
    /// 
    /// </summary>
    public class Build_DTO
    {
        /// <summary>
        /// 
        /// </summary>
        internal IList<Dimension_DTO> DimensionList { get; set; }

        internal FrequencyRecordDTO_Create Frequency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal Matrix_DTO matrixDto { get; set; }

        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        [LowerCase]
        internal string LngIsoCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string MtrCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal bool MtrOfficialFlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string MtrInput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string CprCode { get; set; }

        /// <summary>
        /// This is the frequency code that will be used in the modified metadata
        /// </summary>
        internal string FrqCode { get; set; }

        /// <summary>
        /// This is required where there is no TIMEVAL in the matrix and the user has nominated a dimension that will serve as a time dimension
        /// The FrqValue will be the name of the chosen dimension and if a FrqCode has not been supplied, a code will be generated.
        /// </summary>
        internal string FrqValue { get; set; }

        /// <summary>
        /// Statistic Value
        /// </summary>
        internal string SttValue { get; set; }

        internal Format_DTO_Read Format { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public Build_DTO(dynamic parameters)
        {
            if (parameters.MtrInput != null)
                this.MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);

            this.LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");


            if (parameters.MtrOfficialFlag != null)
                this.MtrOfficialFlag = parameters.MtrOfficialFlag;
            else this.MtrOfficialFlag = Utility.GetCustomConfig("APP_PX_DEFAULT_OFFICIAL_STATISTIC").ToUpper() == Utility.GetCustomConfig("APP_PX_TRUE").ToUpper();

            if (parameters.MtrCode != null)
                this.MtrCode = parameters.MtrCode;
            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;
            if (parameters.FrqCode != null)
                this.FrqCode = parameters.FrqCode;
            if (parameters.FrqValue != null)
                this.FrqValue = parameters.FrqValue;
            Format = new Format_DTO_Read();
            if (parameters.FrmType != null && parameters.FrmVersion != null)
            {
                Format = new Format_DTO_Read
                {
                    FrmVersion = parameters.FrmVersion,
                    FrmType = parameters.FrmType,
                    FrmDirection = Format_DTO_Read.FormatDirection.UPLOAD.ToString()
                };
            }



            if (parameters.Dimension != null)
            {
                matrixDto = new Matrix_DTO();
                if (parameters.LngIsoCode != null)
                    matrixDto.LngIsoCode = parameters.LngIsoCode;
                else
                    matrixDto.LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");

                if (parameters.MtrCode != null)
                    matrixDto.MtrCode = parameters.MtrCode;
                if (parameters.MtrOfficialFlag != null)
                    matrixDto.MtrOfficialFlag = parameters.MtrOfficialFlag;

                DimensionList = new List<Dimension_DTO>();

                foreach (var dim in parameters.Dimension)
                {
                    Dimension_DTO mlDto = new Dimension_DTO(dim);

                    if (dim.MtrTitle != null)
                    {
                        mlDto.Contents = dim.MtrTitle;
                        if (mlDto.Classifications != null)
                            mlDto.MtrTitle = mlDto.Contents + " (" + String.Join(", ", ((mlDto.Classifications.Select(c => c.Value.ToString())).ToList<string>()).ToArray()) + ')';
                        else
                            mlDto.MtrTitle = dim.MtrTitle;

                        if (mlDto.MtrTitle.Length > 256) mlDto.MtrTitle = mlDto.MtrTitle.Substring(0, 256);
                    }

                    if (this.CprCode != null)
                    {
                        mlDto.CprCode = this.CprCode;
                        Copyright_BSO cBso = new Copyright_BSO();
                        Copyright_DTO_Create cDTO = cBso.Read(this.CprCode);
                        mlDto.CprValue = cDTO.CprValue;
                    }
                    if (dim.CprValue != null)
                        mlDto.CprValue = dim.CprValue;

                    if (dim.Frequency != null)
                    {
                        mlDto.Frequency = new FrequencyRecordDTO_Create();

                        if (parameters.FrqCode != null)
                            mlDto.Frequency.Code = parameters.FrqCode;
                        if (dim.Frequency.FrqValue != null)
                            mlDto.Frequency.Value = dim.Frequency.FrqValue;

                        if (dim.Frequency.Period != null)
                        {
                            mlDto.Frequency.Period = new List<PeriodRecordDTO_Create>();
                            foreach (var per in dim.Frequency.Period)
                            {
                                PeriodRecordDTO_Create period = new Data.PeriodRecordDTO_Create();
                                if (per.PrdCode != null)
                                    period.Code = per.PrdCode;
                                if (per.PrdValue != null)
                                    period.Value = per.PrdValue;
                                mlDto.Frequency.Period.Add(period);
                            }
                        }

                    }

                    if (this.FrqCode != null)
                    {
                        mlDto.FrqCode = this.FrqCode;
                        //Get Frequency value
                        Frequency_BSO bso = new Frequency_BSO();
                        Frequency_DTO dto = new Frequency_DTO();
                        dto = bso.Read(this.FrqCode);
                        mlDto.FrqValue = dto.FrqValue;
                    }

                    if (dim.FrqCode != null)
                        mlDto.FrqCode = dim.FrqCode;
                    if (dim.FrqValue != null)
                        mlDto.FrqValue = dim.FrqValue;

                    if (dim.LngIsoCode != null)
                        mlDto.LngIsoCode = dim.LngIsoCode;

                    if (dim.MtrTitle != null)
                        mlDto.MtrTitle = dim.MtrTitle;
                    if (dim.MtrNote != null)
                        mlDto.MtrNote = dim.MtrNote;


                    if (dim.Statistic != null)
                    {
                        mlDto.Statistics = new List<StatisticalRecordDTO_Create>();

                        foreach (var stat in dim.Statistic)
                        {
                            StatisticalRecordDTO_Create src = new StatisticalRecordDTO_Create(stat);

                            mlDto.Statistics.Add(src);
                        }
                    }


                    DimensionList.Add(mlDto);


                }

            }
        }

    }




    /// <summary>
    /// 
    /// </summary>
    public class Build_DTO_BuildRead
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        [NoTrim]
        [NoHtmlStrip]
        public string MtrInput { get; internal set; }



        /// <summary>
        /// 
        /// </summary>
        public string LngIsoCode { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<Dimension_DTO> Dimension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// 

        public List<PeriodRecordDTO_Create> Periods { get; internal set; }

        public string MtrCode { get; set; }

        public string CprCode { get; set; }


        public bool MtrOfficialFlag { get; set; }

        /// <summary>
        /// This is the FrqCode that we are changing
        /// </summary>
        public string FrqCodeTimeval { get; set; }
        /// <summary>
        /// This is required where there is no TIMEVAL in the matrix and the user has nominated a dimension that will serve as a time dimension
        /// The FrqValue will be the name of the chosen dimension and if a FrqCode has not been supplied, a code will be generated.
        /// For Build, this must be considered distinct from the FrqValue in Dimension
        /// </summary>
        public string FrqValueTimeval { get; set; }

        public Format_DTO_Read Format { get; set; }

        public string Signature { get; set; }


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
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public Build_DTO_BuildRead(dynamic parameters)
        {
            if (parameters.MtrInput != null)
                this.MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);


            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
            Periods = new List<PeriodRecordDTO_Create>();

            if (parameters.MtrCode != null)
                MtrCode = parameters.MtrCode;
            if (parameters.CprCode != null)
            {
                CprCode = parameters.CprCode;

            }
            if (parameters.FrqCodeTimeval != null)
                FrqCodeTimeval = parameters.FrqCodeTimeval;
            if (parameters.FrqValueTimeval != null)
                FrqValueTimeval = parameters.FrqValueTimeval;
            if (parameters.MtrOfficialFlag != null)
                MtrOfficialFlag = parameters.MtrOfficialFlag;

            Format = new Format_DTO_Read();
            if (parameters.FrmType != null && parameters.FrmVersion != null)
            {
                Format.FrmType = parameters.FrmType;
                Format.FrmVersion = parameters.FrmVersion;
                Format.FrmDirection = Format_DTO_Read.FormatDirection.UPLOAD.ToString();
            }
            if (parameters.FrmType != null)

                if (parameters.FrmVersion != null)

                    if (parameters.Signature != null)
                        this.Signature = parameters.Signature;



            if (parameters.Dimension != null)
            {
                Dimension = new List<Dimension_DTO>();
                foreach (var dim in parameters.Dimension)
                {
                    Dimension_DTO dimension = new Dimension_DTO();
                    if (dim.Frequency != null)
                    {
                        dimension.Frequency = new FrequencyRecordDTO_Create();
                        if (dim.Frequency.Period != null)
                        {
                            dimension.Frequency.Period = new List<Data.PeriodRecordDTO_Create>();
                            foreach (var per in dim.Frequency.Period)
                            {
                                PeriodRecordDTO_Create period = new PeriodRecordDTO_Create();
                                if (per.PrdCode != null) period.Code = per.PrdCode;
                                if (per.PrdValue != null) period.Value = per.PrdValue;
                                if (Periods.Where(x => x.Code.Equals(period.Code)).FirstOrDefault() == null)
                                    Periods.Add(period);
                                dimension.Frequency.Period.Add(period);
                            }
                        }
                        if (dim.Frequency.FrqValue != null)
                        {
                            dimension.Frequency.Value = dim.Frequency.FrqValue;
                        }


                    }
                    if (dim.Classification != null)
                    {
                        dimension.Classifications = new List<ClassificationRecordDTO_Create>();
                        foreach (var cls in dim.Classification)
                        {
                            ClassificationRecordDTO_Create classification = new ClassificationRecordDTO_Create();
                            if (cls.ClsCode != null) classification.Code = cls.ClsCode;
                            if (cls.ClsGeoUrl != null) classification.GeoUrl = cls.ClsGeoUrl;
                            dimension.Classifications.Add(classification);
                        }
                    }
                    if (dim.LngIsoCode != null)
                        dimension.LngIsoCode = dim.LngIsoCode;
                    if (dim.MtrTitle != null)
                        dimension.MtrTitle = dim.Dimension;
                    if (dim.FrqValue != null)
                        dimension.FrqValue = dim.FrqValue;
                    if (dim.MtrNote != null)
                        dimension.MtrNote = dim.MtrNote;
                    if (dim.StatisticLabel != null)
                        dimension.StatisticLabel = dim.StatisticLabel;
                    if (dim.MtrTitle != null)
                        dimension.MtrTitle = dim.MtrTitle;

                    Dimension.Add(dimension);
                }
            }


        }

        public Build_DTO_BuildRead()
        {
        }

        /// <summary>
        /// Gets the specific dto dimension for a language
        /// </summary>
        /// <param name="lngIsoCode"></param>
        /// <returns></returns>
        internal Dimension_DTO getDimensionForLanguage(string lngIsoCode)
        {
            foreach (Dimension_DTO dim in this.Dimension)
            {
                if (dim.LngIsoCode == lngIsoCode)
                    return dim;
            }
            return null;
        }
    }





    /// <summary>
    /// 
    /// </summary>
    public class BuildUpdate_DTO
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        [NoTrim]
        [NoHtmlStrip]
        public string MtrInput { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public PxData_DTO PxData { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string LngIsoCode { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<Dimension_DTO> Dimension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// 

        public List<PeriodRecordDTO_Create> Periods { get; internal set; }

        public string MtrCode { get; set; }

        public string CprCode { get; set; }


        public bool MtrOfficialFlag { get; set; }

        /// <summary>
        /// This is the FrqCode that we are changing
        /// </summary>
        public string FrqCodeTimeval { get; set; }
        /// <summary>
        /// This is required where there is no TIMEVAL in the matrix and the user has nominated a dimension that will serve as a time dimension
        /// The FrqValue will be the name of the chosen dimension and if a FrqCode has not been supplied, a code will be generated.
        /// For Build, this must be considered distinct from the FrqValue in Dimension
        /// </summary>
        public string FrqValueTimeval { get; set; }

        public Format_DTO_Read Format { get; set; }

        public string Signature { get; set; }


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
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public BuildUpdate_DTO(dynamic parameters)
        {
            if (parameters.MtrInput != null)
                this.MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);
            if (parameters.Data != null)
            {
                PxData = new PxData_DTO(parameters);
            }

            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
            Periods = new List<PeriodRecordDTO_Create>();

            if (parameters.MtrCode != null)
                MtrCode = parameters.MtrCode;
            if (parameters.CprCode != null)
            {
                CprCode = parameters.CprCode;

            }
            if (parameters.FrqCodeTimeval != null)
                FrqCodeTimeval = parameters.FrqCodeTimeval;
            if (parameters.FrqValueTimeval != null)
                FrqValueTimeval = parameters.FrqValueTimeval;
            if (parameters.MtrOfficialFlag != null)
                MtrOfficialFlag = parameters.MtrOfficialFlag;

            Format = new Format_DTO_Read();
            if (parameters.FrmType != null && parameters.FrmVersion != null)
            {
                Format.FrmType = parameters.FrmType;
                Format.FrmVersion = parameters.FrmVersion;
                Format.FrmDirection = Format_DTO_Read.FormatDirection.UPLOAD.ToString();
            }
            // if (parameters.FrmType != null)

            //    if (parameters.FrmVersion != null)

            if (parameters.Signature != null)
                this.Signature = parameters.Signature;



            if (parameters.Dimension != null)
            {
                Dimension = new List<Dimension_DTO>();
                foreach (var dim in parameters.Dimension)
                {
                    Dimension_DTO dimension = new Dimension_DTO();
                    if (dim.Frequency != null)
                    {
                        dimension.Frequency = new FrequencyRecordDTO_Create();
                        if (dim.Frequency.Period != null)
                        {
                            dimension.Frequency.Period = new List<Data.PeriodRecordDTO_Create>();
                            foreach (var per in dim.Frequency.Period)
                            {
                                PeriodRecordDTO_Create period = new PeriodRecordDTO_Create();
                                if (per.PrdCode != null) period.Code = per.PrdCode;
                                if (per.PrdValue != null) period.Value = per.PrdValue;
                                if (Periods.Where(x => x.Code.Equals(period.Code)).FirstOrDefault() == null)
                                    Periods.Add(period);
                                dimension.Frequency.Period.Add(period);
                            }
                        }
                        if (dim.Frequency.FrqValue != null)
                        {
                            dimension.Frequency.Value = dim.Frequency.FrqValue;
                        }


                    }
                    if (dim.Classification != null)
                    {
                        dimension.Classifications = new List<ClassificationRecordDTO_Create>();
                        foreach (var cls in dim.Classification)
                        {
                            ClassificationRecordDTO_Create classification = new ClassificationRecordDTO_Create();
                            if (cls.ClsCode != null) classification.Code = cls.ClsCode;
                            if (cls.ClsGeoUrl != null) classification.GeoUrl = cls.ClsGeoUrl;
                            dimension.Classifications.Add(classification);
                        }
                    }
                    if (dim.LngIsoCode != null)
                        dimension.LngIsoCode = dim.LngIsoCode;
                    if (dim.MtrTitle != null)
                        dimension.MtrTitle = dim.Dimension;
                    if (dim.FrqValue != null)
                        dimension.FrqValue = dim.FrqValue;
                    if (dim.MtrNote != null)
                        dimension.MtrNote = dim.MtrNote;
                    if (dim.StatisticLabel != null)
                        dimension.StatisticLabel = dim.StatisticLabel;
                    if (dim.MtrTitle != null)
                        dimension.MtrTitle = dim.MtrTitle;

                    Dimension.Add(dimension);
                }
            }


        }

        public BuildUpdate_DTO()
        {
        }

        /// <summary>
        /// Gets the specific dto dimension for a language
        /// </summary>
        /// <param name="lngIsoCode"></param>
        /// <returns></returns>
        internal Dimension_DTO getDimensionForLanguage(string lngIsoCode)
        {
            foreach (Dimension_DTO dim in this.Dimension)
            {
                if (dim.LngIsoCode == lngIsoCode)
                    return dim;
            }
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PxPeriodListDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public List<PeriodRecordDTO_Create> Periods { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public PxPeriodListDTO(dynamic parameters)
        {
            Periods = new List<PeriodRecordDTO_Create>();
            if (parameters.Period != null)
            {

                foreach (var p in parameters.Period)
                {
                    PeriodRecordDTO_Create period = new PeriodRecordDTO_Create();
                    if (p.PrdCode != null) period.Code = p.PrdCode;
                    if (p.PrdValue != null) period.Value = p.PrdValue;
                    Periods.Add(period);
                }
            }
        }
    }

    /// <summary>
    /// PxData_DTO
    /// </summary>
    public class PxData_DTO
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Rows { get; internal set; }
        internal List<dynamic> DataItems { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        internal PxData_DTO(dynamic parameters)
        {
            if (parameters.Data != null)
            {
                DataItems = new List<dynamic>();
                foreach (var v in parameters.Data)
                {
                    DataItems.Add(v);

                }

            }
        }

        internal PxData_DTO() { }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PxDataItem_DTO
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class Dimension_DTO
    {
        /// <summary>
        /// 
        /// </summary>
        public string FrqCode { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string FrqValue { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [LowerCase]
        public string LngIsoCode { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string MtrTitle { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string MtrNote { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string CprCode { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string CprValue { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Contents { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string StatisticLabel { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ClassificationRecordDTO_Create> Classifications { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public List<StatisticalRecordDTO_Create> Statistics { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public List<PeriodRecordDTO_Create> Periods { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public FrequencyRecordDTO_Create Frequency { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dim"></param>
        public Dimension_DTO(dynamic dim)
        {
            if (dim.FrqCode != null)
                this.FrqCode = dim.FrqCode;
            if (dim.FrqValue != null)
                this.FrqValue = dim.FrqValue;

            if (dim.Frequency != null)
            {
                Frequency = new FrequencyRecordDTO_Create();
                if (dim.Frequency.FrqCode != null)
                {
                    this.FrqCode = dim.Frequency.FrqCode;
                    Frequency.Code = dim.Frequency.FrqCode;
                }
                if (dim.Frequency.FrqValue != null)
                {
                    this.FrqValue = dim.Frequency.FrqValue;
                    Frequency.Value = dim.Frequency.FrqValue;
                }
                if (dim.Frequency.Period != null)
                {
                    Frequency.Period = new List<PeriodRecordDTO_Create>();
                    foreach (dynamic per in dim.Frequency.Period)
                    {
                        PeriodRecordDTO_Create prd = new PeriodRecordDTO_Create();
                        if (per.PrdCode != null)
                            prd.Code = per.PrdCode;
                        if (per.PrdValue != null)
                            prd.Value = prd.Code;
                        Frequency.Period.Add(prd);

                    }
                }
            }

            //Sometimes Period as a collection of periods may exist at the top level - e.g. PxFileBuild_API.Update
            if (dim.Period != null)
            {
                Periods = new List<PeriodRecordDTO_Create>();
                foreach (dynamic per in dim.Period)
                {
                    PeriodRecordDTO_Create prd = new PeriodRecordDTO_Create();
                    if (per.PrdCode != null)
                        prd.Code = per.PrdCode;
                    if (per.PrdValue != null)
                        prd.Value = prd.Code;
                    Periods.Add(prd);

                }
            }

            if (dim.StatisticLabel != null)
                this.StatisticLabel = dim.StatisticLabel;

            if (dim.CprValue != null)
            {
                this.CprValue = dim.CprValue;
            }

            if (dim.LngIsoCode != null)
                this.LngIsoCode = dim.LngIsoCode;

            if (dim.MtrNote != null)
                this.MtrNote = dim.MtrNote;

            if (dim.MtrTitle != null)
                this.MtrTitle = dim.MtrTitle;


            if (dim.Classification != null)
            {
                this.Classifications = new List<ClassificationRecordDTO_Create>();
                foreach (var pCls in dim.Classification)
                {
                    ClassificationRecordDTO_Create cls = new ClassificationRecordDTO_Create(pCls);


                    this.Classifications.Add(cls);
                }
            }
        }

        public Dimension_DTO()
        {
        }

        /// <summary>
        /// Tests if two Dimension_DTO objects are equivalent, i.e. that the properties that are not language variant are the same but not necessarily in the same order
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        internal bool IsEquivalentUnordered(Dimension_DTO comp)
        {
            //check that the Classification codes are the same 
            var thisCodeList = String.Join(",", ((this.Classifications.OrderBy(x => x.Code).Select(c => c.Code.ToString())).ToList<string>()).ToArray());
            var otherCodeList = String.Join(",", ((comp.Classifications.OrderBy(x => x.Code).Select(c => c.Code.ToString())).ToList<string>()).ToArray());


            if (!thisCodeList.Equals(otherCodeList)) return false;

            //check that the Variables within the Classifications are the same 
            var thisVariableObjectList = this.Classifications.OrderBy(x => x.Code).Select(v => v.Variable);
            var otherVariableObjectList = comp.Classifications.OrderBy(x => x.Code).Select(v => v.Variable);


            string thisVrbString = "";
            foreach (var vrb in thisVariableObjectList)
            {
                if (vrb != null)
                    thisVrbString = thisVrbString + String.Join(",", ((vrb.Select(c => c.Code.ToString())).ToList<string>().OrderBy(x => x)).ToArray());
            }
            string otherVrbString = "";
            foreach (var vrb in otherVariableObjectList)
            {
                if (vrb != null)
                    otherVrbString = otherVrbString + String.Join(",", ((vrb.Select(c => c.Code.ToString())).ToList<string>().OrderBy(x => x)).ToArray());
            }

            if (!thisVrbString.Equals(otherVrbString)) return false;

            //check that the Statistic codes are the same 
            var thisStatList = "";
            if (this.Statistics != null)
                thisStatList = String.Join(",", ((this.Statistics.Select(c => c.Code.ToString())).ToList<string>()).OrderBy(x => x).ToArray());

            var otherStatList = "";
            if (comp.Statistics != null)
                otherStatList = String.Join(",", ((comp.Statistics.Select(c => c.Code.ToString())).ToList<string>()).OrderBy(x => x).ToArray());

            if (!thisStatList.Equals(otherStatList)) return false;

            var thisPeriodList = "";
            if (this.Frequency != null)
            {
                if (this.Frequency.Period != null)
                    thisPeriodList = String.Join(",", ((this.Frequency.Period.Select(c => c.Code.ToString())).ToList<string>()).OrderBy(x => x).ToArray());
            }
            var otherPeriodList = "";
            if (comp.Frequency != null)
            {
                if (comp.Frequency.Period != null)
                    otherPeriodList = String.Join(",", ((comp.Frequency.Period.Select(c => c.Code.ToString())).ToList<string>()).OrderBy(x => x).ToArray());
            }

            if (!thisPeriodList.Equals(otherPeriodList)) return false;

            return true;
        }

        /// <summary>
        /// Tests if two Dimension_DTO objects are equivalent, i.e. that the properties that are not language variant are the same and in the same order
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        internal bool IsEquivalent(Dimension_DTO comp)
        {

            //check that the Classification codes are the same and in the same order
            var thisCodeList = String.Join(",", ((this.Classifications.Select(c => c.Code.ToString())).ToList<string>()).ToArray());
            var otherCodeList = String.Join(",", ((comp.Classifications.Select(c => c.Code.ToString())).ToList<string>()).ToArray());


            if (!thisCodeList.Equals(otherCodeList)) return false;

            //check that the Variables within the Classifications are the same and in the same order
            var thisVariableObjectList = this.Classifications.Select(v => v.Variable);
            var otherVariableObjectList = comp.Classifications.Select(v => v.Variable);


            string thisVrbString = "";
            foreach (var vrb in thisVariableObjectList)
            {
                if (vrb != null)
                    thisVrbString = thisVrbString + String.Join(",", ((vrb.Select(c => c.Code.ToString())).ToList<string>()).ToArray());
            }
            string otherVrbString = "";
            foreach (var vrb in otherVariableObjectList)
            {
                if (vrb != null)
                    otherVrbString = otherVrbString + String.Join(",", ((vrb.Select(c => c.Code.ToString())).ToList<string>()).ToArray());
            }

            if (!thisVrbString.Equals(otherVrbString)) return false;

            //check that the Statistic codes are the same and in the same order
            var thisStatList = "";
            if (this.Statistics != null)
                thisStatList = String.Join(",", ((this.Statistics.Select(c => c.Code.ToString())).ToList<string>()).ToArray());

            var otherStatList = "";
            if (comp.Statistics != null)
                otherStatList = String.Join(",", ((comp.Statistics.Select(c => c.Code.ToString())).ToList<string>()).ToArray());

            if (!thisStatList.Equals(otherStatList)) return false;
            return true;
        }
    }
}
