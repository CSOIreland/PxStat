using API;
using PxStat.Resources;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Creates a ReleaseProduct
    /// </summary>
    internal class ReleaseProductAssociation_BSO_Create : BaseTemplate_Create<ReleaseProduct_DTO_Create, ReleaseProductAssociation_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal ReleaseProductAssociation_BSO_Create(JSONRPC_API request) : base(request, new ReleaseProductAssociation_VLD_Create())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoReleaseProduct = new ReleaseProductAssocaition_ADO();

            // Create the Release Product association - and retrieve the newly created Id
            int newId = adoReleaseProduct.Create(Ado, DTO, SamAccountName);

            switch (newId)
            {
                case -3:
                    Log.Instance.Debug("Cannot create the Release Product association because the Release Code does not exist");
                    Response.error = Label.Get("error.create");
                    return false;

                case -2:
                    Log.Instance.Debug("Cannot create the Release Product association because the Product Code does not exist");
                    Response.error = Label.Get("error.create");
                    return false;

                case -1:
                    Log.Instance.Debug("Cannot create Release Product association because the Product is already associated with the Release");
                    Response.error = Label.Get("error.duplicate");
                    return false;

                case 0:
                    Log.Instance.Debug("Cannot create the Release Product association because the is no core Product associated with the Release");
                    Response.error = Label.Get("error.create");
                    return false;
            }

            //Flush the cache for search 
           Cas.RunCasFlush(Resources.Constants.C_CAS_NAVIGATION_SEARCH);

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}
