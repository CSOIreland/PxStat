using API;
using Newtonsoft.Json.Linq;
using PxStat.JsonStatSchema;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Navigation;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{
    internal class Cube_BSO : IDisposable
    {
        ADO ado;

        internal Cube_BSO(ADO Ado)
        {
            ado = Ado;
        }

        public Cube_BSO()
        {
        }

        internal Role UpdateRoleFromMetadata(ADO theAdo, CubeQuery_DTO dto)
        {
            Cube_ADO cAdo = new Cube_ADO(theAdo);
            dto.Role = new Role();
            Dictionary<string, string> roleDictionary;
            if (dto.jStatQueryExtension.extension.RlsCode == 0)
                roleDictionary = cAdo.ReadDimensionRoles(Utility.GetCustomConfig("APP_CSV_STATISTIC"), dto.jStatQueryExtension.extension.Matrix);
            else
                roleDictionary = cAdo.ReadDimensionRoles(Utility.GetCustomConfig("APP_CSV_STATISTIC"), null, dto.jStatQueryExtension.extension.RlsCode);

            if (roleDictionary.ContainsKey("time"))
            {
                dto.Role.Time = new List<string>();
                dto.Role.Time.Add(roleDictionary["time"]);
            }
            if (roleDictionary.ContainsKey("metric"))
            {
                dto.Role.Metric = new List<string>();
                dto.Role.Metric.Add(roleDictionary["metric"]);
            }

            return dto.Role;
        }
        /// <summary>
        /// Get the collection with metadata
        /// </summary>
        /// <param name="theAdo"></param>
        /// <param name="theCubeDTO"></param>
        /// <param name="theResponse"></param>
        /// <returns></returns>
        internal dynamic ExecuteReadCollection(ADO theAdo, Cube_DTO_ReadCollection DTO)
        {
            var ado = new Cube_ADO(theAdo);

            var dbData = ado.ReadCollectionMetadata(DTO.language, DTO.datefrom, DTO.product);

            List<dynamic> jsonStatCollection = new List<dynamic>();

            //Get a list of individual matrix data entities
            List<dynamic> releases = getReleases(dbData);


            var theJsonStatCollection = new JsonStatCollection();
            theJsonStatCollection.Link = new JsonStatCollectionLink();
            theJsonStatCollection.Link.Item = new List<Item>();

            List<Format_DTO_Read> formats = new List<Format_DTO_Read>();
            using (Format_BSO format = new Format_BSO(new ADO("defaultConnection")))
            {
                formats = format.Read(new Format_DTO_Read() { FrmDirection = Utility.GetCustomConfig("APP_FORMAT_DOWNLOAD_NAME") });
            };

            //For each of these, get a list of statistics and a list of classifications
            //Then get the JSON-stat for that metadata and add to jsonStatCollection
            foreach (var rls in releases)
            {
                List<dynamic> thisReleaseMetadata = dbData.Where(x => x.RlsCode == rls.RlsCode).Where(x => x.LngIsoCode == rls.LngIsoCode).ToList<dynamic>();

                List<dynamic> stats = getStatistics(thisReleaseMetadata);
                List<dynamic> classifications = getClassifications(thisReleaseMetadata);
                List<dynamic> periods = getPeriods(thisReleaseMetadata);
                theJsonStatCollection.Link.Item.Add(GetJsonStatRelease(thisReleaseMetadata, stats, classifications, periods, formats));


            }

            //Get the minimum next release date. The cache can only live until then.
            //If there's no next release date then the cache will live for the maximum configured amount.

            DateTime minDateItem = default;


            dynamic minimum = null;

            if (dbData != null)
            {
                minimum = dbData.Where(x => x.RlsLiveDatetimeFrom > DateTime.Now).Min(x => x.RlsLiveDatetimeFrom);
                minDateItem = minimum ?? default(DateTime);
            }

            if (minDateItem < DateTime.Now)
            {
                minDateItem = default(DateTime);
            }

            var result = new JRaw(Serialize.ToJson(theJsonStatCollection));

            MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadCollection", DTO, result, minDateItem, Constants.C_CAS_DATA_CUBE_READ_COLLECTION);


            // return the formatted data. This is an array of JSON-stat objects.
            return result;


        }

        internal Matrix GetMetadataMatrix(string LngIsoCode, string MtrCode)
        {
            var release = new Release_ADO(ado).ReadLiveNow(MtrCode, LngIsoCode);

            var result = Release_ADO.GetReleaseDTO(release);

            // The matrix constructor will load all the metadata from the db when instances specification
            return result != null ? new Matrix(ado, result, LngIsoCode) : null;
        }

        internal List<dynamic> ReadCollection(string LngIsoCode, string SbjCode, string PrcCode)
        {
            Cube_ADO cAdo = new Cube_ADO(ado);
            if (Int32.TryParse(SbjCode, out int sbjInt))
                return cAdo.ReadCollection(LngIsoCode, default, sbjInt, PrcCode);
            else
                return cAdo.ReadCollection(LngIsoCode, default, 0, PrcCode);
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
                                d.ExceptionalFlag,
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
                                rls.Key.ExceptionalFlag,
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
        private Item GetJsonStatRelease(List<dynamic> collection, List<dynamic> statistics, List<dynamic> classifications, List<dynamic> periods, List<Format_DTO_Read> formats)
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
            jsStat.Extension.Add("exceptional", thisItem.ExceptionalFlag);
            jsStat.Extension.Add("language", new { code = thisItem.LngIsoCode, name = thisItem.LngIsoName });
            jsStat.Extension.Add("matrix", thisItem.MtrCode);

            Format_DTO_Read fDtoMain = formats.Where(x => x.FrmType == Constants.C_SYSTEM_JSON_STAT_NAME && x.FrmVersion == Utility.GetCustomConfig("APP_JSON_STAT_QUERY_VERSION")).FirstOrDefault();// new Format_DTO_Read() { FrmDirection = Utility.GetCustomConfig("APP_FORMAT_DOWNLOAD_NAME"), FrmType = Constants.C_SYSTEM_JSON_STAT_NAME };



            jsStat.Href = new Uri(Configuration_BSO.GetCustomConfig(ConfigType.global, "url.restful") + string.Format(Utility.GetCustomConfig("APP_RESTFUL_DATASET"), Utility.GetCustomConfig("APP_READ_DATASET_API"), thisItem.MtrCode, fDtoMain.FrmType, fDtoMain.FrmVersion, thisItem.LngIsoCode));
            jsStat.Label = thisItem.MtrTitle;
            jsStat.Updated = DataAdaptor.ConvertToString(thisItem.RlsLiveDatetimeFrom);
            jsStat.Dimension = new Dictionary<string, Dimension>();

            formats.Remove(fDtoMain);


            var link = new DimensionLink
            {
                Alternate = new List<Alternate>()
            };
            foreach (var f in formats)
            {
                link.Alternate.Add(new Alternate() { Href = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.restful") + string.Format(Utility.GetCustomConfig("APP_RESTFUL_DATASET"), Utility.GetCustomConfig("APP_READ_DATASET_API"), thisItem.MtrCode, f.FrmType, f.FrmVersion, thisItem.LngIsoCode) });
            }
            jsStat.Link = link;


            formats.Add(fDtoMain);

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

        internal DateTime GetNextReleaseDate(string PrcCode = null)
        {
            Navigation_ADO adoNav = new Navigation_ADO(this.ado);
            //If we're cacheing, we only want the cache to live until the next scheduled release goes live
            //Also, if there is a next release before the scheduled cache expiry time, then we won't trust the cache
            var nextRelease = adoNav.ReadNextLiveDate(DateTime.Now);
            DateTime nextReleaseDate = default;
            if (nextRelease.hasData)
            {
                if (!nextRelease.data[0].NextRelease.Equals(DBNull.Value))
                    nextReleaseDate = Convert.ToDateTime(nextRelease.data[0].NextRelease);
            }

            return nextReleaseDate;
            /*
            if (cache.hasData && nextReleaseDate >= cache.expiresAt)
            {
                Response.data = cache.data;
                return true;
            }
            */
        }

        public void Dispose()
        {
            if (ado != null)
                ado.Dispose();
        }
    }
}
