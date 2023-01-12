using API;
using PxStat.Data;
using PxStat.Template;
using System.Collections.Generic;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_Update : BaseTemplate_Read<DBuild_DTO_Update, DBuild_VLD_Update>
    {
        /// <summary>
        /// 
        /// </summary>
        List<int> divisors = new List<int>();



        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_Update(JSONRPC_API request) : base(request, new DBuild_VLD_Update())
        { }

        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            //Check permissions
            DBuild_BSO dbso = new DBuild_BSO();
            if (!dbso.HasBuildPermission(SamAccountName, "update"))
            {
                Response.error = Label.Get("error.privilege");
                return false;
            }


            if (!DTO.Signature.Equals(Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()))))
            {
                Response.error = Label.Get("error.validation");
                return false;
            }



            Response = dbso.BsoUpdate(Response, DTO, new MetaData());
            return Response.error != null;
        }



    }
}
