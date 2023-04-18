using Newtonsoft.Json;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.DBuild
{

    public class DBuild_DTO_Create
    {

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
        public string FrqCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public DBuild_DTO_Create(dynamic parameters)
        {

            this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

            if (parameters.MtrOfficialFlag != null)
                this.MtrOfficialFlag = parameters.MtrOfficialFlag;
            else
            {
                this.MtrOfficialFlag = Configuration_BSO.GetCustomConfig(ConfigType.global, "dataset.officialStatistics");
            }

            if (parameters.MtrCode != null)
                this.MtrCode = parameters.MtrCode;
            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;
            if (parameters.FrqCodeTimeval != null)
                this.FrqCodeTimeval = parameters.FrqCodeTimeval;
            if (parameters.FrqValueTimeval != null)
                this.FrqValueTimeval = parameters.FrqValueTimeval;
            if (parameters.FrqCode != null)
                this.FrqCode = parameters.FrqCode;
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

            if (parameters.Elimination != null)
            {
                Elimination = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters.Elimination.ToString());
            }


            if (parameters.CprCode != null)
            {
                this.CprCode = parameters.CprCode;
            }

            if (parameters.Map != null)
            {
                Map = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters.Map.ToString());
            }


            if (parameters.Dimension != null)
            {
                Dspecs = new List<DSpec_DTO>();
                foreach (var dtoSpec in parameters.Dimension)
                {
                    DSpec_DTO dspec = new DSpec_DTO();
                    dspec.StatDimensions = new List<StatDimension_DTO>();
                    if (dtoSpec.MtrNote != null)
                    {
                        dspec.MtrNote = dtoSpec.MtrNote;
                    }


                    if (dtoSpec.MtrTitle != null)
                    {
                        dspec.MtrTitle = dtoSpec.MtrTitle;
                        dspec.Contents = dtoSpec.MtrTitle;

                    }

                    if (dtoSpec.LngIsoCode != null)
                    {
                        dspec.Language = dtoSpec.LngIsoCode;
                    }

                    if (dtoSpec.Statistic != null)
                    {
                        StatDimension_DTO sdim = new StatDimension_DTO
                        {
                            Role = Constants.C_DATA_DIMENSION_ROLE_STATISTIC,
                            Code = Constants.C_DATA_DIMENSION_ROLE_STATISTIC,
                            Value = dtoSpec.StatisticLabel ?? null,
                            Variables = new List<DimensionVariable_DTO>()
                        };
                        dspec.ContentVariable = dtoSpec.StatisticLabel ?? null;
                        dspec.StatDimensions.Add(sdim);
                        foreach (var stat in dtoSpec.Statistic)
                        {
                            sdim.Variables.Add(new DimensionVariable_DTO()
                            {
                                Code = stat.SttCode ?? null,
                                Value = stat.SttValue ?? null,
                                Unit = stat.SttUnit ?? null,
                                Decimals = stat.SttDecimal ?? 0

                            });
                        }
                    }

                    if (dtoSpec.Frequency != null)
                    {
                        StatDimension_DTO statDim = new StatDimension_DTO
                        {
                            Role = Constants.C_DATA_DIMENSION_ROLE_TIME,
                            Code = parameters.FrqCode ?? null,
                            Value = dtoSpec.Frequency.FrqValue ?? null,
                            Variables = new List<DimensionVariable_DTO>()
                        };
                        if (dtoSpec.Frequency.Period != null)
                        {
                            foreach (var prd in dtoSpec.Frequency.Period)
                            {
                                statDim.Variables.Add(new DimensionVariable_DTO()
                                {
                                    Code = prd.PrdCode,
                                    Value = prd.PrdValue
                                });
                            }
                        }


                        dspec.StatDimensions.Add(statDim);
                    }

                    if (dtoSpec.Classification != null)
                    {
                        foreach (var cls in dtoSpec.Classification)
                        {
                            StatDimension_DTO statDim = new StatDimension_DTO
                            {
                                Role = Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION,
                                Code = cls.ClsCode ?? null,
                                Value = cls.ClsValue ?? null,
                                GeoUrl = cls.ClsGeoUrl ?? null,
                                GeoFlag = cls.ClsGeoUrl != null ? true : false,
                                Variables = new List<DimensionVariable_DTO>()
                            };

                            if (cls.Variable != null)
                            {
                                foreach (var vrb in cls.Variable)
                                {
                                    statDim.Variables.Add(new DimensionVariable_DTO()
                                    {
                                        Code = vrb.VrbCode ?? null,
                                        Value = vrb.VrbValue ?? null
                                    });
                                }
                            }



                            dspec.StatDimensions.Add(statDim);
                        }
                    }

                    if (Elimination != null)
                    {
                        foreach (var e in Elimination)
                        {
                            if (dspec.StatDimensions.Where(x => x.Code.Equals(e.Key))?.Count() > 0)
                            {
                                var eDim = dspec.StatDimensions.Where(x => x.Code.Equals(e.Key)).First();
                                if (eDim.Variables.Where(x => x.Code.Equals(e.Value))?.Count() > 0)
                                {
                                    var eVar = eDim.Variables.Where(x => x.Code.Equals(e.Value)).First();
                                    eVar.Elimination = true;
                                }
                            }
                        }
                    }

                    if (Map != null)
                    {
                        foreach (var m in Map)
                        {
                            if (dspec.StatDimensions.Where(x => x.Code.Equals(m.Key))?.Count() > 0)
                            {
                                var eDim = dspec.StatDimensions.Where(x => x.Code.Equals(m.Key)).First();
                                eDim.GeoFlag = m.Value != null;
                                eDim.GeoUrl = m.Value;
                            }
                        }
                    }

                    Dspecs.Add(dspec);

                }

            }




        }



    }


}
