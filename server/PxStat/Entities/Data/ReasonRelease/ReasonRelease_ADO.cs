using API;
using System.Collections.Generic;


namespace PxStat.Data
{
    /// <summary>
    /// ADO methods for ReasonRelease
    /// </summary>
    internal class ReasonRelease_ADO
    {
        /// <summary>
        /// Reads a ReasonRelease entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reasonRelease"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, ReasonRelease_DTO_Read reasonRelease)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RlsCode",value=reasonRelease.RlsCode},
                new ADO_inputParams() {name= "@LngIsoCode",value=reasonRelease.LngIsoCode}
            };


            if (reasonRelease.RsnCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@RsnCode", value = reasonRelease.RsnCode });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Data_Reason_Release_Read", inputParamList);

            //return the list of entities that have been found
            return output;
        }

        /// <summary>
        /// Reads a ReasonRelease entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reasonRelease"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(IADO ado, ReasonRelease_DTO_Read reasonRelease)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RlsCode",value=reasonRelease.RlsCode},
                new ADO_inputParams() {name= "@LngIsoCode",value=reasonRelease.LngIsoCode}
            };


            if (reasonRelease.RsnCode != null)
                inputParamList.Add(new ADO_inputParams() { name = "@RsnCode", value = reasonRelease.RsnCode });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Data_Reason_Release_Read", inputParamList);

            //return the list of entities that have been found
            return output;
        }


        /// <summary>
        /// Creates a ReasonRelease entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reasonRelease"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Create(ADO ado, ReasonRelease_DTO_Create reasonRelease, string userName)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RlsCode",value=reasonRelease.RlsCode},
                new ADO_inputParams() {name= "@RsnCode",value=reasonRelease.RsnCode},

                new ADO_inputParams() {name= "@CcnUsername",value=userName},
            };

            if (reasonRelease.CmmValue != null)
                inputParamList.Add(new ADO_inputParams() { name = "@CmmValue", value = reasonRelease.CmmValue });

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam
            {
                name = "return",
                value = 0
            };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Data_Reason_Release_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Updates a ReasonRelease entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reasonRelease"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Update(ADO ado, ReasonRelease_DTO_Update reasonRelease, string userName)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RlsCode",value=reasonRelease.RlsCode},
                new ADO_inputParams() {name= "@RsnCode",value=reasonRelease.RsnCode},
                new ADO_inputParams() {name= "@CcnUsername",value=userName},
            };

            if (reasonRelease.CmmValue != null)
                inputParamList.Add(new ADO_inputParams() { name = "@CmmValue", value = reasonRelease.CmmValue });


            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam
            {
                name = "return",
                value = 0
            };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Data_Reason_Release_Update", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Deletes a ReasonRelease entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reasonRelease"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Delete(ADO ado, ReasonRelease_DTO_Delete reasonRelease, string userName)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RlsCode",value=reasonRelease.RlsCode},
                new ADO_inputParams() {name= "@RsnCode",value=reasonRelease.RsnCode},
                new ADO_inputParams() {name= "@CcnUsername",value=userName},
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Data_Reason_Release_Delete", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }
    }
}
