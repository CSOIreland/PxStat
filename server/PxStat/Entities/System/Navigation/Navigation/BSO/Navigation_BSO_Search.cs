using API;
using PxStat.Security;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Searches for releases via keywords
    /// </summary>
    internal class Navigation_BSO_Search : BaseTemplate_Read<Navigation_DTO_Search, Navigation_VLD_Search>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// 

        int searchTermCount;

        internal Navigation_BSO_Search(JSONRPC_API request) : base(request, new Navigation_VLD_Search())
        { }

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

            Navigation_ADO adoNav = new Navigation_ADO(Ado);

            DTO.SearchTerms = PrepareSearchData();

            //DTO.search has too much variation to use as a cache key - the search terms are formatted in the table so we don't need it any more     
            DTO.Search = "";

            MemCachedD_Value cache = MemCacheD.Get_BSO("PxStat.System.Navigation", "Navigation_API", "Search", DTO); //
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }


            dynamic data = adoNav.Search(DTO, searchTermCount);

            Response.data = FormatOutput(data, DTO.LngIsoCode);

            MemCacheD.Store_BSO("PxStat.System.Navigation", "Navigation_API", "Search", DTO, Response.data, default(DateTime), Resources.Constants.C_CAS_NAVIGATION_SEARCH);

            return true;
        }

        private DataTable PrepareSearchData()
        {
            //Get a keyword extractor. The keyword extractor should, if possible, correspond to the supplied LngIsoCode
            //If an extractor is not found for the supplied LngIsoCode then a basic default extractor is supplied.
            Keyword_BSO_Extract kbe = new Keyword_BSO_Extract(DTO.LngIsoCode);
            //  IKeywordExtractor extractor = kbe.GetExtractor();

            //Get a list of keywords based on the supplied search term
            List<string> searchList = new List<string>();
            List<string> searchListInvariant = new List<string>();


            if (DTO.Search != null)
            {
                //Get a singularised version of each word
                searchList = kbe.ExtractSplitSingular(DTO.Search);
                //We need a count of search terms (ignoring duplicates caused by singularisation)
                searchTermCount = searchList.Count;
                //Some words will not be searched for in their singular form (flagged on the table), so we need those as well
                searchListInvariant = kbe.ExtractSplit(DTO.Search);
                //Eliminate duplicates from the list
                searchList = searchList.Union(searchListInvariant).ToList();
            }



            //The list of keywords must be supplied to the search stored procedure
            //In order to do this,  we pass a datatable as a parameter
            //First we create the table
            DataTable dt = new DataTable();
            dt.Columns.Add("Key");
            dt.Columns.Add("Value");
            dt.Columns.Add("Attribute");

            int searchSynonymMult = Configuration_BSO.GetCustomConfig(ConfigType.server, "search.synonym-multiplier");
            int searchWordMult = Configuration_BSO.GetCustomConfig(ConfigType.server, "search.search-word-multiplier");

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
                        row["Attribute"] = searchSynonymMult;
                        dt.Rows.Add(row);
                    }
                }
                else
                {
                    var row = dt.NewRow();
                    row["Key"] = word;
                    row["Value"] = word;
                    row["Attribute"] = searchSynonymMult;
                    dt.Rows.Add(row);

                }
                //The main search term must be included along with the synonyms, i.e. a word is a synonym of itself
                //But with a higher search priority, which goes into the Attribute column
                var keyrow = dt.NewRow();
                if (dt.Select("Key = '" + word + "'").Count() == 0)
                {
                    keyrow["Key"] = word;
                    keyrow["Value"] = word;
                    keyrow["Attribute"] = searchWordMult;
                    dt.Rows.Add(keyrow);
                }
            }


            //Sort the search terms to ensure invariant word order in the search       
            dt.DefaultView.Sort = "Key,Value asc";
            dt = dt.DefaultView.ToTable();

            return dt;
        }

        /// <summary>
        /// Reformats the returned data into a complex object
        /// </summary>
        /// <param name="rawList"></param>
        /// <returns></returns>
        private List<dynamic> FormatOutput(List<dynamic> rawList, string lngIsoCode)
        {
            List<dynamic> releases = GetReleases(rawList).ToList();
            var v = rawList.Where(x => x.PrdId == 17677);
            List<dynamic> classifications = GetClassifications(rawList).ToList();
            List<dynamic> periods = GetPeriods(rawList).ToList();
            List<dynamic> outList = new List<dynamic>();


            foreach (var release in releases)
            {
                dynamic rel = new ExpandoObject();
                rel.MtrCode = release.MtrCode;
                rel.MtrTitle = release.MtrTitle;
                rel.MtrOfficialFlag = release.MtrOfficialFlag;
                rel.SbjCode = release.SbjCode;
                rel.SbjValue = release.SbjValue;
                rel.PrcCode = release.PrcCode;
                rel.PrcValue = release.PrcValue;
                rel.RlsLiveDatetimeFrom = release.RlsLiveDatetimeFrom;
                rel.RlsExceptionalFlag = release.RlsExceptionalFlag;
                rel.RlsReservationFlag = release.RlsReservationFlag;
                rel.RlsArchiveFlag = release.RlsArchiveFlag;
                rel.RlsExperimentalFlag = release.RlsExperimentalFlag;
                rel.RlsAnalyticalFlag = release.RlsAnalyticalFlag;
                rel.CprCode = release.CprCode;
                rel.CprValue = release.CprValue;
                rel.LngIsoCode = release.LngIsoCode;
                rel.LngIsoName = release.LngIsoName;
                rel.classification = new List<dynamic>();
                List<dynamic> classList = new List<dynamic>();
                var rlsCls = classifications.Where(x => x.RlsCode == release.RlsCode);
                foreach (var cls in rlsCls)
                {
                    dynamic RlsCls = new ExpandoObject();
                    string readLanguage = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
                    //If this classification exists in the requested language, return it, otherwise return it in the default language
                    if ((classifications.Where(x => (x.RlsCode == release.RlsCode && x.LngIsoCode == DTO.LngIsoCode)).Count() > 0))
                    {
                        readLanguage = DTO.LngIsoCode;
                    }

                    if (cls.RlsCode == release.RlsCode && (cls.LngIsoCode == readLanguage))
                    {
                        RlsCls.ClsCode = cls.ClsCode;
                        RlsCls.ClsValue = cls.ClsValue;
                        RlsCls.ClsGeoFlag = cls.ClsGeoFlag;
                        RlsCls.ClsGeoUrl = cls.ClsGeoUrl;
                        classList.Add(RlsCls);
                    }

                }

                rel.classification = classList;
                rel.FrqCode = release.FrqCode;
                rel.FrqValue = release.FrqValue;

                List<dynamic> prdList = new List<dynamic>();

                var rlsPrd = periods.Where(x => x.RlsCode == release.RlsCode);
                foreach (var prd in rlsPrd)
                {
                    prdList.Add(prd.PrdValue);

                }
                rel.period = prdList.ToArray();
                rel.Score = release.Score;

                //Does a version of this matrix exist in our preferred language?
                bool existsInPreferredLangauge = releases.Where(x => x.LngIsoCode == lngIsoCode && x.MtrCode == release.MtrCode).ToList<dynamic>().Count > 0;

                // We need to conditionally display our results depending on the preferred language
                //Either this matrix exists in our preferred language..
                //..Or else it doesn't exist in our preferred language but a version exists in the default language
                if (release.LngIsoCode == lngIsoCode || (release.LngIsoCode == Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code") && !existsInPreferredLangauge))
                {
                    outList.Add(rel);
                }
            }

            if (DTO.PrcCode == null)
                return outList.OrderByDescending(x => x.Score).ThenByDescending(x => x.RlsLiveDatetimeFrom).ThenBy(x => x.MtrCode).ToList();
            else
                return outList.OrderBy(x => x.MtrCode).ToList();

        }

        /// <summary>
        /// Get the list of RlsCodes with Periods from the main body of returned data
        /// </summary>
        /// <param name="rawList"></param>
        /// <returns></returns>
        private IEnumerable<dynamic> GetPeriods(List<dynamic> rawList)
        {
            var periods = (from t in rawList
                           orderby t.MtrCode, t.PrdId
                           group t by new
                           {
                               t.RlsCode,
                               t.PrdValue
                           }
                            into grp
                           select new
                           {
                               grp.Key.RlsCode,
                               grp.Key.PrdValue

                           }).ToList();

            return periods;
        }

        /// <summary>
        /// Get the list of RlsCodes with Classifications from the main body of returned data
        /// </summary>
        /// <param name="rawList"></param>
        /// <returns></returns>
        private IEnumerable<dynamic> GetClassifications(List<dynamic> rawList)
        {
            var classifications = (from t in rawList
                                   orderby t.ClsId
                                   group t by new
                                   {
                                       t.RlsCode,
                                       t.ClsCode,
                                       t.ClsValue,
                                       t.ClsGeoFlag,
                                       t.ClsGeoUrl,
                                       t.LngIsoCode
                                   }
                            into grp
                                   select new
                                   {
                                       grp.Key.RlsCode,
                                       grp.Key.ClsCode,
                                       grp.Key.ClsValue,
                                       grp.Key.ClsGeoFlag,
                                       grp.Key.ClsGeoUrl,
                                       grp.Key.LngIsoCode


                                   }).ToList();

            return classifications;

        }

        /// <summary>
        /// Get the list of unique Releases along with associated fields
        /// </summary>
        /// <param name="rawList"></param>
        /// <returns></returns>
        private IEnumerable<dynamic> GetReleases(List<dynamic> rawList)
        {
            var Releases = (from t in rawList
                            group t by new
                            {
                                t.RlsCode,
                                t.MtrCode,
                                t.MtrTitle,
                                t.MtrOfficialFlag,
                                t.SbjCode,
                                t.SbjValue,
                                t.PrcCode,
                                t.PrcValue,
                                t.RlsLiveDatetimeFrom,
                                t.RlsExceptionalFlag,
                                t.RlsReservationFlag,
                                t.RlsArchiveFlag,
                                t.RlsExperimentalFlag,
                                t.RlsAnalyticalFlag,
                                t.CprCode,
                                t.CprValue,
                                t.FrqCode,
                                t.FrqValue,
                                t.LngIsoCode,
                                t.LngIsoName,
                                //t.IsGeo,
                                t.Score
                            }
             into grp
                            select new
                            {
                                grp.Key.RlsCode,
                                grp.Key.MtrCode,
                                grp.Key.MtrTitle,
                                grp.Key.MtrOfficialFlag,
                                grp.Key.SbjCode,
                                grp.Key.SbjValue,
                                grp.Key.PrcCode,
                                grp.Key.PrcValue,
                                grp.Key.RlsLiveDatetimeFrom,
                                grp.Key.RlsExceptionalFlag,
                                grp.Key.RlsReservationFlag,
                                grp.Key.RlsArchiveFlag,
                                grp.Key.RlsExperimentalFlag,
                                grp.Key.RlsAnalyticalFlag,
                                grp.Key.CprCode,
                                grp.Key.CprValue,
                                grp.Key.FrqCode,
                                grp.Key.FrqValue,
                                grp.Key.LngIsoCode,
                                grp.Key.LngIsoName,
                                grp.Key.Score


                            }).ToList();
            return Releases;
        }
    }
}
