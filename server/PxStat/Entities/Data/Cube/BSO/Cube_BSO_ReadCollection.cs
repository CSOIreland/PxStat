using API;
using Newtonsoft.Json.Linq;
using PxStat.Resources;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a list of current releases
    /// </summary>
    internal class Cube_BSO_ReadCollection : BaseTemplate_Read<Cube_DTO_ReadCollection, Cube_VLD_ReadCollection>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadCollection(JSONRPC_API request) : base(request, new Cube_VLD_ReadCollection())
        {
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
            //See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadCollection", DTO);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }

            //return ExecuteReadCollection(Ado, DTO, Response);

            Cube_BSO cBso = new Cube_BSO();


            Response.data = cBso.ExecuteReadCollectionMetadata(Ado, DTO.language, DTO.datefrom);



            return true;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="theAdo"></param>
        /// <param name="theCubeDTO"></param>
        /// <param name="theResponse"></param>
        /// <returns></returns>
        static internal bool ExecuteReadCollection(ADO theAdo, Cube_DTO_ReadCollection theCubeDTO, JSONRPC_Output theResponse)
        {


            var ado = new Cube_ADO(theAdo);

            var dbData = ado.ReadCollection(theCubeDTO.language, theCubeDTO.datefrom);


            var collection = GetJsonStatCollectionObject(dbData);

            if (collection.Link.Item.Count == 0)
            {
                theResponse.data = new JRaw(Serialize.ToJson(collection));
                return true;
            }

            //Get the minimum next release date. The cache can only live until then.
            //If there's no next release date then the cache will live for the maximum configured amount.

            DateTime minDateItem = default(DateTime);

            var minimum = dbData.Where(x => x.RlsLiveDatetimeFrom > DateTime.Now).Min(x => x.RlsLiveDatetimeFrom);

            if (minimum != null)
            {
                minDateItem = minimum;
            }

            if (minDateItem < DateTime.Now)
            {
                minDateItem = default(DateTime);
            }

            theResponse.data = new JRaw(Serialize.ToJson(collection));

            MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadCollection", theCubeDTO, theResponse.data, minDateItem, Constants.C_CAS_DATA_CUBE_READ_COLLECTION);

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
                    Href = new Uri(string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["APP_URL"], Utility.GetCustomConfig("APP_COOKIELINK_TABLE"), element.MtrCode)),
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
