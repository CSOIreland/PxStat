using API;
using PxStat.Security;
using System.Collections.Generic;

namespace PxStat.Data
{
    internal class PxApiMetadata
    {
        internal List<PxApiItem> ReadSubjectsAsObjectList(IResponseOutput result)
        {
            List<PxApiItem> pList = new List<PxApiItem>();

            foreach (var item in result.data)
            {
                pList.Add(new PxApiItem() { id = item.SbjCode.ToString(), text = item.SbjValue, type = Configuration_BSO.GetStaticConfig("APP_PXAPI_LIST") });
            }

            return pList;
        }

        internal List<PxApiItem> ReadProductsAsObjectList(IResponseOutput result)
        {
            List<PxApiItem> pList = new List<PxApiItem>();


            foreach (var item in result.data)
            {
                pList.Add(new PxApiItem() { id = item.PrcCode.ToString(), text = item.PrcValue, type = Configuration_BSO.GetStaticConfig("APP_PXAPI_LIST") });
            }

            return pList;
        }

        internal List<PxApiItem> ReadCollectionAsObjectList(IResponseOutput result)
        {
            List<PxApiItem> pList = new List<PxApiItem>();
            if (result.data != null)
            {
                foreach (var item in result.data)
                {
                    pList.Add(new PxApiItem() { id = item.MtrCode, text = item.MtrTitle, type = Configuration_BSO.GetStaticConfig("APP_PXAPI_TABLE") });
                }
            }

            return pList;
        }


    }


    public class PxApiItem
    {
        public string id { get; set; }
        public string type { get; set; }
        public string text { get; set; }

    }

    public class PxApiMetadataItem : PxApiItem
    {
        public string updated { get; set; }
    }
}
