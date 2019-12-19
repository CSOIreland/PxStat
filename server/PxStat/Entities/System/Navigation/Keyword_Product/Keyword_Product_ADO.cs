using API;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// ADO class for Keyword Product
    /// </summary>
    internal class Keyword_Product_ADO
    {
        /// <summary>
        /// ADO class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="ado"></param>
        public Keyword_Product_ADO(ADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Creates a Keyword Product from a DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int Create(Keyword_Product_DTO dto)
        {
            if (dto.LngIsoCode != null)
            {
                Keyword_BSO_Extract bse = new Keyword_BSO_Extract(dto.LngIsoCode);
                dto.KprValue = bse.Singularize(dto.KprValue);
            }

            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@KprValue",value= dto.KprValue},
                     new ADO_inputParams() {name ="@PrcCode",value= dto.PrcCode},
                     new ADO_inputParams() {name ="@KprMandatoryFlag",value= dto.KprMandatoryFlag!=null? dto.KprMandatoryFlag:false},
                     new ADO_inputParams() {name ="@KprSingularisedFlag",value= dto.LngIsoCode!=null}
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Keyword_Product_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        /// Creates a KeywordProduct from a data table
        /// </summary>
        /// <param name="dt"></param>
        internal void Create(DataTable dt)
        {
            //Before bulk inserting, we must map our datatable to the database table, column by column
            var maps = new List<SqlBulkCopyColumnMapping>()
                {
                    new SqlBulkCopyColumnMapping("KPR_VALUE", "KPR_VALUE"),
                    new SqlBulkCopyColumnMapping("KPR_PRC_ID", "KPR_PRC_ID"),
                    new SqlBulkCopyColumnMapping("KPR_MANDATORY_FLAG", "KPR_MANDATORY_FLAG")
                };

            ado.ExecuteBulkCopy("TD_KEYWORD_PRODUCT", maps, dt, true);
        }

        /// <summary>
        /// Reads a list of KeywordProducts
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal List<dynamic> Read(Keyword_Product_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>();
            if (dto.KprCode != default(int))
            {
                inputParams.Add(new ADO_inputParams { name = "@KprCode", value = dto.KprCode });
            }

            if (dto.PrcCode != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@PrcCode", value = dto.PrcCode });
            }

            if (dto.SbjCode != default(int))
            {
                inputParams.Add(new ADO_inputParams { name = "@SbjCode", value = dto.SbjCode });
            }

            var reader = ado.ExecuteReaderProcedure("System_Navigation_Keyword_Product_Read", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Updates a KeywordProduct
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int Update(Keyword_Product_DTO dto)
        {
            if (dto.LngIsoCode != null)
            {
                Keyword_BSO_Extract bse = new Keyword_BSO_Extract(dto.LngIsoCode);
                dto.KprValue = bse.Singularize(dto.KprValue);
            }
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@KprCode",value= dto.KprCode},
                     new ADO_inputParams() {name ="@KprValue",value= dto.KprValue},
                     new ADO_inputParams() {name ="@KprMandatoryFlag",value= dto.KprMandatoryFlag!=null? dto.KprMandatoryFlag:false},
                     new ADO_inputParams() {name ="@KprSingularisedFlag",value= dto.LngIsoCode!=null }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Keyword_Product_Update", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        /// Deletes a KeywordProduct
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="mandatoryOnly"></param>
        /// <returns></returns>
        internal int Delete(Keyword_Product_DTO dto, bool? mandatoryOnly = null)
        {
            var inputParams = new List<ADO_inputParams>();

            if (dto.KprCode != default(int))
                inputParams.Add(new ADO_inputParams() { name = "@KprCode", value = dto.KprCode });

            if (dto.PrcCode != null)
                inputParams.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });

            if (mandatoryOnly != null)
                inputParams.Add(new ADO_inputParams() { name = "@KprMandatoryFlag", value = mandatoryOnly });

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Keyword_Product_Delete", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

    }
}
