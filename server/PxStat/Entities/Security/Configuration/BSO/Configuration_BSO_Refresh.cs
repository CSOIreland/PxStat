using API;
using PxStat.Template;

namespace PxStat.Security
{
    internal class Configuration_BSO_Refresh : BaseTemplate_Update<Configuration_DTO_Update, Configuration_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Configuration_BSO_Refresh(JSONRPC_API request) : base(request, new Configuration_VLD_Update())
        {
        }

        /// <summary>
        /// Test privileges
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

            if (Configuration_BSO.UpdateStaticConfig())
            {
                Response.data = JSONRPC.success;
                return true;
            }
            Response.error = Label.Get("error.update");
            return false;
        }
    }
}
