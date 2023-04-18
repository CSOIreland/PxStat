using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Update a Keyword Release
    /// </summary>
    internal class Keyword_Release_BSO_Update : BaseTemplate_Update<Keyword_Release_DTO, Keyword_Release_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Keyword_Release_BSO_Update(JSONRPC_API request) : base(request, new Keyword_Release_VLD_Update())
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
            Keyword_Release_ADO adoKeywordRelease = new Keyword_Release_ADO();
            //You can't change the mandatory flag
            DTO.KrlMandatoryFlag = false;
            if (DTO.LngIsoCode != null)
            {
                Keyword_BSO_Extract bse = new Keyword_BSO_Extract(DTO.LngIsoCode);
                DTO.KrlValue = bse.Singularize(DTO.KrlValue);
            }
            int updated = adoKeywordRelease.Update(Ado, DTO);
            if (updated == 0)
            {
                Log.Instance.Debug("No record found for update request");
                Response.error = Label.Get("error.delete");
                return false;
            }
            else if (updated < 0)
            {
                Response.error = Label.Get("error.duplicate");
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}
