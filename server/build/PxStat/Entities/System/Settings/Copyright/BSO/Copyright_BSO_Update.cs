using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Updates a Copyright
    /// </summary>
    internal class Copyright_BSO_Update : BaseTemplate_Update<Copyright_DTO_Update, Copyright_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Copyright_BSO_Update(JSONRPC_API request) : base(request, new Copyright_VLD_Update())
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
            Copyright_ADO adoCopyright = new Copyright_ADO();

            //First we must check if the amended Copyright code exists already (we can't have duplicates)
            if (!DTO.CprCodeNew.Equals(DTO.CprCodeOld))
            {
                if (adoCopyright.Exists(Ado, DTO.CprCodeNew))
                {
                    //The proposed new copyright code exists already, we can't proceed
                    Log.Instance.Debug("Copyright code exists already - create request refused");
                    Response.error = Label.Get("error.duplicate");
                    return false;
                }
            }

            //Update and retrieve the number of updated rows
            int nUpdated = adoCopyright.Update(Ado, DTO, SamAccountName);

            if (nUpdated == 0)
            {
                Log.Instance.Debug("Can't update Copyright");
                Response.error = Label.Get("error.update");
                return true;
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}

