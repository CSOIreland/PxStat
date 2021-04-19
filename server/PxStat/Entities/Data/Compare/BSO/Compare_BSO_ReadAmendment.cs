using API;
using Newtonsoft.Json.Linq;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Compare_BSO_ReadAmendment : BaseTemplate_Read<Compare_DTO_Read, Compare_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Compare_BSO_ReadAmendment(JSONRPC_API request) : base(request, new Compare_VLD_Read())
        {
        }

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
            Compare_BSO bso = new Compare_BSO();
            Compare_DTO_Read dtoRight = new Compare_DTO_Read();


            Compare_DTO_Read dtoLeft = new Compare_DTO_Read();
            dtoLeft.RlsCode = DTO.RlsCode;

            dtoLeft.LngIsoCode = DTO.LngIsoCode;

            dtoRight.RlsCode = new Compare_ADO(Ado).ReadPreviousReleaseForUser(DTO.RlsCode, SamAccountName);
            if (dtoRight.RlsCode == 0) return false;

            dtoRight.LngIsoCode = DTO.LngIsoCode;

            var jsonStat = bso.CompareAmendment(Ado, dtoLeft, dtoRight).GetJsonStatObject(true);
            Response.data = new JRaw(Serialize.ToJson(jsonStat));

            return true;


        }

    }
}
