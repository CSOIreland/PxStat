using API;
using DocumentFormat.OpenXml.Wordprocessing;
using PxStat.Resources;
using PxStat.Template;
using System;
using System.Dynamic;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Returns a boolean to see to see if a language has a language plugin
    /// </summary>
    internal class Language_BSO_CheckIsLngIsoCode : BaseTemplate_Read<Language_DTO_Check, Language_VLD_Check>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Language_BSO_CheckIsLngIsoCode(JSONRPC_API request) : base(request, new Language_VLD_Check())
        {
        }

        /// <summary>
        /// Test Authentication
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
            // Check that Language exists
            if (LanguageManager.Languages == null)
            {
                Response.data = false;
                return false;
            }
            Response.data = LanguageManager.Languages.ContainsKey(DTO.LngIsoCode);
            return true;
        }
    }
}