using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Creates a Keyword Release
    /// </summary>
    internal class Keyword_Release_BSO_Create : BaseTemplate_Create<Keyword_Release_DTO, Keyword_Release_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Keyword_Release_BSO_Create(JSONRPC_API request) : base(request, new Keyword_Release_VLD_Create())
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
            var adoKeywordRelease = new Keyword_Release_ADO();

            if (DTO.LngIsoCode != null)
            {
                Keyword_BSO_Extract bse = new Keyword_BSO_Extract(DTO.LngIsoCode);
                DTO.KrlValue = bse.Singularize(DTO.KrlValue);
            }

            //Create the Keyword_Subject - and retrieve the newly created Id
            int newId = adoKeywordRelease.Create(Ado, DTO);
            if (newId == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }
            else if (newId < 0)
            {
                Response.error = Label.Get("error.duplicate");
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}
