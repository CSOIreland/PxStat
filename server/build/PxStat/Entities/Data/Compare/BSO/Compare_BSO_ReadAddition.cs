using API;
using Newtonsoft.Json.Linq;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Compare_BSO_ReadAddition : BaseTemplate_Read<Compare_DTO_Read, Compare_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Compare_BSO_ReadAddition(JSONRPC_API request) : base(request, new Compare_VLD_Read())
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

            Compare_DTO_Read dtoRight = new Compare_DTO_Read();

            DCompare_BSO dbso = new DCompare_BSO();

            Compare_DTO_Read dtoLeft = new Compare_DTO_Read();
            dtoLeft.RlsCode = DTO.RlsCode;
            dtoLeft.LngIsoCode = DTO.LngIsoCode;



            dtoRight.RlsCode = new Compare_ADO(Ado).ReadPreviousReleaseForUser(DTO.RlsCode, SamAccountName);
            if (dtoRight.RlsCode == 0) return false;
            dtoRight.LngIsoCode = DTO.LngIsoCode;
 
              
            IDmatrix comparisonMatrix = dbso.CompareAddDelete(Ado,  dtoRight, dtoLeft);


            JsonStatBuilder2_0 jxb = new JsonStatBuilder2_0();
            var jsonStat = jxb.Create(comparisonMatrix, comparisonMatrix.Language, true,true);

            Response.data = new JRaw(Serialize.ToJson(jsonStat));

            return true;


        }


    }
}
