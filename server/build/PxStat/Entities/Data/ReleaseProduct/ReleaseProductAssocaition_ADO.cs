using System.Collections.Generic;
using API;
using Newtonsoft.Json;
using System.Data;
using System;

namespace PxStat.Data
{
    /// <summary>
    /// IADO classes for associationDto
    /// </summary>
    internal class ReleaseProductAssocaition_ADO
    {
        /// <summary>
        /// Reads a Release Product
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(IADO ado, ReleaseProduct_DTO_Read dto, string userName)
        {
            ADO_readerOutput output = new ADO_readerOutput();
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@ReleaseCode", value = dto.RlsCode });
            paramList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode });

            // Get the Products associated with the Release
            output = ado.ExecuteReaderProcedure("Data_ReleaseProduct_Read", paramList);
            return output;
        }

        /// <summary>
        /// Creates a new ReleaseProduct
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>

        /// <returns></returns>
        internal int Create(IADO ado, ReleaseProduct_DTO_Create dto, string username)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@ReleaseCode",value=dto.RlsCode},
                new ADO_inputParams() {name= "@ProductCode",value=dto.PrcCode},
                new ADO_inputParams() {name= "@username",value=username},
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Data_ReleaseProduct_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }
        /// <summary>
        /// Deletes a ReleaseProduct
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="associationDto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(IADO ado, ReleaseProductAssociation_DTO_Delete associationDto, string username)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@ReleaseCode",value=associationDto.RlsCode},
                new ADO_inputParams() {name= "@ProductCode",value=associationDto.PrcCode},
                new ADO_inputParams() {name= "@username",value=username},
            };

            //Create a return parameter
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "@return";
            retParam.value = 0;

            // Executing the stored procedure
            ado.ExecuteNonQueryProcedure("Data_ReleaseProduct_Delete", inputParamList, ref retParam);

            Log.Instance.Debug("Number of records affected: " + retParam.value);
            return retParam.value;
        }

        /// <summary>
        /// Checks if a ReleaseProduct combination already exists in the database
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal bool Exists(IADO ado, ReleaseProduct_DTO_Create dto)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@RlsCode", value = dto.RlsCode },
                new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode },
            };

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Data_ReleaseProduct_Read", paramList);
            return output.hasData;
        }
    }
}