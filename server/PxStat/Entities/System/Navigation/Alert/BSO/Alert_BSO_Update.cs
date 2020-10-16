using API;
using PxStat.Entities.System.Navigation.AlertLanguage;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Update an alert
    /// </summary>
    internal class Alert_BSO_Update : BaseTemplate_Update<Alert_DTO, Alert_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Alert_BSO_Update(JSONRPC_API request) : base(request, new Alert_VLD_Update())
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
            Alert_ADO adoAlert = new Alert_ADO();

            int updated = 0;

            if (DTO.LngIsoCode == Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code"))
            {
                updated = adoAlert.Update(Ado, DTO, SamAccountName);
            }
            else
            {
                //Update or create in a language alternate version
                AlertLanguage_BSO bso = new AlertLanguage_BSO();
                updated = bso.CreateOrUpdate(DTO, Ado);
            }

            if (updated == 0)
            {
                Log.Instance.Debug("Error updating Alert");
                Response.error = Label.Get("error.update");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }

    }

}
