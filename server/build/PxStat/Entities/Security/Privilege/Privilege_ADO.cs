using API;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// IADO methods for Privilege
    /// </summary>
    internal class Privilege_ADO
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal Privilege_ADO() { }

        /// <summary>
        /// Reads a Privilege entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="privilege"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(IADO ado, Privilege_DTO privilege)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            if (!string.IsNullOrEmpty(privilege.PrvCode))
                paramList.Add(new ADO_inputParams() { name = "@PrvCode", value = privilege.PrvCode });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Security_Privilege_Read", paramList);

            //return the list of entities that have been found
            return output;
        }
    }
}