using API;
using System.Collections.Generic;

namespace Px5Migrator
{
    internal class GeoMap_ADO
    {
        /// <summary>
        /// ADO class parameter
        /// </summary>
        private ADO ado;

        internal GeoMap_ADO(ADO ado)
        {
            this.ado = ado;
        }





        internal ADO_readerOutput Read(string gmpCode)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@GmpCode",value=gmpCode}
            };



            var reader = ado.ExecuteReaderProcedure("Data_GeoMap_Read", inputParamList);

            return reader;

        }





    }
}
