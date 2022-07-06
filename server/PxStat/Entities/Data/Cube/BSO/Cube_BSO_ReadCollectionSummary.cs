using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class Cube_BSO_ReadCollectionSummary : BaseTemplate_Read<Cube_DTO_ReadCollectionSummary, Cube_VLD_ReadCollectionSummary>
    {
        bool _meta;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadCollectionSummary(IRequest request, bool meta = true) : base(request, new Cube_VLD_ReadCollectionSummary())
        {
            _meta = meta;
        }

        /// <summary>
        /// Test authentication
        /// </summary>
        /// <returns></returns>
        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //so that caches don't get mixed up..
            DTO.meta = _meta;


            Cube_BSO cBso = new Cube_BSO(Ado);

            // cache store is done in the following function
            //Response.data = cBso.ExecuteReadCollection(Ado, DTO, _meta);

            Response.data = cBso.ReadCollection(DTO.language, DTO.subject.ToString(), DTO.product);

            return true;
        }


    }
}
