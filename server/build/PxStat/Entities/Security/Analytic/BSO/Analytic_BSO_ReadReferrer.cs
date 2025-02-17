using API;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Security
{
    /// <summary>
    /// Read referrer data from Analytics
    /// </summary>
    internal class Analytic_BSO_ReadReferrer : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadReferrer>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_ReadReferrer(JSONRPC_API request) : base(request, new Analytic_VLD_ReadReferrer())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            if (IsPowerUser() || IsModerator())
                return true;


            return false;
        }


        protected override bool Execute()
        {
            if (IsModerator() && String.IsNullOrEmpty(DTO.MtrCode))
            {
                Response.error = Label.Get("error.privilege");
                return false;
            }

            MemCachedD_Value cache = ApiServicesHelper.CacheD.Get_BSO("PxStat.Security", "Analytic", "ReadReferrer", DTO);
            
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }
            Analytic_ADO ado = new Analytic_ADO(Ado);
            List<dynamic> outputSummary = ado.ReadReferer(DTO);
            if (outputSummary != null)
            {
                Response.data = FormatData(outputSummary);
                ApiServicesHelper.CacheD.Store_BSO("PxStat.Security", "Analytic", "ReadReferrer", DTO, Response.data, default(DateTime));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Format data for output
        /// </summary>
        /// <param name="readData"></param>
        /// <returns></returns>
        private dynamic FormatData(dynamic readData)
        {
            dynamic output = new ExpandoObject();
            var itemDict = output as IDictionary<string, object>;
            int limit = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "analytic.read-referrer-item-limit");
            int counter = 1;
            int otherSum = 0;

            string unknown = Label.Get("analytic.unknown");
            string others = Label.Get("analytic.others");

            List<nltReferer> rList = new();
            foreach (dynamic item in readData)
            {
                rList.Add(new nltReferer() { NltReferer = GetDomain(item.NltReferer), nltCount = item.NltCount });  
            }


            var oList=rList.GroupBy(x => x.NltReferer).Select(x => new { NltReferer = x.Key, NltCount = x.Sum(s => s.nltCount) });
            foreach (dynamic item in oList)
            {

                if (counter < limit)
                {
                    var itemObject = item.NltReferer.ToString() == "-" ? unknown : item.NltReferer.ToString();
                    
                    itemDict.Add(GetDomain(itemObject), item.NltCount);
                }
                else otherSum = otherSum + item.NltCount;
                counter++;
            }

            if (otherSum > 0) itemDict.Add(others, otherSum);
            
            return itemDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
       
        }

        private string GetDomain(string domain)
        {
            try
            {
                Uri uri = new Uri(domain);
                return uri.Host;
            }
            catch (FormatException fx)

            {
                return domain;
            }
            catch (Exception ex) { throw; }
        }
    }
    class nltReferer
    {
        internal string NltReferer { get; set; }      
        internal int GrpId { get; set; }
        internal int nltCount { get; set; }
    }
}
