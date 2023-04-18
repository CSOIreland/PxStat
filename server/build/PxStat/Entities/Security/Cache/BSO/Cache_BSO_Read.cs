using API;
using Enyim.Caching.Memcached;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace PxStat.Security
{
    internal class Cache_BSO_Read : BaseTemplate_Read<Cache_DTO_Read, Cache_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cache_BSO_Read(JSONRPC_API request) : base(request, new Cache_VLD_Read())
        {
        }

        /// <summary>
        /// Test privileges
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            string webconfig = AppDomain.CurrentDomain.BaseDirectory + "Web.config";
            // To get the memcached server data from web.config because not part of appsettings
            XDocument config = XDocument.Load(webconfig);

            var port = config.Descendants("enyim.com")?.Descendants("servers")?.Descendants("add")?.Attributes("port");

            var address = config.Descendants("enyim.com")?.Descendants("servers")?.Descendants("add")?.Attributes("address");

            ServerStats stats = MemCacheD.GetStats();

            if (stats == null) return false;

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!(port == null || address == null))//if port or address are null return false otherwise get the stats of the first server
            {
                for (int i = 0; i < Enum.GetNames(typeof(StatItem)).Length; i++)
                {
                    // takes first item from each of address and port
                    var value = stats.GetRaw(new IPEndPoint(IPAddress.Parse(address.First().Value.ToString()), Int32.Parse(port.First().Value)), (StatItem)i);
                    string key = ((StatItem)i).ToString();
                    result.Add(key, value);
                }
                Response.data = result;
                return true;
            }
            else
                return false;
        }
    }

}
