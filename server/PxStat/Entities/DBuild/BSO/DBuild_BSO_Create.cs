using API;
using PxStat.Data;
using PxStat.Template;
using System.Collections.Generic;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_Create : BaseTemplate_Read<DBuild_DTO_Create, DBuild_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_Create(JSONRPC_API request) : base(request, new DBuild_VLD_Create(new MetaData()))
        { }

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
            DBuild_BSO bid = new DBuild_BSO();
            IDmatrix dmatrix = bid.Create(DTO);


            if (DTO.Map != null)
            {
                foreach (var spec in dmatrix.Dspecs)
                {
                    //Ensure the map url is well formed and the map exists
                    spec.Value.ValidateMaps(true);
                    if (spec.Value.ValidationErrors == null) continue;
                    if (spec.Value.ValidationErrors.Count > 0)
                    {
                        Response.error = spec.Value.ValidationErrors;
                        return false;
                    }
                }
            }

            DMatrix_VLD dmv = new DMatrix_VLD();
            var vresult = dmv.Validate(dmatrix);
            if (vresult.IsValid)
            {

                List<dynamic> resultPx = new List<dynamic>();

                PxFileBuilder pxb = new PxFileBuilder();
                string px = pxb.Create(dmatrix, new MetaData(), DTO.LngIsoCode);

                List<string> file = new List<string> { px };
                resultPx.Add(px);

                Response.data = resultPx;
                return true;
            }
            else
            {
                Response.error = Label.Get("error.invalid");
                return false;
            }
        }

    }
}
