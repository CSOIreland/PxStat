using API;
using PxStat.Template;
namespace PxStat.Security
{
    internal class Database_BSO_Read : BaseTemplate_Read<Database_DTO_Read, Database_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Database_BSO_Read(JSONRPC_API request) : base(request, new Database_VLD_Read())
        { }


        /// <summary>
        /// Test Privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }
        protected override bool Execute()
        {
            Database_ADO adoDatabase = new Database_ADO();


            ADO_readerOutput result;

            result = adoDatabase.Read(Ado, DTO);
            if (!result.hasData)
            {
                return false;
            }

            Response.data = result.data;

            return true;
        }
    }
}
