using API;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// ADO for Navigation methods
    /// </summary>
    internal class Navigation_ADO
    {
        /// <summary>
        /// Class variable ADO
        /// </summary>
        ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Ado"></param>
        internal Navigation_ADO(ADO Ado)
        {
            ado = Ado;
        }

        /// <summary>
        /// Read method
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(Navigation_DTO_Read dto)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name="@LngIsoCode",value=dto.LngIsoCode  },
                new ADO_inputParams() {name="@LngIsoCodeDefault",value=Configuration_BSO.GetCustomConfig("language.iso.code") }
            };

            return ado.ExecuteReaderProcedure("System_Navigation_Navigation_Read", inputParams);
        }

        /// <summary>
        /// Reads the next live release date after the supplied date
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadNextLiveDate(DateTime dateFrom)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name="@ReleaseDate",value=dateFrom }
            };

            return ado.ExecuteReaderProcedure("Data_Release_ReadNext", inputParams);
        }

        /// <summary>
        /// Search method
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal dynamic Search(Navigation_DTO_Search dto)
        {
            //Get a keyword extractor. The keyword extractor should, if possible, correspond to the supplied LngIsoCode
            //If an extractor is not found for the supplied LngIsoCode then a basic default extractor is supplied.
            Keyword_BSO_Extract kbe = new Keyword_BSO_Extract(dto.LngIsoCode);
            //  IKeywordExtractor extractor = kbe.GetExtractor();

            //Get a list of keywords based on the supplied search term
            List<string> searchList = new List<string>();
            List<string> searchListInvariant = new List<string>();
            int searchTermCount = 0;

            if (dto.Search != null)
            {
                //Get a singularised version of each word
                searchList = kbe.ExtractSplitSingular(dto.Search);
                //We need a count of search terms (ignoring duplicates caused by singularisation)
                searchTermCount = searchList.Count;
                //Some words will not be searched for in their singular form (flagged on the table), so we need those as well
                searchListInvariant = kbe.ExtractSplit(dto.Search);
                //Eliminate duplicates from the list
                searchList = searchList.Union(searchListInvariant).ToList();
            }



            //The list of keywords must be supplied to the search stored procedure
            //In order to do this,  we pass a datatable as a parameter
            //First we create the table
            DataTable dt = new DataTable();
            dt.Columns.Add("Key");
            dt.Columns.Add("Value");



            //Gather any synonyms and include them in the search, in the synonyms column
            foreach (string word in searchList)
            {

                var hitlist = kbe.extractor.SynonymList.Where(x => x.match == word.ToLower());
                if ((kbe.extractor.SynonymList.Where(x => x.match == word.ToLower())).Count() > 0)
                {

                    foreach (var syn in hitlist)
                    {
                        var row = dt.NewRow();
                        row["Key"] = syn.lemma;
                        row["Value"] = syn.match;
                        dt.Rows.Add(row);
                    }
                }
                else
                {
                    var row = dt.NewRow();
                    row["Key"] = word;
                    row["Value"] = word;
                    dt.Rows.Add(row);

                }

            }


            //The ExecuteReaderProcedure method requires that the parameters be contained in a List<ADO_inputParams>
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode }

            };

            ADO_inputParams param = new ADO_inputParams() { name = "@Search", value = dt };
            param.typeName = "KeyValueVarchar";
            paramList.Add(param);
            //We need a count of search terms (ignoring duplicates caused by singularisation)
            paramList.Add(new ADO_inputParams() { name = "@SearchTermCount", value = searchTermCount });

            if (dto.MtrCode != null)
                paramList.Add(new ADO_inputParams() { name = "@MtrCode", value = dto.MtrCode });
            if (dto.MtrOfficialFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@MtrOfficialFlag", value = dto.MtrOfficialFlag });
            if (dto.SbjCode != default(int))
                paramList.Add(new ADO_inputParams() { name = "@SbjCode", value = dto.SbjCode });
            if (dto.PrcCode != null)
                paramList.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });
            if (dto.CprCode != null)
                paramList.Add(new ADO_inputParams() { name = "@CprCode", value = dto.CprCode });
            if (dto.RlsExceptionalFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsExceptionalFlag", value = dto.RlsExceptionalFlag });
            if (dto.RlsReservationFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsReservationFlag", value = dto.RlsReservationFlag });
            if (dto.RlsArchiveFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsArchiveFlag", value = dto.RlsArchiveFlag });
            if (dto.RlsAnalyticalFlag != null)
                paramList.Add(new ADO_inputParams() { name = "@RlsAnalyticalFlag", value = dto.RlsAnalyticalFlag });



            //We store from the ADO because the search terms may have been changed by keyword rules
            MemCachedD_Value cache = MemCacheD.Get_ADO("PxStat.System.Navigation", "System_Navigation_Search", paramList); //
            if (cache.hasData)
            {
                return cache.data.ToObject<List<dynamic>>();

            }


            //Call the stored procedure
            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Navigation_Search", paramList);

            MemCacheD.Store_ADO<dynamic>("PxStat.System.Navigation", "System_Navigation_Search", paramList, output.data, new DateTime(), Resources.Constants.C_CAS_NAVIGATION_SEARCH);
            //return the list of entities that have been found
            return output.data;
        }


    }
}
