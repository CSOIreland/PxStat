using API;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using PxStat.Template;

namespace PxStat.Data
{
    public class Cube_BSO_ReadLiveAll : BaseTemplate_Read<Cube_DTO_ReadLiveAll, Cube_VLD_ReadLiveAll>
    {
        public Cube_BSO_ReadLiveAll(IRequest request) : base(request, new Cube_VLD_ReadLiveAll())
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
            Cube_ADO cAdo = new Cube_ADO(Ado);
            var response = cAdo.ReadLiveAll();
            if (response != null)
            {
                 if (response.hasData)
                {
                    if(Request.GetType().Equals(typeof(RESTful_API)))
                        Response.data =Utility.JsonSerialize_IgnoreLoopingReference( response.data);
                    else
                        Response.data = response.data;
                    return true;
                }
            }
            return false;
        }
    }
}
