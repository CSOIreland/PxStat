using API;
using System.Collections.Generic;
using System.Data;

namespace PxStat.System.Navigation
{
    internal class Keyword_Product_BSO_CreateMandatory
    {
        internal void Create(ADO Ado, Product_DTO productDto, int productID)
        {
            //There is no direct means of finding out which langauge the product name uses,
            // so we take a default language from the settings
            string languageCode = Utility.GetCustomConfig("APP_KEYWORD_DEFAULT_LANGUAGE_ISO_CODE");

            //Create the table that will be bulk inserted
            DataTable dt = new DataTable();
            dt.Columns.Add("KPR_VALUE", typeof(string));
            dt.Columns.Add("KPR_PRC_ID", typeof(int));
            dt.Columns.Add("KPR_MANDATORY_FLAG", typeof(bool));

            Keyword_Product_ADO keywordProductAdo = new Keyword_Product_ADO(Ado);


            //Get a Keyword Extractor - the particular version returned will depend on the language
            IKeywordExtractor ext = Keyword_BSO_Extract.GetExtractor(languageCode);
            AddToTable(ref dt, ext.ExtractSplitSingular(productDto.PrcValue), productID);

            keywordProductAdo.Create(dt);


        }

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