using API;
using PxStat.Template;


namespace PxStat.Config
{
    /// <summary>
    /// 
    /// </summary>
    internal class Config_BSO_Update : BaseTemplate_Create<Config_DTO_Create, Config_VLD_Create>
    {
        private string result;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal Config_BSO_Update(JSONRPC_API request) : base(request, new Config_VLD_Create())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Config_BSO cBso = new Config_BSO();
            Config_VLD_Create cVldCreate = new Config_VLD_Create();
            Config_ADO cAdo = new Config_ADO(Ado);
            result = cAdo.Create(DTO, cVldCreate, SamAccountName);
            return true;
        }

        public override bool PostExecute()
        {
            ApiServicesHelper.ApiConfiguration.Refresh();

            if (string.IsNullOrEmpty(result))
            {
                return false;
            }
            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}

