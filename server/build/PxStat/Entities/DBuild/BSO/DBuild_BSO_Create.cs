using API;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Template;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_Create : BaseTemplate_Read<DBuild_DTO_Create, DBuild_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_Create(JSONRPC_API request) : base(request, new DBuild_VLD_Create())
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

            var timeDimension = dmatrix.Dspecs[dmatrix.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).First();

            if (timeDimension != null)
            {
                if (!bid.AreVariablesSequential(timeDimension))
                {
                    timeDimension.Variables = timeDimension.Variables.OrderBy(x => x.Code).ToList();

                    Dictionary<int, int> sequenceDictionary = new();
                    int counter = 1;
                    foreach (var vrb in timeDimension.Variables)
                    {
                        sequenceDictionary.Add(vrb.Sequence, counter);
                        counter++;
                    }
                    dmatrix = bid.SortVariablesInDimension(dmatrix, sequenceDictionary, timeDimension.Sequence);
                }
            }
            else
            {
                Response.error = Label.Get("error.invalid", DTO.LngIsoCode);
                return false;
            }


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

           
            DMatrix_VLD dmv = new DMatrix_VLD( Ado, DTO.LngIsoCode);
            // Also validate in english - just for the logs
            DMatrix_VLD dmvEn = new DMatrix_VLD( Ado);
            dmvEn.Validate(dmatrix);

            var vresult = dmv.Validate(dmatrix);
            if (vresult.IsValid)
            {

                List<dynamic> resultPx = new List<dynamic>();

                PxFileBuilder pxb = new PxFileBuilder();
                string px = pxb.Create(dmatrix,  DTO.LngIsoCode);

                List<string> file = new List<string> { px };
                resultPx.Add(px);

                Response.data = resultPx;
                return true;
            }
            else
            {
                Response.error = Label.Get("error.invalid", DTO.LngIsoCode);
                return false;
            }
        }

    }
}
