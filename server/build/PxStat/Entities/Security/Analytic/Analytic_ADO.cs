using API;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PxStat.Security
{
    /// <summary>
    /// IADO methods for Analytics
    /// </summary>
    internal class Analytic_ADO
    {
        /// <summary>
        /// IADO class parameter
        /// </summary>
        IADO Ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal Analytic_ADO(IADO ado)
        {
            Ado = ado;
        }

        /// <summary>
        /// Create an analytic entry
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int Create(Analytic_DTO dto)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@matrix",value=dto.matrix},
                new ADO_inputParams() {name= "@NltMaskedIp",value=dto.NltMaskedIp},
                new ADO_inputParams() {name= "@NltBotFlag",value=dto.NltBotFlag},
                new ADO_inputParams() {name= "@NltM2m",value=dto.NltM2m},
                new ADO_inputParams() {name= "@NltWidget",value=dto.NltWidget },
                new ADO_inputParams() {name= "@NltDate",value=dto.NltDate},
                new ADO_inputParams() {name= "@EnvironmentLngIsoCode",value=dto.EnvironmentLngIsoCode??Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code")},
                new ADO_inputParams() {name="@LngIsoCode",value=dto.LngIsoCode??Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code")},
                new ADO_inputParams() {name="@NltUser",value=dto.NltUser}
            };
            if (dto.NltOs != null)
                inputParamList.Add(new ADO_inputParams() { name = "@NltOs", value = dto.NltOs });
            if (dto.NltBrowser != null)
                inputParamList.Add(new ADO_inputParams() { name = "@NltBrowser", value = dto.NltBrowser });
            if (dto.NltReferer != null)
                inputParamList.Add(new ADO_inputParams() { name = "@NltReferer", value = dto.NltReferer });
            if (dto.FrmType != null && dto.FrmVersion != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@FrmType", value = dto.FrmType });
                inputParamList.Add(new ADO_inputParams() { name = "@FrmVersion", value = dto.FrmVersion });
            }



            var retParam = new ADO_returnParam() { name = "return", value = 0 };


            Ado.ExecuteNonQueryProcedure("Security_Analytic_Create", inputParamList, ref retParam);


            return retParam.value;

        }


        internal List<dynamic> ReadForAll(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            result = Ado.ExecuteReaderProcedure("Security_Analytic_ReadForAll", inputParamList);



            return result.data;



            
        }

        internal List<dynamic> ReadBrowser(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name="@MtrCode",value=dto.MtrCode});

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode != default)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (!String.IsNullOrEmpty(dto.NltInternalNetworkMask))
                inputParamList.Add(new ADO_inputParams() { name = "@NltMaskedIp", value = dto.NltInternalNetworkMask });

            result = Ado.ExecuteReaderProcedure("Security_Analytic_ReadBrowser", inputParamList);



            return result.data;


        }

        internal List<dynamic> ReadEnvironmentLanguage(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode != default)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (!String.IsNullOrEmpty(dto.NltInternalNetworkMask))
                inputParamList.Add(new ADO_inputParams() { name = "@NltMaskedIp", value = dto.NltInternalNetworkMask });

            result = Ado.ExecuteReaderProcedure("Security_Analytic_ReadEnvironmentLanguage", inputParamList);



            return result.data;


        }


        internal List<dynamic> ReadFormat(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode != default)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (!String.IsNullOrEmpty(dto.NltInternalNetworkMask))
                inputParamList.Add(new ADO_inputParams() { name = "@NltMaskedIp", value = dto.NltInternalNetworkMask });

            result = Ado.ExecuteReaderProcedure("Security_Analytic_ReadFormat", inputParamList);



            return result.data;


        }

        internal List<dynamic> ReadLanguage(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode != default)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (!String.IsNullOrEmpty(dto.NltInternalNetworkMask))
                inputParamList.Add(new ADO_inputParams() { name = "@NltMaskedIp", value = dto.NltInternalNetworkMask });

            result = Ado.ExecuteReaderProcedure("Security_Analytic_ReadLanguage", inputParamList);



            return result.data;


        }

        internal List<dynamic> ReadOs(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode != default)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (!String.IsNullOrEmpty(dto.NltInternalNetworkMask))
                inputParamList.Add(new ADO_inputParams() { name = "@NltMaskedIp", value = dto.NltInternalNetworkMask });

            result = Ado.ExecuteReaderProcedure("Security_Analytic_ReadOs", inputParamList);

            return result.data;
        }

        internal List<dynamic> ReadReferer(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode != default)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (!String.IsNullOrEmpty(dto.NltInternalNetworkMask))
                inputParamList.Add(new ADO_inputParams() { name = "@NltMaskedIp", value = dto.NltInternalNetworkMask });

            result = Ado.ExecuteReaderProcedure("Security_Analytic_ReadReferers", inputParamList);

            return result.data;
        }

        internal List<dynamic> ReadTimeline(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode != default)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (!String.IsNullOrEmpty(dto.NltInternalNetworkMask))
                inputParamList.Add(new ADO_inputParams() { name = "@NltMaskedIp", value = dto.NltInternalNetworkMask });

            result = Ado.ExecuteReaderProcedure("Security_Analytic_ReadTimeline", inputParamList);

            return result.data;
        }

        internal List<dynamic> Read(Analytic_DTO_Read dto)
        {
            //For reasons of cache consistency we limit queries to days in the past
            if (dto.DateTo.Date >= (DateTime.Now.Date)) dto.DateTo = DateTime.Now.Date.AddDays(-1);

            ADO_readerOutput result;

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo},
                new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode}
            };

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode != default)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (!String.IsNullOrEmpty(dto.NltInternalNetworkMask))
                inputParamList.Add(new ADO_inputParams() { name = "@NltMaskedIp", value = dto.NltInternalNetworkMask });

            result = Ado.ExecuteReaderProcedure("Security_Analytic_Read", inputParamList);

            return result.data;
        }

    }
}
