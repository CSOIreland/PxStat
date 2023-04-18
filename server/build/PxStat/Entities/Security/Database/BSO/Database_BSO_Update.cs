using API;
using PxStat.Template;

namespace PxStat.Security
{
    internal class Database_BSO_Update : BaseTemplate_Read<Database_DTO_Update, Database_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Database_BSO_Update(JSONRPC_API request) : base(request, new Database_VLD_Update())
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


            adoDatabase.UpdateIndexes(Ado);


            Response.data = JSONRPC.success;
            return true;

        }


    }


}
