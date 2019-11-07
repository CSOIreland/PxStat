using API;
using PxStat.Template;


namespace PxStat.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Classification_BSO_Read : BaseTemplate_Read<Classification_DTO_Read, Classification_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Classification_BSO_Read(JSONRPC_API request) : base(request, new Classification_VLD_Read())
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

            ADO_readerOutput outputDetails = ClassificationAdo.Read(DTO);


            Response.data = outputDetails.data;

            if (outputDetails.hasData)
            {
                return true;
            }

            return false;

        }


    }
}