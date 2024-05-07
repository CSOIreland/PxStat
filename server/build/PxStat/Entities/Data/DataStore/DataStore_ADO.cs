using API;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Data;

namespace PxStat.DataStore
{
    internal class DataStore_ADO
    {
        //Data_Datrix_ReadDmatrixByRelease
        internal ADO_readerOutput ReadDMatrixByRelease(IADO ado, int rlsCode, string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@RlsCode",value=rlsCode },
                new ADO_inputParams(){ name="@LngIsoCode",value=lngIsoCode }
            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadDmatrixByRelease", inputParams);
        }

        /// <summary>
        /// Creates new keyword entries by bulk loading a table to the TD_KEYWORD_RELEASE table
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="dt"></param>
        internal void CreateKeywordsFromTable(IADO Ado, DataTable dt)
        {
            //Before bulk inserting, we must map our datatable to the database table, column by column
            var maps = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string> ("KRL_VALUE", "KRL_VALUE"),
                    new KeyValuePair<string, string>("KRL_RLS_ID", "KRL_RLS_ID"),
                    new KeyValuePair<string, string>("KRL_MANDATORY_FLAG", "KRL_MANDATORY_FLAG"),
                    new KeyValuePair<string, string>("KRL_SINGULARISED_FLAG","KRL_SINGULARISED_FLAG")
                };
            using (IADO bulkAdo = AppServicesHelper.StaticADO)
            {
                bulkAdo.ExecuteBulkCopy("TD_KEYWORD_RELEASE", maps, dt, false);
            }
        }

        /// <summary>
        /// Gets all matrices for a specific Release Code - there is usually just one non-deleted matrix per release 
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadAllForRelease(IADO Ado, int rlsCode, string userName)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@CcnUsername", value = userName },
                new ADO_inputParams { name = "@RlsCode", value = rlsCode }
            };

            var reader = Ado.ExecuteReaderProcedure("Data_Matrix_ReadAllForRelease", inputParams);

            return reader.data;
        }


        internal ADO_readerOutput GetDimensionsForMatrix(IADO ado, int mtrId)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrId",value=mtrId }

            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadDimensions", inputParams);
        }

        internal ADO_readerOutput GetItemsForDimension(IADO ado, int mdmId)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MdmId",value=mdmId}
            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadDimensionItems", inputParams);
        }





        internal ADO_readerOutput GetFieldDataForMatrix(IADO ado, int mtrId, int cacheLifetime = -1)
        {

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrId",value=mtrId}
            };
            if (cacheLifetime < 0) return ado.ExecuteReaderProcedure("Data_Matrix_ReadSingleField", inputParams);

            var cache =AppServicesHelper.CacheD.Get_ADO("PxStat.DataStore", "Data_Matrix_ReadSingleField", inputParams);
            ADO_readerOutput result;
            if (!cache.hasData)
            {
                result = ado.ExecuteReaderProcedure("Data_Matrix_ReadSingleField", inputParams);

            }
            else
            {
                result = cache.data;
            }
           AppServicesHelper.CacheD.Store_ADO<dynamic>("PxStat.DataStore", "Data_Matrix_ReadSingleField", inputParams, result.data, (DateTime)default);
            return result;
        }

        /// <summary>
        ///  Get a list of Matrix codes based on rights of user groups
        /// </summary>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadByGroup(IADO ado,string grpCode, string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@GrpCode", value = grpCode },
                new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code") }

            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadByGroup", inputParams);
        }

        internal List<dynamic> ReadCollectionMetadata(IADO ado, string languageCode, DateTime DateFrom, string PrcCode = null, bool meta = true)
        {
            var inputParams = new List<ADO_inputParams>();

            inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") });

            if (!string.IsNullOrEmpty(languageCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeRead", value = languageCode });
            }

            if (DateFrom != default(DateTime))
            {
                inputParams.Add(new ADO_inputParams { name = "@DateFrom", value = DateFrom });
            }

            if (PrcCode != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@PrcCode", value = PrcCode });
            }

            ADO_readerOutput output;
            if (meta)
                output = ado.ExecuteReaderProcedure("Data_Release_ReadMetaCollection", inputParams);
            else
                output = ado.ExecuteReaderProcedure("Data_Release_ReadCollection", inputParams);

            if (output.hasData)
            {
                return output.data;
            }

            return null;
        }

        internal ADO_readerOutput DataMatrixReadLive(IADO ado, string mtrCode, string lngIsoCode)
        {
            string defaultLngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrCode",value=mtrCode},
                new ADO_inputParams(){ name="@LngIsoCode",value=lngIsoCode},
                new ADO_inputParams(){name ="@LngIsoCodeDefault", value=defaultLngIsoCode }
            };
            return ado.ExecuteReaderProcedure("Data_Matrix_Read_Live", inputParams);


        }

        internal ADO_readerOutput DataMatrixRead(IADO ado, int rlsCode, string lngIsoCode, string ccnUsername)
        {

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@RlsCode",value=rlsCode},
                new ADO_inputParams(){ name="@LngIsoCode",value=lngIsoCode},
                new ADO_inputParams(){ name="@CcnUsername",value=ccnUsername}
            };
            return ado.ExecuteReaderProcedure("Data_Matrix_Read", inputParams);


        }

        internal string getDbName()
        {
            return new SqlConnection(ApiServicesHelper.Configuration.GetConnectionString(ApiServicesHelper.ADOSettings.API_ADO_DEFAULT_CONNECTION)).Database;
        }

        /// <summary>
        /// Soft deletes a Matrix
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Delete(IADO ado,int rlsCode, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@RlsCode",value= rlsCode},
                    new ADO_inputParams() {name ="@CcnUsername",value= userName}
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Matrix_Delete", inputParams, ref returnParam);

            return Resources.DataAdaptor.ReadInt(returnParam.value);
        }
    }
}
