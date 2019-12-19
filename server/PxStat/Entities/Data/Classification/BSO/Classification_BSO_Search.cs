using API;
using PxStat.Template;


namespace PxStat.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Classification_BSO_Search : BaseTemplate_Read<Classification_DTO_Search, Classification_VLD_Search>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Classification_BSO_Search(JSONRPC_API request) : base(request, new Classification_VLD_Search())
        {
        }

        /// <summary>
        /// Test privileges
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
            var ClassificationAdo = new Classification_ADO(Ado);

            ADO_readerOutput outputDetails = ClassificationAdo.Search(DTO);


            Response.data = outputDetails.data;

            if (outputDetails.hasData)
            {
                return true;
            }

            return false;

        }


    }
}
