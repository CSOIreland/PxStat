using API;
using PxStat.Template;
using System.Collections.Generic;
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



            dynamic data = adoNav.Search(DTO);

            Response.data = FormatOutput(data, DTO.LngIsoCode);

            return true;
        }

        /// <summary>
        /// Reformats the returned data into a complex object
        /// </summary>
        /// <param name="rawList"></param>
        /// <returns></returns>
        private List<dynamic> FormatOutput(List<dynamic> rawList, string lngIsoCode)
        {
            List<dynamic> releases = GetReleases(rawList).ToList();
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
                rel.RlsEmergencyFlag = release.RlsEmergencyFlag;
                rel.RlsReservationFlag = release.RlsReservationFlag;
                rel.RlsArchiveFlag = release.RlsArchiveFlag;
                rel.RlsAnalyticalFlag = release.RlsAnalyticalFlag;
                rel.RlsDependencyFlag = release.RlsDependencyFlag;
                rel.CprCode = release.CprCode;
                rel.CprValue = release.CprValue;
                rel.LngIsoCode = release.LngIsoCode;
                rel.LngIsoName = release.LngIsoName;
                rel.classification = new List<dynamic>();
                List<dynamic> classList = new List<dynamic>();
                foreach (var cls in classifications)
                {
                    dynamic RlsCls = new ExpandoObject();
                    string readLanguage = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
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

                foreach (var prd in periods)
                {
                    if (prd.RlsCode == release.RlsCode)
                    {
                        prdList.Add(prd.PrdValue);
                    }
                }
                rel.period = prdList.ToArray();
                rel.Score = release.Score;

                //Does a version of this matrix exist in our preferred language?
                bool existsInPreferredLangauge = releases.Where(x => x.LngIsoCode == lngIsoCode && x.MtrCode == release.MtrCode).ToList<dynamic>().Count > 0;

                // We need to conditionally display our results depending on the preferred language
                //Either this matrix exists in our preferred language..
                //..Or else it doesn't exist in our preferred language but a version exists in the default language
                if (release.LngIsoCode == lngIsoCode || (release.LngIsoCode == Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE") && !existsInPreferredLangauge))
                {
                    outList.Add(rel);
                }
            }

            return outList;
        }

        /// <summary>
        /// Get the list of RlsCodes with Periods from the main body of returned data
        /// </summary>
        /// <param name="rawList"></param>
        /// <returns></returns>
        private IEnumerable<dynamic> GetPeriods(List<dynamic> rawList)
        {
            var periods = (from t in rawList
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
                                t.RlsEmergencyFlag,
                                t.RlsReservationFlag,
                                t.RlsArchiveFlag,
                                t.RlsAnalyticalFlag,
                                t.RlsDependencyFlag,
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
                                grp.Key.RlsEmergencyFlag,
                                grp.Key.RlsReservationFlag,
                                grp.Key.RlsArchiveFlag,
                                grp.Key.RlsAnalyticalFlag,
                                grp.Key.RlsDependencyFlag,
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
