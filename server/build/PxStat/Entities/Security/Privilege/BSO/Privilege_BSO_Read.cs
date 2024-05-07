using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Reads privilege data
    /// </summary>
    internal class Privilege_BSO_Read : BaseTemplate_Read<Privilege_DTO, Privilege_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Privilege_BSO_Read(JSONRPC_API request) : base(request, new Privilege_VLD())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoPrivilege = new Privilege_ADO();

            //Privileges are returned as an IADO result
            ADO_readerOutput result = adoPrivilege.Read(Ado, DTO);

            if (!result.hasData)
            {
                return false;
            }

            Response.data = result.data;
            return true;
        }
    }
}

