using API;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// IADO class for Keyword Subject
    /// </summary>
    internal class Keyword_Subject_ADO
    {
        /// <summary>
        /// Class variable IADO
        /// </summary>
        private IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Keyword_Subject_ADO(IADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Creates a keyword subject from a DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int Create(Keyword_Subject_DTO dto)
        {
            if (dto.LngIsoCode != null)
            {
                Keyword_BSO_Extract bse = new Keyword_BSO_Extract(dto.LngIsoCode);
                dto.KsbValue = bse.Singularize(dto.KsbValue);
            }
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@KsbValue",value= dto.KsbValue},
                     new ADO_inputParams() {name ="@SbjCode",value= dto.SbjCode},
                     new ADO_inputParams() {name ="@KsbMandatoryFlag",value= dto.KsbMandatoryFlag!=null?dto.KsbMandatoryFlag:false},
                     new ADO_inputParams() {name ="@KsbSingularisedFlag",value= dto.LngIsoCode!=null }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Keyword_Subject_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        /// Creates a keyword subject from a data table
        /// </summary>
        /// <param name="dt"></param>
        internal void Create(DataTable dt)
        {
            //Before bulk inserting, we must map our datatable to the database table, column by column
            var maps = new List<KeyValuePair<string,string>>()
                {
                    new KeyValuePair<string,string>("KSB_VALUE", "KSB_VALUE"),
                    new KeyValuePair<string,string>("KSB_SBJ_ID", "KSB_SBJ_ID"),
                    new KeyValuePair<string,string>("KSB_MANDATORY_FLAG", "KSB_MANDATORY_FLAG")
                };
            using (IADO bulkAdo = AppServicesHelper.StaticADO)
            {
                bulkAdo.ExecuteBulkCopy("TD_KEYWORD_SUBJECT", maps, dt, false);
            }
        }

        /// <summary>
        /// Reads Keyword subjects
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal List<dynamic> Read(Keyword_Subject_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>();
            if (dto.KsbCode != default(int))
            {
                inputParams.Add(new ADO_inputParams { name = "@KsbCode", value = dto.KsbCode });
            }

            if (dto.SbjCode != default(int))
            {
                inputParams.Add(new ADO_inputParams { name = "@SbjCode", value = dto.SbjCode });
            }

            var reader = ado.ExecuteReaderProcedure("System_Navigation_Keyword_Subject_Read", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Updates a Keyword Subject
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int Update(Keyword_Subject_DTO dto)
        {
            if (dto.LngIsoCode != null)
            {
                Keyword_BSO_Extract bse = new Keyword_BSO_Extract(dto.LngIsoCode);
                dto.KsbValue = bse.Singularize(dto.KsbValue);
            }

            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@KsbCode",value= dto.KsbCode},
                     new ADO_inputParams() {name ="@KsbValue",value= dto.KsbValue},
                     new ADO_inputParams() {name ="@KsbMandatoryFlag",value= dto.KsbMandatoryFlag!=null?dto.KsbMandatoryFlag:false},
                     new ADO_inputParams() {name ="@KsbSingularisedFlag",value= dto.LngIsoCode!=null}
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Keyword_Subject_Update", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        /// Deletes a Keyword Subject
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="mandatoryOnly"></param>
        /// <returns></returns>
        internal int Delete(Keyword_Subject_DTO dto, bool? mandatoryOnly = null)
        {
            var inputParams = new List<ADO_inputParams>();
            if (dto.KsbCode != default(int))
                inputParams.Add(new ADO_inputParams() { name = "@KsbCode", value = dto.KsbCode });
            if (dto.SbjCode != default(int))
                inputParams.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });
            if (mandatoryOnly != null)
                inputParams.Add(new ADO_inputParams() { name = "@KsbMandatoryFlag", value = mandatoryOnly });

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Keyword_Subject_Delete", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

    }
}
