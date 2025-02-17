using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a list of current releases
    /// </summary>
    public class Cube_BSO_ReadCollection : BaseTemplate_Read<Cube_DTO_ReadCollection, Cube_VLD_ReadCollection>
    {
       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Cube_BSO_ReadCollection(IRequest request) : base(request, new Cube_VLD_ReadCollection())
        {
            
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


            //See if this request has cached data

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO_WITHLOCK<dynamic>("PxStat.Data", "Cube_API", "ReadCollection", DTO);



            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }


            Cube_BSO cBso = new Cube_BSO();

            // cache store is done in the following function


            Dcollection dc = new Dcollection();

            Response.data = dc.ReadCollection(Ado, DTO);
            return true;
        }


    }
}
