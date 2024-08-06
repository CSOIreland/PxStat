using API;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PxStat.Data;
using PxStat.Security;
using PxStat.System.Settings;
using System.Collections.Generic;
using System;

namespace PxStat.DBuild
{
    public class DBuild_DTO_UpdateByRelease
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

        public bool Labels { get; set; }

        public int RlsCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public DBuild_DTO_UpdateByRelease(dynamic parameters)
        {
            this.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            if (parameters.MtrOfficialFlag != null)
                this.MtrOfficialFlag = parameters.MtrOfficialFlag;
            else
            {
                this.MtrOfficialFlag = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.officialStatistics");
            }

            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;
            if (parameters.FrqCodeTimeval != null)
                this.FrqCodeTimeval = parameters.FrqCodeTimeval;
            if (parameters.FrqValueTimeval != null)
                this.FrqValueTimeval = parameters.FrqValueTimeval;
            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;
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

    }
}
