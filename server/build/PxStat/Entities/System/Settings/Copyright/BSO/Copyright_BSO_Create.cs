using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Create a Copyright
    /// </summary>
    internal class Copyright_BSO_Create : BaseTemplate_Create<Copyright_DTO_Create, Copyright_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Copyright_BSO_Create(JSONRPC_API request) : base(request, new Copyright_VLD_Create())
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
            var adoCopyright = new Copyright_ADO();

            //First we must check if the group exists already (we can't have duplicates)
            if (adoCopyright.Exists(Ado, DTO.CprCode))
            {
                //This copyright exists already, we can't proceed
                Log.Instance.Debug("Copyright exists already - create request refused");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the copyright - and retrieve the newly created Id
            int newId = adoCopyright.Create(Ado, DTO, SamAccountName);

            if (newId == 0)
            {
                Log.Instance.Debug("Can't create copyright");
                Response.error = Label.Get("error.create");
                return false;
            }
            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}

