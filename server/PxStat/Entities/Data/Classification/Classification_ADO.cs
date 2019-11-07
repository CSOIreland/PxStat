using API;
using System.Collections.Generic;


namespace PxStat.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Classification_ADO
    {
        /// <summary>
        /// ADO class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Classification_ADO(ADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Search method
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput Search(Classification_DTO_Search dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@Search",value= dto.Search}
                };

            if (dto.LngIsoCode != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode });
            }

            var reader = ado.ExecuteReaderProcedure("Data_Classification_Search", inputParams);
            return reader;
        }

        /// <summary>
        /// Read method
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(Classification_DTO_Read dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ClsID",value= dto.ClsID}
                };
            var reader = ado.ExecuteReaderProcedure("Data_Classification_Read", inputParams);
            return reader;
        }
    }
}