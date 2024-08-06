using API;
using PxStat.Template;
using System.Linq;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Read one or more subjects
    /// </summary>
    internal class Subject_BSO_Read : BaseTemplate_Read<Subject_DTO, Subject_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subject_BSO_Read(IRequest request) : base(request, new Subject_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            var adoSubject = new Subject_ADO(Ado);
            var list = adoSubject.Read(DTO);
            Response.data = list.ToList().OrderBy(x => x.SbjValue); 

            return true;
        }
    }
}
