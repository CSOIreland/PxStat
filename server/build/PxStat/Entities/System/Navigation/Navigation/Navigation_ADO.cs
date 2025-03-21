﻿using API;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Data;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// IADO for Navigation methods
    /// </summary>
    internal class Navigation_ADO
    {
        /// <summary>
        /// Class variable IADO
        /// </summary>
        IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Ado"></param>
        internal Navigation_ADO(IADO Ado)
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
                new ADO_inputParams() {name="@LngIsoCodeDefault",value=Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code") }
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





        internal dynamic Search(Navigation_DTO_Search dto)
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


            //Call the stored procedure
            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Navigation_Search", paramList);

            return output.data;
        }

        internal dynamic AssociatedSearch(Navigation_DTO_Search dto)
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


            //Call the stored procedure
            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Navigation_Associated_Search", paramList);

            return output.data;
        }

        internal dynamic EntitySearch(Navigation_DTO_Search dto)
        {

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            if (dto.MtrCode != null)
                paramList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });
            if (dto.MtrOfficialFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@MtrOfficialFlag", value = dto.MtrOfficialFlag });
            if (dto.ThmCode!= default(int))
                paramList.Add(new ADO_inputParams() { name = "@ThmCode", value = dto.ThmCode });
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
                paramList.Add(new ADO_inputParams() { name = "@RlsExperimentalFlag", value = dto.RlsExperimentalFlag });
            if (dto.RlsAnalyticalFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsAnalyticalFlag", value = dto.RlsAnalyticalFlag });
      
            //Call the stored procedure
            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Navigation_EntitySearch", paramList);
            return output.data;
        }

        internal dynamic EntityAssociatedSearch(Navigation_DTO_Search dto)
        {

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            if (dto.MtrCode != null)
                paramList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });
            if (dto.MtrOfficialFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@MtrOfficialFlag", value = dto.MtrOfficialFlag });
            if (dto.ThmCode != default(int))
                paramList.Add(new ADO_inputParams() { name = "@ThmCode", value = dto.ThmCode });
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
                paramList.Add(new ADO_inputParams() { name = "@RlsExperimentalFlag", value = dto.RlsExperimentalFlag });
            if (dto.RlsAnalyticalFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsAnalyticalFlag", value = dto.RlsAnalyticalFlag });
         
            //Call the stored procedure
            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Navigation_Entity_Associated_Search", paramList);
            return output.data;
        }

        internal dynamic ReadSearchResults(DataTable dtMatrixResults, string lngIsoCode)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@LngIsoCode", value = lngIsoCode },
                new ADO_inputParams() { name = "@DefaultLngIsoCode", value = Configuration_BSO.GetApplicationConfigItem( ConfigType.global,"language.iso.code") }
            };

            ADO_inputParams param = new ADO_inputParams() { name = "@Result", value = dtMatrixResults };

            param.typeName = "KeyValueVarcharAttribute";
            paramList.Add(param);

            //Call the stored procedure
            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Navigation_Search_Read", paramList);

            return output.data;
        }

    }
}
