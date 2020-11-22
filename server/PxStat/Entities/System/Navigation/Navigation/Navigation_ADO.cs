using API;
using PxStat.Security;
using System;
using System.Collections.Generic;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// ADO for Navigation methods
    /// </summary>
    internal class Navigation_ADO
    {
        /// <summary>
        /// Class variable ADO
        /// </summary>
        ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Ado"></param>
        internal Navigation_ADO(ADO Ado)
        {
            ado = Ado;
        }

        /// <summary>
        /// Read method
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(Navigation_DTO_Read dto)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name="@LngIsoCode",value=dto.LngIsoCode  },
                new ADO_inputParams() {name="@LngIsoCodeDefault",value=Configuration_BSO.GetCustomConfig(ConfigType.global,"language.iso.code") }
            };

            return ado.ExecuteReaderProcedure("System_Navigation_Navigation_Read", inputParams);
        }

        /// <summary>
        /// Reads the next live release date after the supplied date
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadNextLiveDate(DateTime dateFrom, string PrcCode = null)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name="@ReleaseDate",value=dateFrom }
            };

            if (PrcCode != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@PrcCode", value = PrcCode });
            }

            return ado.ExecuteReaderProcedure("Data_Release_ReadNext", inputParams);
        }



        internal dynamic Search(Navigation_DTO_Search dto, int searchTermCount)
        {


            //The ExecuteReaderProcedure method requires that the parameters be contained in a List<ADO_inputParams>
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode }

            };

            ADO_inputParams param = new ADO_inputParams() { name = "@Search", value = dto.SearchTerms };
            param.typeName = "KeyValueVarcharAttribute";
            paramList.Add(param);
            //We need a count of search terms (ignoring duplicates caused by singularisation)
            paramList.Add(new ADO_inputParams() { name = "@SearchTermCount", value = searchTermCount });

            if (dto.MtrCode != null)
                paramList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });
            if (dto.MtrOfficialFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@MtrOfficialFlag", value = dto.MtrOfficialFlag });
            if (dto.SbjCode != default(int))
                paramList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });
            if (dto.PrcCode != null)
                paramList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });
            if (dto.CprCode != null)
                paramList.Add(new ADO_inputParams() { name = "@CprCode", value = dto.CprCode });
            if (dto.RlsExceptionalFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsExceptionalFlag", value = dto.RlsExceptionalFlag });
            if (dto.RlsReservationFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsReservationFlag", value = dto.RlsReservationFlag });
            if (dto.RlsArchiveFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsArchiveFlag", value = dto.RlsArchiveFlag });
            if (dto.RlsExperimentalFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsExperiementalFlag", value = dto.RlsExperimentalFlag });
            if (dto.RlsAnalyticalFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsAnalyticalFlag", value = dto.RlsAnalyticalFlag });





            //Call the stored procedure
            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Navigation_Search", paramList);

            return output.data;
        }
    }
}
