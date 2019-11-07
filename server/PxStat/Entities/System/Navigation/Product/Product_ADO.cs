using System.Collections.Generic;
using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// ADO methods for Product
    /// </summary>
    internal class Product_ADO
    {
        /// <summary>
        /// ADO class parameter
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Product_ADO(ADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Creates a product
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Create(Product_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@PrcValue",value= dto.PrcValue},
                    new ADO_inputParams() {name ="@PrcCode",value= dto.PrcCode},
                    new ADO_inputParams() {name ="@SbjCode",value= dto.SbjCode},
                    new ADO_inputParams() {name="@LngIsoCode",value=dto.LngIsoCode },
                     new ADO_inputParams() {name ="@userName",value= userName},
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Product_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        /// Reads one or more products
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal List<dynamic> Read(Product_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>();
            if (dto.PrcCode != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@PrcCode", value = dto.PrcCode });
            }

            if (dto.SbjCode != default(int))
            {
                inputParams.Add(new ADO_inputParams { name = "@SbjCode", value = dto.SbjCode });
            }

            if (dto.LngIsoCode != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode });
            }

            var reader = ado.ExecuteReaderProcedure("System_Navigation_Product_Read", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Tests if the Product Code exists already in the database
        /// </summary>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal bool ExistsCode(string prcCode)
        {
            var inputParams = new List<ADO_inputParams>() { new ADO_inputParams { name = "@PrcCode", value = prcCode } };
            var reader = ado.ExecuteReaderProcedure("System_Navigation_Product_Read", inputParams);
            return reader.hasData;
        }

        /// <summary>
        /// Tests if a product exists with the supplied PrcValue
        /// </summary>
        /// <param name="prcValue"></param>
        /// <returns></returns>
        internal bool Exists(string prcValue)
        {
            var inputParams = new List<ADO_inputParams>() { new ADO_inputParams { name = "@PrcValue", value = prcValue } };
            var reader = ado.ExecuteReaderProcedure("System_Navigation_Product_Read", inputParams);
            return reader.hasData;
        }

        /// <summary>
        /// A version of Exists more suitable for update. It tests whether the PrcValue exists on a product but
        /// ignores the product we're trying to update.
        /// </summary>
        /// <param name="prcValue"></param>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal bool Exists(string prcValue, string prcCode)
        {
            var inputParams = new List<ADO_inputParams>() { new ADO_inputParams { name = "@PrcValue", value = prcValue }, };
            var reader = ado.ExecuteReaderProcedure("System_Navigation_Product_Read", inputParams);
            return reader.data.Find(e => e.PrcValue == prcValue && e.PrcCode != prcCode) != null;
        }

        internal bool Exists(string prcValue, int sbjCode)
        {
            var inputParams = new List<ADO_inputParams>() { new ADO_inputParams { name = "@PrcValue", value = prcValue },
            new ADO_inputParams { name = "@SbjCode", value = sbjCode }
            };
            var reader = ado.ExecuteReaderProcedure("System_Navigation_Product_Read", inputParams);
            return reader.data.Find(e => e.PrcValue == prcValue && e.SbjCode != sbjCode) != null;
        }


        /// <summary>
        /// Updates a product
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="IsDefault"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Update(Product_DTO dto, bool IsDefault, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@PrcCode",value= dto.PrcCode},
                    new ADO_inputParams() {name ="@PrcCodeNew",value= dto.PrcCodeNew},
                    new ADO_inputParams() {name ="@SbjCode",value= dto.SbjCode},
                    new ADO_inputParams() {name ="@PrcValue",value= dto.PrcValue},
                     new ADO_inputParams() {name ="@userName",value= userName},
                     new ADO_inputParams() {name ="@IsDefault",value= IsDefault},

                };

            var outputParam = new ADO_outputParam() { name = "@ProductID", value = 0 };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Product_Update", inputParams, ref returnParam, ref outputParam);

            return (int)outputParam.value;
        }

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Delete(Product_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@PrcCode",value= dto.PrcCode},
                     new ADO_inputParams() {name ="@userName",value= userName},
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Product_Delete", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

    }
}