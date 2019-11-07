using API;
using Newtonsoft.Json;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Reads a Trace. Required parameters StartDate,EndDate and optionally AuthenticationType
    /// </summary>
    internal class Trace_BSO_Read : BaseTemplate_Read<Trace_DTO_Read, Trace_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Trace_BSO_Read(JSONRPC_API request) : base(request, new Trace_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoTrace = new Trace_ADO();

            //Traces are returned as an ADO result
            ADO_readerOutput result = adoTrace.Read(Ado, DTO);

            if (!result.hasData)
            {
                return false;
            }

            Log.Instance.Debug("Data found :" + JsonConvert.SerializeObject(result));
            Response.data = result.data;

            return true;
        }
    }
}