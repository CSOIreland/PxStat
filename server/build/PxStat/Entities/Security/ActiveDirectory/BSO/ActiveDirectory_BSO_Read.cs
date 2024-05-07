using System.Collections.Generic;
using API;
using PxStat.Template;
using Newtonsoft.Json;

namespace PxStat.Security
{
    /// <summary>
    /// Reads Active Directory data
    /// </summary>
    internal class ActiveDirectory_BSO_Read : BaseTemplate_Read<ActiveDirectory_DTO, ActiveDirectory_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal ActiveDirectory_BSO_Read(JSONRPC_API request) : base(request, new ActiveDirectory_VLD())
        {
        }

        /// <summary>
        /// Test Privilege
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
            List<ActiveDirectory_DTO> result = ActiveDirectory_ADO.Read(Ado, DTO);

            if (result.Count == 0)
            {
                Log.Instance.Debug("Active Directory user not found");
                return false;
            }

            Response.data = result;


            return true;
        }

        /// <summary>
        /// Checks if a user exists in active directory - based on the the ccnUsername parameter
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal static bool IsInActiveDirectory(IADO ado, string username)
        {
            ActiveDirectory_ADO adADO = new ActiveDirectory_ADO();
            ActiveDirectory_DTO dto = new ActiveDirectory_DTO();
            dto.CcnUsername = username;
            var result = adADO.GetUser(ado, dto);
            return result.CcnUsername != null;
        }
    }
}