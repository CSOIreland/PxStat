using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Updates a Reason
    /// </summary>
    internal class Reason_BSO_Update : BaseTemplate_Update<Reason_DTO_Update, Reason_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Reason_BSO_Update(JSONRPC_API request) : base(request, new Reason_VLD_Update())
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
            var adoReason = new Reason_ADO();

            if (DTO.LngIsoCode == Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code")) // we are updating the Reason in the default language
            {
                //Update the Reason - and retrieve the number of updated rows
                int nUpdated = adoReason.Update(Ado, DTO, SamAccountName);
                if (nUpdated == 0)
                {
                    Log.Instance.Debug("Can't update Reason");
                    Response.error = Label.Get("error.update");
                    return false;
                }
            }
            else //we are updating the Reason in another valid language
            {
                int nUpdated = adoReason.CreateOrUpdateReasonLanguage(Ado, DTO);
                if (nUpdated == 0)
                {
                    Log.Instance.Debug("Can't create/update Reason Language");
                    Response.error = Label.Get("error.update");
                    return false;
                }
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}
