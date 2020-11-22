using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Deletes all performance entries
    /// </summary>
    internal class Performance_BSO_Delete : BaseTemplate_Delete<Performance_DTO_Delete, Performance_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Performance_BSO_Delete(JSONRPC_API request) : base(request, new Performance_VLD_Delete())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator(); ;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoPerformance = new Performance_ADO();

            int commentCode = adoPerformance.Delete(Ado, DTO, SamAccountName);

            if (commentCode == 0)
            {
                //Can't delete performance entries so we can't proceed
                Log.Instance.Debug("Can't delete  Performance - request refused");
                Response.error = Label.Get("error.delete", DTO.LngIsoCode);
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}
