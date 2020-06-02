using API;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// ADO methods for Analytics
    /// </summary>
    internal class Analytic_ADO
    {
        /// <summary>
        /// ADO class parameter
        /// </summary>
        ADO Ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal Analytic_ADO(ADO ado)
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
                new ADO_inputParams() {name= "@NltDate",value=dto.NltDate},
                new ADO_inputParams() {name= "@LngIsoCode",value=Configuration_BSO.GetCustomConfig("language.iso.code")}
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

        /// <summary>
        /// Read summarised Analytic data
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(Analytic_DTO_Read dto)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo}
            };


            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode > 0)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (dto.NltInternalNetworkMask != null)
            {
                if (dto.NltInternalNetworkMask.Length > 0)
                    inputParamList.Add(new ADO_inputParams() { name = "@NltInternalNetworkMask", value = dto.NltInternalNetworkMask });
            }

            //Call the stored procedure
            return Ado.ExecuteReaderProcedure("Security_Analytic_Read", inputParamList);


        }
        /// <summary>
        /// Read an analysis by Operating System
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadOs(Analytic_DTO_Read dto)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo}
            };

            if (dto.NltInternalNetworkMask != null)
            {
                if (dto.NltInternalNetworkMask.Length > 0)
                    inputParamList.Add(new ADO_inputParams() { name = "@NltInternalNetworkMask", value = dto.NltInternalNetworkMask });
            }

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode > 0)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            //Call the stored procedure
            return Ado.ExecuteReaderProcedure("Security_Analytic_ReadOs", inputParamList);

        }

        //Security_Analytic_ReadFormat
        internal ADO_readerOutput ReadFormat(Analytic_DTO_Read dto)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo}
            };

            if (dto.NltInternalNetworkMask != null)
            {
                if (dto.NltInternalNetworkMask.Length > 0)
                    inputParamList.Add(new ADO_inputParams() { name = "@NltInternalNetworkMask", value = dto.NltInternalNetworkMask });
            }

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode > 0)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });


            //Call the stored procedure
            return Ado.ExecuteReaderProcedure("Security_Analytic_ReadFormat", inputParamList);

        }

        /// <summary>
        /// Read an analysis by Browser
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadBrowser(Analytic_DTO_Read dto)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo}
            };

            if (dto.NltInternalNetworkMask != null)
            {
                if (dto.NltInternalNetworkMask.Length > 0)
                    inputParamList.Add(new ADO_inputParams() { name = "@NltInternalNetworkMask", value = dto.NltInternalNetworkMask });
            }

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (dto.SbjCode > 0)
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });


            //Call the stored procedure
            return Ado.ExecuteReaderProcedure("Security_Analytic_ReadBrowser", inputParamList);

        }
        /// <summary>
        /// Read an analysis by Date
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadTimeline(Analytic_DTO_Read dto)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo}
            };

            if (dto.NltInternalNetworkMask != null)
            {
                if (dto.NltInternalNetworkMask.Length > 0)
                    inputParamList.Add(new ADO_inputParams() { name = "@NltInternalNetworkMask", value = dto.NltInternalNetworkMask });
            }

            if (dto.MtrCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });

            if (dto.LngIsoCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode });

            if (dto.SbjCode != default(int))
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            //Call the stored procedure
            return Ado.ExecuteReaderProcedure("Security_Analytic_ReadTimeline", inputParamList);

        }

        /// <summary>
        /// Read an analysis by Referrer
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadReferrer(Analytic_DTO_Read dto)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo}
            };

            if (dto.NltInternalNetworkMask != null)
            {
                if (dto.NltInternalNetworkMask.Length > 0)
                    inputParamList.Add(new ADO_inputParams() { name = "@NltInternalNetworkMask", value = dto.NltInternalNetworkMask });
            }


            if (dto.SbjCode != default(int))
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            return Ado.ExecuteReaderProcedure("Security_Analytic_ReadReferers", inputParamList);

        }

        /// <summary>
        /// Read an analysis by Language of the Matrix queried
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadLanguage(Analytic_DTO_Read dto)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@DateFrom",value=dto.DateFrom},
                new ADO_inputParams() {name= "@DateTo",value=dto.DateTo}
            };

            if (dto.NltInternalNetworkMask != null)
            {
                if (dto.NltInternalNetworkMask.Length > 0)
                    inputParamList.Add(new ADO_inputParams() { name = "@NltInternalNetworkMask", value = dto.NltInternalNetworkMask });
            }

            if (dto.LngIsoCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode });

            if (dto.SbjCode != default(int))
                inputParamList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });

            if (dto.PrcCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            return Ado.ExecuteReaderProcedure("Security_Analytic_ReadLanguage", inputParamList);
        }
    }
}
