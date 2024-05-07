using API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxStat.Data;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reflection;

namespace PxStat.DBuild
{
    /// <summary>
    /// 
    /// </summary>
    public class DBuild_DTO_Update
    {
        /// <summary>
        /// 
        /// </summary>
        [NoTrim]
        [NoHtmlStrip]
        [DefaultSanitizer]
        public IList<Dimension_DTO> DimensionList { get; set; }

        /// 
        /// </summary>
        [LowerCase]
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Matrix Title
        /// </summary>
        public string MtrTitle { get; set; }
        /// <summary>
        /// Matrix Note
        /// </summary>
        public string MtrNote { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MtrCode { get; set; }
        /// <summary>
        /// Idenfiier for the STATISTIC dimension in the PX file
        /// </summary>
        public string StatisticLabel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? MtrOfficialFlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [NoTrim]
        [NoHtmlStrip]
        [DefaultSanitizer]
        public string MtrInput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CprCode { get; set; }

        /// <summary>
        /// This is the frequency code that will be used in the modified metadata
        /// </summary>
        public string FrqCodeTimeval { get; set; }

        /// <summary>
        /// This is required where there is no TIMEVAL in the matrix and the user has nominated a dimension that will serve as a time dimension
        /// The FrqValue will be the name of the chosen dimension and if a FrqCode has not been supplied, a code will be generated.
        /// </summary>
        public string FrqValueTimeval { get; set; }

        /// <summary>
        /// Statistic Value
        /// </summary>
        public string SttValue { get; set; }

        public Format_DTO_Read Format { get; set; }

        public Dictionary<string, string> Elimination { get; set; }

        public Dictionary<string, string> Map { get; set; }

        public List<List<string>> ChangeData { get; set; }

        public List<DSpec_DTO> Dspecs { get; set; }
        public string Signature { get; set; }
        public bool Labels { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public DBuild_DTO_Update(dynamic parameters)
        {
            if (parameters.MtrInput != null)
                this.MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);




            this.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");




            if (parameters.MtrOfficialFlag != null)
                this.MtrOfficialFlag = parameters.MtrOfficialFlag;
            else
            {
 
                this.MtrOfficialFlag = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.officialStatistics");
            }

            if (parameters.MtrCode != null)
                this.MtrCode = parameters.MtrCode;
            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;
            if (parameters.FrqCodeTimeval != null)
                this.FrqCodeTimeval = parameters.FrqCodeTimeval;
            if (parameters.FrqValueTimeval != null)
                this.FrqValueTimeval = parameters.FrqValueTimeval;
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
            if (parameters.Labels != null)
            {
                if (Boolean.TryParse(parameters.Labels.ToString(), out bool bval))
                    Labels = bval;
            }
            if (parameters.Elimination != null)
            {
                Elimination = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters.Elimination.ToString());
            }
            if (parameters.Signature != null)
            {
                Signature = parameters.Signature;
            }

            if (parameters.CprCode != null)
            {

            }
            if (parameters.Data != null)
            {
                ChangeData = new List<List<string>>();
                foreach (var line in parameters.Data)
                {
                    JArray jLine = JArray.FromObject(line);
                    
                    ChangeData.Add(jLine.ToObject<List<string>>());//.Select(x => x.Trim()).ToList());
                }

            }

            if (parameters.Elimination != null)
            {

            }


            if (parameters.report != null)
            {

            }


            if (parameters.Dimension != null)
            {
                Dspecs = new List<DSpec_DTO>();
                foreach (var dim in parameters.Dimension)
                {
                    DSpec_DTO dspec = new DSpec_DTO();
                    dspec.StatDimensions = new List<StatDimension_DTO>();


                    if (dim.StatisticLabel != null)
                    {
                        StatDimension_DTO sdim = new StatDimension_DTO();
                        sdim.Role = "STATISTIC";
                        sdim.Code = sdim.Role;
                        sdim.Value = dim.StatisticLabel;
                        dspec.ContentVariable = dim.StatisticLabel;
                        dspec.StatDimensions.Add(sdim);
                    }

                    if (dim.MtrNote != null)
                    {
                        dspec.MtrNote = dim.MtrNote;
                    }


                    if (dim.MtrTitle != null)
                    {
                        dspec.MtrTitle = dim.MtrTitle;

                    }

                    if (dim.LngIsoCode != null)
                    {
                        dspec.Language = dim.LngIsoCode;
                    }


                    if (dim.Frequency != null)
                    {
                        StatDimension_DTO statDim = new StatDimension_DTO();
                        statDim.Role = "TIME";

                        if (parameters.FrqCodeTimeval != null)
                        {
                            statDim.Code = parameters.FrqCodeTimeval;
                        }
                        if (dim.Frequency.FrqValue != null)
                        {
                            statDim.Value = dim.Frequency.FrqValue;
                        }

                        if (dim.Frequency.Period != null)
                        {
                            statDim.Variables = new List<DimensionVariable_DTO>();

                            foreach (var per in dim.Frequency.Period)
                            {
                                DimensionVariable_DTO varDto = new DimensionVariable_DTO();
                                varDto.Code = per.PrdCode;
                                varDto.Value = per.PrdValue;
                                statDim.Variables.Add(varDto);
                            }

                        }

                        dspec.StatDimensions.Add(statDim);
                    }

                    Dspecs.Add(dspec);

                }

            }

            if (parameters.Map != null)
            {
                Map = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters.Map.ToString());
            }
        }

