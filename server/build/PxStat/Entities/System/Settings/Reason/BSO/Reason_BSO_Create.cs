using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Create a Reason
    /// </summary>
    internal class Reason_BSO_Create : BaseTemplate_Create<Reason_DTO_Create, Reason_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Reason_BSO_Create(JSONRPC_API request) : base(request, new Reason_VLD_Create())
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
            var adoReason = new Reason_ADO();

            //First we must check if the Reason exists already (we can't have duplicates)
            if (adoReason.Exists(Ado, DTO.RsnCode))
            {
                //This Reason exists already, we can't proceed
                Log.Instance.Debug("Reason exists already - create request refused");
                Response.error = Label.Get("error.duplicate");

                return false;
            }

            //Create the Reason - and retrieve the newly created Id
            int newId = adoReason.Create(Ado, DTO, SamAccountName);
            if (newId == 0)
            {
                Log.Instance.Debug("Can't create Reason");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}