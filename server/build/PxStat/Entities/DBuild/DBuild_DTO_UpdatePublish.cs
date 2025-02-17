using Newtonsoft.Json.Linq;
using PxStat.Security;
using System;
using System.Collections.Generic;

namespace PxStat.DBuild
{

    public class DBuild_DTO_UpdatePublish
    {
        //[DefaultSanitizer]

        public string MtrCode { get; set; }
        public string LngIsoCode { get; set; }
        public DateTime WrqDatetime { get; set; }

        public List<DSpec_DTO> Dspecs { get; set; }
        public List<List<string>> ChangeData { get; set; }
        public int RlsCode { get; set; }

        public DBuild_DTO_UpdatePublish() { }

        public DBuild_DTO_UpdatePublish(dynamic parameters)
        {
            if (parameters.MtrCode != null)
                MtrCode = parameters.MtrCode;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            if (parameters.WrqDatetime != null)
                WrqDatetime = parameters.WrqDatetime;

            if (parameters.RlsCode != null)
                RlsCode = parameters.RlsCode;


            if (parameters.Data != null)
            {
                ChangeData = new List<List<string>>();
                foreach (var line in parameters.Data)
                {
                    JArray jLine = JArray.FromObject(line);

                    ChangeData.Add(jLine.ToObject<List<string>>());
                }

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
                        dspec.MtrNote = Resources.Cleanser.CleanseDynamic(dim.MtrNote);
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
                                varDto.Code = Resources.Cleanser.CleanseDynamic(per.PrdCode);
                                varDto.Value = Resources.Cleanser.CleanseDynamic(per.PrdValue);
                                statDim.Variables.Add(varDto);
                            }

                        }

                        dspec.StatDimensions.Add(statDim);
                    }

                    Dspecs.Add(dspec);

                }

            }


        }


    }
}
