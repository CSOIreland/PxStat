using API;
using System.Collections.Generic;

namespace PxStat.System.Settings
{
    /// <summary>
    /// ADO for Format
    /// </summary>
    internal class Format_ADO
    {
        /// <summary>
        /// Read the Formats. If either parameter is null then all data for the other parameter is returned
        /// If both parameters are null then all data is returned
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, Format_DTO_Read format)
        {
            ADO_readerOutput output = new ADO_readerOutput();
            var paramList = new List<ADO_inputParams>();
            if (!string.IsNullOrEmpty(format.FrmType))
                paramList.Add(new ADO_inputParams() { name = "@FrmType", value = format.FrmType });
            if (!string.IsNullOrEmpty(format.FrmVersion))
                paramList.Add(new ADO_inputParams() { name = "@FrmVersion", value = format.FrmVersion });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("System_Settings_Format_Read", paramList);

            //Read the result of the call to the database
            if (output.hasData)
            {
                Log.Instance.Debug("Data found");
            }
            else
            {
                //No data found
                Log.Instance.Debug("No data found");
            }

            //return the list of entities that have been found
            return output;
        }
    }
}