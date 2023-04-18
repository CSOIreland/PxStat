using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Reads a Keyword Subject
    /// </summary>
    internal class Keyword_Subject_BSO_Read : BaseTemplate_Read<Keyword_Subject_DTO, Keyword_Subject_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Keyword_Subject_BSO_Read(JSONRPC_API request) : base(request, new Keyword_Subject_VLD_Read())
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
            var adoKeyword_Subject = new Keyword_Subject_ADO(Ado);
            var list = adoKeyword_Subject.Read(DTO);
            Response.data = list;

            return true;
        }
    }
}

