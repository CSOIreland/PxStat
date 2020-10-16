using API;
using PxStat.Security;
using System.Collections.Generic;

namespace PxStat.Workflow
{
    /// <summary>
    /// ADO for Workflow
    /// </summary>
    internal class Workflow_ADO
    {
        /// <summary>
        /// Read All Workflows
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadAll(ADO ado, dynamic dto, string ccnUsername)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername}
            };

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Workflow_Releases_Live_Read", inputParams);


            //return the list of entities that have been found
            return output;

        }

        /// <summary>
        /// Read Work in progress workflows
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadWorkInProgress(ADO ado, string userName, string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@CcnUsername", value = userName } ,
                new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode  },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetCustomConfig(ConfigType.global,"language.iso.code") }
            };

            var reader = ado.ExecuteReaderProcedure("Workflow_ReadWorkInProgress", inputParams);

            return reader;
        }

        /// <summary>
        /// Read workflows
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, Workflow_DTO dto, string ccnUsername)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode},
                new ADO_inputParams() { name="@CcnUsername", value=ccnUsername}

            };

            inputParams.Add(new ADO_inputParams() { name = "@WrqCurrentRequestOnly", value = dto.WrqCurrentFlagOnly });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Workflow_Read", inputParams);

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

        /// <summary>
        /// Read workflows that are awaiting response
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="ccnUsername"></param>
        /// <param name="rlsCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadAwaitingResponse(ADO ado, string ccnUsername, int rlsCode, string lngIsoCode)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@LngIsoCode",value= lngIsoCode},
                new ADO_inputParams() {name ="@LngIsoCodeDefault",value= Configuration_BSO.GetCustomConfig(ConfigType.global,"language.iso.code")},
            };

            if (rlsCode != default(int))
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsCode", value = rlsCode });
            }

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Workflow_ReadAwaitingResponse", inputParams);



            //return the list of entities that have been found
            return output;
        }

        internal ADO_readerOutput ReadLive(ADO ado, string ccnUsername, Workflow_DTO dto)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@LngIsoCode",value= dto.LngIsoCode },
                new ADO_inputParams() {name ="@LngIsoCodeDefault",value= Configuration_BSO.GetCustomConfig(ConfigType.global,"language.iso.code")}
            };

            if (dto.RlsCode != default(int))
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsCode", value = dto.RlsCode });
            }

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Workflow_ReadLive", inputParams);




            //return the list of entities that have been found
            return output;
        }

        internal ADO_readerOutput ReadPendingLive(ADO ado, string ccnUsername, Workflow_DTO dto)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@LngIsoCode",value= dto.LngIsoCode },
                new ADO_inputParams() {name ="@LngIsoCodeDefault",value= Configuration_BSO.GetCustomConfig(ConfigType.global,"language.iso.code")}
            };

            if (dto.RlsCode != default(int))
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsCode", value = dto.RlsCode });
            }

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Workflow_ReadPendingLive", inputParams);




            //return the list of entities that have been found
            return output;
        }

        /// <summary>
        /// Read workflows that are awaiting signoff
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="ccnUsername"></param>
        /// <param name="rlsCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadAwaitingSignoff(ADO ado, string ccnUsername, int rlsCode, string lngIsoCode)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@LngIsoCode",value= lngIsoCode},
                new ADO_inputParams() {name ="@LngIsoCodeDefault",value= Configuration_BSO.GetCustomConfig(ConfigType.global,"language.iso.code")}

            };

            if (rlsCode != default(int))
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsCode", value = rlsCode });
            }

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Workflow_ReadAwaitingSignoff", inputParams);




            //return the list of entities that have been found
            return output;
        }

        /// <summary>
        /// Read workflow history
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadHistory(ADO ado, int rlsCode, string userName)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= userName},
                new ADO_inputParams() {name="@RlsCode",value=rlsCode }
            };

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Workflow_ReadHistory", inputParams);

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
