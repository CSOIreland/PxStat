using API;
using PxStat.Security;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// BSO for Keyword Products (mandatory)
    /// </summary>
    internal class Keyword_Product_BSO_Mandatory
    {
        /// <summary>
        /// Creates a Keyword Product
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="productDto"></param>
        /// <param name="productID"></param>
        internal void Create(IADO Ado, Product_DTO productDto, int productID)
        {
            //If there is no direct means of finding out which langauge the product name uses,
            // we take a default language from the settings
            string LngIsoCode;

            if (productDto.LngIsoCode == null)
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            else
                LngIsoCode = productDto.LngIsoCode;

            //Create the table that will be bulk inserted
            DataTable dt = new DataTable();
            dt.Columns.Add("KPR_VALUE", typeof(string));
            dt.Columns.Add("KPR_PRC_ID", typeof(int));
            dt.Columns.Add("KPR_MANDATORY_FLAG", typeof(bool));

            Keyword_Product_ADO keywordProductAdo = new Keyword_Product_ADO(Ado);


            //Get a Keyword Extractor - the particular version returned will depend on the language
            Keyword_BSO_Extract kbe = new Navigation.Keyword_BSO_Extract(LngIsoCode);

            AddToTable(ref dt, kbe.ExtractSplitSingular(productDto.PrcValue), productID);


            keywordProductAdo.Create(dt);
        }

        internal void Create(IADO Ado, Product_DTO productDto, int productID,List<dynamic> versions)
        {
            //If there is no direct means of finding out which langauge the product name uses,
            // we take a default language from the settings
            string LngIsoCode;

            if (productDto.LngIsoCode == null)
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            else
                LngIsoCode = productDto.LngIsoCode;

            //Create the table that will be bulk inserted
            DataTable dt = new DataTable();
            dt.Columns.Add("KPR_VALUE", typeof(string));
            dt.Columns.Add("KPR_PRC_ID", typeof(int));
            dt.Columns.Add("KPR_MANDATORY_FLAG", typeof(bool));

            Keyword_Product_ADO keywordProductAdo = new Keyword_Product_ADO(Ado);

            Dictionary<string,Keyword_BSO_Extract> kbeList = new();


            List<string> allKeywords = new();
            foreach(var item in versions)
            {
                if (!kbeList.ContainsKey(item.LngIsoCode))
                    kbeList.Add(item.LngIsoCode, new Keyword_BSO_Extract(item.LngIsoCode));
                Keyword_BSO_Extract k = kbeList[item.LngIsoCode];
                allKeywords.AddRange(k.ExtractSplitSingular(item.PrcValue));
            }
            List<string> shortList=allKeywords.Select(x=>x).Distinct().ToList();
             
            AddToTable(ref dt, shortList, productID);


            keywordProductAdo.Create(dt);
        }



        /// <summary>
        /// Deletes a Keyword Product
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="dto"></param>
        /// <param name="mandatoryOnly"></param>
        /// <returns></returns>
        internal int Delete(IADO Ado, Product_DTO dto, bool? mandatoryOnly = null)
        {
            Keyword_Product_ADO kpAdo = new Keyword_Product_ADO(Ado);
            Keyword_Product_DTO kpDto = new Keyword_Product_DTO();
            kpDto.PrcCode = dto.PrcCode;
            if (mandatoryOnly != null)
                return kpAdo.Delete(kpDto, mandatoryOnly);
            else
                return kpAdo.Delete(kpDto);

        }

        /// <summary>
        /// Adds a list of keywords to a table for upload
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="words"></param>
        /// <param name="rlsID"></param>
        private void AddToTable(ref DataTable dt, List<string> words, int rlsID)
        {
            foreach (string word in words)
            {

                //First check if the keyword is already in the table. It doesn't need to go in twice!
                string searchTerm = "KPR_VALUE = '" + word + "'";
                DataRow[] dr = dt.Select(searchTerm);
                if (dr.Length == 0)
                {
                    var row = dt.NewRow();

                    row["KPR_VALUE"] = word;
                    row["KPR_PRC_ID"] = rlsID;
                    row["KPR_MANDATORY_FLAG"] = true;

                    dt.Rows.Add(row);
                }

            }
        }
    }
}
