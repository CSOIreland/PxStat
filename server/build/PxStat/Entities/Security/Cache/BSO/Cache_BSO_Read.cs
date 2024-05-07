using API;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.Configuration;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            int port = AppServicesHelper.AppSettings.GetValue<int>("enyimMemcached:Servers:0:Port");
            string address = AppServicesHelper.AppSettings.GetValue<string>("enyimMemcached:Servers:0:Address");

            ServerStats stats = AppServicesHelper.CacheD.GetStats();

            if (stats == null) return false;

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!(port == 0 || address == null))//if port or address are 0 or null return false otherwise get the stats of the first server
            {
                for (int i = 0; i < Enum.GetNames(typeof(StatItem)).Length; i++)
                {
                    // takes first item from each of address and port
                    var value = stats.GetRaw(new IPEndPoint(IPAddress.Parse(address), port), (StatItem)i); 
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
