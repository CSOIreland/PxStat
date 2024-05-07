using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Creates a KeywordProduct
    /// </summary>
    internal class Keyword_Product_BSO_Create : BaseTemplate_Create<Keyword_Product_DTO, Keyword_Product_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Keyword_Product_BSO_Create(JSONRPC_API request) : base(request, new Keyword_Product_VLD_Create())
        {
        }

        /// <summary>
        /// Test Privilege
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
            var adoKeyword_Product = new Keyword_Product_ADO(Ado);

            //Create the Keyword_Product - and retrieve the newly created Id
            int newId = adoKeyword_Product.Create(DTO);
            if (newId == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }
            else if (newId < 0)
            {
                Response.error = Label.Get("error.duplicate");
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}

