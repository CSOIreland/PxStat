using API;
using PxStat.Resources;
using PxStat.Security;
using PxStat.Template;
using System;
using System.Collections.Generic;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a list of current releases
    /// </summary>
    internal class Cube_BSO_ReadCollection : BaseTemplate_Read<Cube_DTO_ReadCollection, Cube_VLD_ReadCollection>
    {
        bool _meta;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadCollection(JSONRPC_API request, bool meta = true) : base(request, new Cube_VLD_ReadCollection())
        {
            _meta = meta;
        }

        /// <summary>
        /// Test authentication
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

            //so that caches don't get mixed up..
            DTO.meta = _meta;

            //See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadCollection", DTO);



            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }


            Cube_BSO cBso = new Cube_BSO();

            // cache store is done in the following function
            Response.data = cBso.ExecuteReadCollection(Ado, DTO, _meta);


            return true;
        }






        /// <summary>
        /// Returns the dataset in JSON-stat format
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private static JsonStatCollection GetJsonStatCollectionObject(List<dynamic> collection)
        {
            var theJsonStatCollection = new JsonStatCollection();
            var theCollectionLink = new JsonStatCollectionLink() { Item = new List<Item>() };
            theJsonStatCollection.Link = theCollectionLink;

            if (collection == null)
            {
                return theJsonStatCollection;
            }

            if (collection.Count == 0)
            {
                return theJsonStatCollection;
            }

            foreach (var element in collection)
            {

                var aItem = new Item()
                {
                    Href = new Uri(string.Format("{0}/{1}/{2}", Configuration_BSO.GetCustomConfig(ConfigType.global, "url.application"), Utility.GetCustomConfig("APP_COOKIELINK_TABLE"), element.MtrCode)),
                    Class = ItemClass.Dataset,
                    Label = element.MtrTitle,
                    Updated = DataAdaptor.ConvertToString(element.RlsLiveDatetimeFrom),

                    Extension = new Dictionary<string, object>()
                };

                var Frequency = new { name = element.FrqValue, code = element.FrqCode };

                aItem.Extension.Add("copyright", new { name = element.CprValue, code = element.CprCode, href = element.CprUrl });
                aItem.Extension.Add("exceptional", element.ExceptionalFlag);
                aItem.Extension.Add("language", new { code = element.LngIsoCode, name = element.LngIsoName });
                aItem.Extension.Add("matrix", element.MtrCode);
                aItem.Extension.Add("frequency", Frequency);
                theCollectionLink.Item.Add(aItem);
            }



            return theJsonStatCollection;
        }
    }
}
