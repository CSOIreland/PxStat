using API;
using Newtonsoft.Json.Linq;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Compare the release with the previous release and get the deleted datapoints
    /// </summary>
    internal class Compare_BSO_ReadDeletion : BaseTemplate_Read<Compare_DTO_Read, Compare_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Compare_BSO_ReadDeletion(JSONRPC_API request) : base(request, new Compare_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
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
            Compare_BSO bso = new Compare_BSO();
            Compare_DTO_Read dtoRight = new Compare_DTO_Read();


            Compare_DTO_Read dtoLeft = new Compare_DTO_Read();

            dtoRight.RlsCode = DTO.RlsCode;
            dtoRight.LngIsoCode = DTO.LngIsoCode;


            dtoLeft.RlsCode = new Compare_ADO(Ado).ReadPreviousReleaseForUser(DTO.RlsCode, SamAccountName);
            if (dtoLeft.RlsCode == 0) return false;

            dtoLeft.LngIsoCode = DTO.LngIsoCode;

            var jsonStat = bso.CompareAddDelete(Ado, dtoRight, dtoLeft).GetJsonStatObject(true);
            Response.data = new JRaw(Serialize.ToJson(jsonStat));

            return true;



        }

    }
}
