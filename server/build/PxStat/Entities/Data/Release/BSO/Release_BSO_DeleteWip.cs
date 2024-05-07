using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Delete a Work in Progress Release
    /// </summary>
    internal class Release_BSO_DeleteWip : BaseTemplate_Delete<Release_DTO_Delete, Release_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_DeleteWip(JSONRPC_API request) : base(request, new Release_VLD_Delete())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Release_ADO adoRelease = new Release_ADO(Ado);
            if (adoRelease.IsWip(DTO.RlsCode))
            {
                Release_BSO_Delete deleteBso = new Release_BSO_Delete();
                if (deleteBso.Delete(Ado, DTO.RlsCode, SamAccountName, false) == 0)
                {
                    Log.Instance.Debug("Can't delete Release");
                    Response.error = Label.Get("error.delete");
                    return false;
                }

                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;

            }
            else
            {
                Log.Instance.Debug("Release can't be deleted because it is not WIP");
                return false;
            }
        }


    }
}