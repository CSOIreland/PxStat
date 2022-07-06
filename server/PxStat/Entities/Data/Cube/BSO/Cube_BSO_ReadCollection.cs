using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a list of current releases
    /// </summary>
    internal class Cube_BSO_ReadCollection : BaseTemplate_Read<Cube_DTO_ReadCollection, Cube_VLD_ReadCollection>
    {
        bool _meta;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadCollection(IRequest request, bool meta = true) : base(request, new Cube_VLD_ReadCollection())
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

            //See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadCollection", DTO);



            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }


            Cube_BSO cBso = new Cube_BSO();

            // cache store is done in the following function
            //Response.data = cBso.ExecuteReadCollection(Ado, DTO, _meta);


            Dcollection dc = new Dcollection();

            Response.data = dc.ReadCollection(Ado, DTO);
            return true;
        }


    }
}