        public DBuild_DTO_Update()
        {
        }

        public Signature_DTO GetSignatureDTO()
        {
            return new Signature_DTO
            {
                MtrInput = this.MtrInput,
                FrqCodeTimeval = this.FrqCodeTimeval,
                FrqValueTimeval = this.FrqValueTimeval
            };


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
        public string MtrInput { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<Dimension_DTO> Dimension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// 

        public List<PeriodRecordDTO_Create> Periods { get; set; }

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




        public Signature_DTO GetSignatureDTO()
        {
            return new Signature_DTO
            {
                MtrInput = this.MtrInput,
                FrqCodeTimeval = this.FrqCodeTimeval,
                FrqValueTimeval = this.FrqValueTimeval
            };


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
        public string MtrInput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PxData_DTO PxData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<Dimension_DTO> Dimension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// 

        public List<PeriodRecordDTO_Create> Periods { get; set; }

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

        public bool Labels { get; set; }

        public Dictionary<string, string> Elimination { get; set; }

        public Dictionary<string, string> Map { get; set; }

        public Signature_DTO GetSignatureDTO()
        {
            return new Signature_DTO
            {
                MtrInput = this.MtrInput,
                FrqCodeTimeval = this.FrqCodeTimeval,
                FrqValueTimeval = this.FrqValueTimeval
            };


        }



    }

    /// <summary>
    /// 
    /// </summary>
    public class PxPeriodListDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public List<PeriodRecordDTO_Create> Periods { get; set; }

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
        public List<string> Rows { get; set; }
        public List<dynamic> DataItems { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public PxData_DTO(dynamic parameters)
        {
            if (parameters.Data != null)
            {
                DataItems = new List<dynamic>();
                foreach (var v in parameters.Data)
                {
                    v.SortId = 0;
                    DataItems.Add(v);

                }

            }
        }

        public PxData_DTO() { }
    }


    /// <summary>
    /// 
    /// </summary>
    public class Dimension_DTO
    {
        /// <summary>
        /// 
        /// </summary>
        public string FrqCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FrqValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [LowerCase]
        public string LngIsoCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MtrTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MtrNote { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CprCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CprValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StatisticLabel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //public List<ClassificationRecordDTO_Create> Classifications { get; set; }

        /// <summary>
        /// 
        /// </summary>
       // public List<StatisticalRecordDTO_Create> Statistics { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<PeriodRecordDTO_Create> Periods { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FrequencyRecordDTO_Create Frequency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dim"></param>
   
        /*
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
        */
    }

    public class Signature_DTO
    {
        public string MtrInput { get; set; }
        public string FrqValueTimeval { get; set; }
        public string FrqCodeTimeval { get; set; }


    }
}
