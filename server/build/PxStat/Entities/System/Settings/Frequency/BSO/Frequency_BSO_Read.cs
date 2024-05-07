using API;
using PxStat.Security;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Reads a Frequency
    /// </summary>
    internal class Frequency_BSO_Read : BaseTemplate_Read<Frequency_DTO, Frequency_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Frequency_BSO_Read(JSONRPC_API request) : base(request, new Frequency_VLD_Read())
        { }

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
            List<string> configList = new List<string>(Configuration_BSO.GetStaticConfig("APP_PX_FREQUENCY_CODES").Split(','));
            List<dynamic> configs = new List<dynamic>();
            foreach (var v in configList)
            {
                string[] item = v.Split('/');
                if (item.Length < 2) return false;

                dynamic freq = new ExpandoObject();
                freq.FrqCode = item[0];
                //NOTE: Translation of item[1]; at Client side.
                freq.FrqValue = item[1];
                configs.Add(freq);
            }

            Response.data = configs;

            return true;
        }

        internal string Read(string FrqCode)
        {
            return "something";
        }
    }
}
