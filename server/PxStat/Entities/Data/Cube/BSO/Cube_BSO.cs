using API;
using Newtonsoft.Json.Linq;
using PxStat.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PxStat.Data
{
    internal class Cube_BSO
    {
        /// <summary>
        /// Get the collection with metadata
        /// </summary>
        /// <param name="theAdo"></param>
        /// <param name="theCubeDTO"></param>
        /// <param name="theResponse"></param>
        /// <returns></returns>
        internal dynamic ExecuteReadCollectionMetadata(ADO theAdo, string language, DateTime datefrom)
        {
            var ado = new Cube_ADO(theAdo);

            var dbData = ado.ReadCollectionMetadata(language, datefrom);



            List<dynamic> jsonStatCollection = new List<dynamic>();

            //Get a list of individual matrix data entities
            List<dynamic> releases = getReleases(dbData);


            var theJsonStatCollection = new JsonStatCollection();
            theJsonStatCollection.Link = new JsonStatCollectionLink();
            theJsonStatCollection.Link.Item = new List<Item>();

            //For each of these, get a list of statistics and a list of classifications
            //Then get the JSON-stat for that metadata and add to jsonStatCollection
            foreach (var rls in releases)
            {
                List<dynamic> thisReleaseMetadata = dbData.Where(x => x.RlsCode == rls.RlsCode).Where(x => x.LngIsoCode == rls.LngIsoCode).ToList<dynamic>();

                List<dynamic> stats = getStatistics(thisReleaseMetadata);
                List<dynamic> classifications = getClassifications(thisReleaseMetadata);
                List<dynamic> periods = getPeriods(thisReleaseMetadata);
                theJsonStatCollection.Link.Item.Add(GetJsonStatRelease(thisReleaseMetadata, stats, classifications, periods));

                //jsonData.Add(new JRaw(Serialize.ToJson(matrix.GetJsonStatObject())));
            }


            // return the formatted data. This is an array of JSON-stat objects.
            return new JRaw(Serialize.ToJson(theJsonStatCollection));


        }

        private List<dynamic> getPeriods(List<dynamic> releaseItems)
        {
            var periods = (from p in releaseItems
                           group p by new
                           {
                               p.PrdCode,
                               p.PrdValue
                           }
                       into prd
                           select new
                           {
                               prd.Key.PrdCode,
                               prd.Key.PrdValue
                           }
                       ).ToList<dynamic>();
            return periods;
        }

        /// <summary>
        /// Get a list of classifications based on the collections database read
        /// </summary>
        /// <param name="releaseItems"></param>
        /// <returns></returns>
        private List<dynamic> getClassifications(List<dynamic> releaseItems)
        {
            var stats = (from c in releaseItems
                         group c by new
                         {
                             c.ClsCode,
                             c.ClsValue
                         }
                       into cls
                         select new
                         {
                             cls.Key.ClsCode,
                             cls.Key.ClsValue
                         }
                       ).ToList<dynamic>();
            return stats;
        }

        /// <summary>
        /// Get a list of statistics based on the collection database read
        /// </summary>
        /// <param name="releaseItems"></param>
        /// <returns></returns>
        private List<dynamic> getStatistics(List<dynamic> releaseItems)
        {
            var stats = (from s in releaseItems
                         group s by new
                         {
                             s.SttValue,
                             s.SttCode
                         }
                       into stt
                         select new
                         {
                             stt.Key.SttValue,
                             stt.Key.SttCode
                         }
                       ).ToList<dynamic>();
            return stats;
        }

        /// <summary>
        /// Get a list of individual releases from the collection database read
        /// </summary>
        /// <param name="dbData"></param>
        /// <returns></returns>
        private List<dynamic> getReleases(List<dynamic> dbData)

        {
            if (dbData == null) return new List<dynamic>();
            var releases = (from d in dbData
                            group d by new
                            {
                                d.RlsCode,
                                d.MtrCode,
                                d.LngIsoCode,
                                d.LngIsoName,
                                d.MtrTitle,
                                d.CprValue,
                                d.CprUrl,
                                d.CprCode,
                                d.RlsLiveDatetimeFrom,
                                d.RlsLiveDatetimeTo,
                                d.EmergencyFlag,
                                d.FrqCode,
                                d.FrqValue
                            }
                            into rls
                            select new
                            {
                                rls.Key.RlsCode,
                                rls.Key.MtrCode,
                                rls.Key.LngIsoCode,
                                rls.Key.LngIsoName,
                                rls.Key.MtrTitle,
                                rls.Key.CprValue,
                                rls.Key.CprUrl,
                                rls.Key.CprCode,
                                rls.Key.RlsLiveDatetimeFrom,
                                rls.Key.RlsLiveDatetimeTo,
                                rls.Key.EmergencyFlag,
                                rls.Key.FrqCode,
                                rls.Key.FrqValue
                            }
                            ).ToList<dynamic>();
            return releases;
        }

        /// <summary>
        /// Get a JSON-stat metadata based on a database read of collections
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="statistics"></param>
        /// <param name="classifications"></param>
        /// <returns></returns>
        private Item GetJsonStatRelease(List<dynamic> collection, List<dynamic> statistics, List<dynamic> classifications, List<dynamic> periods)
        {

            var jsStat = new Item();
            // jsStat.Class = Class.Dataset;
            //jsStat.Version = Version.The20;
            jsStat.Id = new List<string>();
            jsStat.Size = new List<long>();

            var thisItem = collection.FirstOrDefault();

            jsStat.Extension = new Dictionary<string, object>();

            var Frequency = new { name = thisItem.FrqValue, code = thisItem.FrqCode };

            jsStat.Extension.Add("copyright", new { name = thisItem.CprValue, code = thisItem.CprCode, href = thisItem.CprUrl });
            jsStat.Extension.Add("emergency", thisItem.EmergencyFlag);
            jsStat.Extension.Add("language", new { code = thisItem.LngIsoCode, name = thisItem.LngIsoName });
            jsStat.Extension.Add("matrix", thisItem.MtrCode);

            jsStat.Href = new Uri(string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["APP_URL"], Utility.GetCustomConfig("APP_COOKIELINK_TABLE"), thisItem.MtrCode));
            jsStat.Label = thisItem.MtrTitle;
            jsStat.Updated = DataAdaptor.ConvertToString(thisItem.RlsLiveDatetimeFrom);
            jsStat.Dimension = new Dictionary<string, Dimension>();

            var statDimension = new Dimension()
            {

                Label = Utility.GetCustomConfig("APP_CSV_STATISTIC"),

                Category = new Category()
                {
                }

            };

            jsStat.Dimension.Add(Utility.GetCustomConfig("APP_CSV_STATISTIC"), statDimension);

            jsStat.Id.Add(Frequency.code);

            List<PeriodRecordDTO_Create> plist = new List<PeriodRecordDTO_Create>();
            foreach (var per in periods)
            {
                plist.Add(new PeriodRecordDTO_Create() { Code = per.PrdCode, Value = per.PrdValue });
            }


            var timeDimension = new Dimension()
            {

                Label = Frequency.name,

                Category = new Category()
                {
                    Index = plist.Select(v => v.Code).ToList(),
                    Label = plist.ToDictionary(v => v.Code, v => v.Value)
                }

            };

            jsStat.Dimension.Add(Frequency.code, timeDimension);

            foreach (var s in classifications)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add(s.ClsCode, s.ClsValue);
                var clsDimension = new Dimension()
                {

                    Label = s.ClsValue,
                    Category = new Category()
                    {

                    }

                };

                jsStat.Dimension.Add(s.ClsCode, clsDimension);
            }


            jsStat.Role = new Role();
            jsStat.Role.Metric = new List<string>
            {
                Utility.GetCustomConfig("APP_CSV_STATISTIC")
            };
            jsStat.Role.Time = new List<string>
            {
                thisItem.FrqCode
            };

            //Values will be blank in this case, all we want is metadata
            //jsStat.Value = new JsonStatValue() { AnythingArray = new List<ValueElement>() };
            return jsStat;
        }

    }
}
