using API;
using DocumentFormat.OpenXml.Spreadsheet;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using PxStat.JsonStatSchema;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Navigation;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PxStat.Data
{
    internal class Cube_BSO : IDisposable
    {
        IADO ado;

        internal Cube_BSO(IADO Ado)
        {
            ado = Ado;
        }

        public Cube_BSO()
        {
        }



        internal Role UpdateRoleFromMetadata(IADO theAdo, CubeQuery_DTO dto)
        {
            Cube_ADO cAdo = new Cube_ADO(theAdo);
            dto.Role = new Role();
            Dictionary<string, string> roleDictionary;
            if (dto.jStatQueryExtension.extension.RlsCode == 0)
                roleDictionary = cAdo.ReadDimensionRoles(Configuration_BSO.GetStaticConfig("APP_CSV_STATISTIC"), dto.jStatQueryExtension.extension.Matrix);
            else
                roleDictionary = cAdo.ReadDimensionRoles(Configuration_BSO.GetStaticConfig("APP_CSV_STATISTIC"), null, dto.jStatQueryExtension.extension.RlsCode);

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


        internal List<dynamic> ReadCollectionList(string LngIsoCode, string SbjCode, string PrcCode)
        {
            Cube_ADO cAdo = new Cube_ADO(ado);
            if (Int32.TryParse(SbjCode, out int sbjInt))
                return cAdo.ReadListLive(LngIsoCode, default, sbjInt, PrcCode);
            else
                return cAdo.ReadListLive(LngIsoCode, default, 0, PrcCode);
        }

        /// <summary>
        /// Read and update title to contain dimensions and time range
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        internal List<dynamic> ReadTitleUpdate(DataTable dataTable)
        {
            Cube_ADO cAdo = new Cube_ADO(ado);
            return cAdo.ReadTitleUpdate(dataTable);
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
                       ).OrderBy(x => x.PrdCode).ToList<dynamic>();
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
        /// Get a list of classifications based on the collections database read
        /// </summary>
        /// <param name="releaseItems"></param>
        /// <returns></returns>
        private List<dynamic> getClassificationsNoVrbCount(List<dynamic> releaseItems)
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
        private List<dynamic> getReleases(List<dynamic> dbData, bool meta = true)

        {

            if (dbData == null) return new List<dynamic>();
            if (meta)
            {
                return (from d in dbData
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

            }
            else
            {
                return (from d in dbData
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
                            d.ExceptionalFlag
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
                            rls.Key.ExceptionalFlag
                        }
                                ).ToList<dynamic>();

            }
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

            var thisItem = collection.FirstOrDefault();

            jsStat.Extension = new Dictionary<string, object>();

            var Frequency = new { name = thisItem.FrqValue, code = thisItem.FrqCode };

            jsStat.Extension.Add("copyright", new { name = thisItem.CprValue, code = thisItem.CprCode, href = thisItem.CprUrl });
            jsStat.Extension.Add("exceptional", thisItem.ExceptionalFlag);
            jsStat.Extension.Add("language", new { code = thisItem.LngIsoCode, name = thisItem.LngIsoName });
            jsStat.Extension.Add("matrix", thisItem.MtrCode);

            Format_DTO_Read fDtoMain = formats.Where(x => x.FrmType == Constants.C_SYSTEM_JSON_STAT_NAME && x.FrmVersion == Constants.C_SYSTEM_JSON_STAT_2X_VERSION).FirstOrDefault();// new Format_DTO_Read() { FrmDirection = Configuration_BSO.GetStaticConfig("APP_FORMAT_DOWNLOAD_NAME"), FrmType = Constants.C_SYSTEM_JSON_STAT_NAME };



            jsStat.Href = new Uri(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' + string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), thisItem.MtrCode, fDtoMain.FrmType, fDtoMain.FrmVersion, thisItem.LngIsoCode));
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

                link.Alternate.Add(new Alternate() { Href = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' + string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), thisItem.MtrCode, f.FrmType, f.FrmVersion, thisItem.LngIsoCode), Type = f.FrmMimetype });
            }
            jsStat.Link = link;


            formats.Add(fDtoMain);



            var statDimension = new Dimension()
            {

                Label = Configuration_BSO.GetStaticConfig ("APP_CSV_STATISTIC"),
                Id = Configuration_BSO.GetStaticConfig("APP_CSV_STATISTIC"),
                Category = new Category()
                {

                }

            };




            jsStat.Dimension.Add(Configuration_BSO.GetStaticConfig("APP_CSV_STATISTIC"), statDimension);

            jsStat.Id.Add(Frequency.code);
            jsStat.Id.Add(statDimension.Id);

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

                jsStat.Id.Add(s.ClsCode);
            }


            jsStat.Role = new Role();
            jsStat.Role.Metric = new List<string>
            {
                Configuration_BSO.GetStaticConfig("APP_CSV_STATISTIC")
            };
            jsStat.Role.Time = new List<string>
            {
                thisItem.FrqCode
            };

            return jsStat;
        }

        /// <summary>
        /// Get a JSON-stat metadata based on a database read of collections
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="statistics"></param>
        /// <param name="classifications"></param>
        /// <returns></returns>
        private Item GetJsonStatReleaseNoCollections(List<dynamic> collection, List<Format_DTO_Read> formats, List<dynamic> classifications)
        {

            var jsStat = new Item();
            // jsStat.Class = Class.Dataset;
            //jsStat.Version = Version.The20;

            // jsStat.Dimension = new Dictionary<string, Dimension>();
            jsStat.Id = new List<string>();

            var thisItem = collection.FirstOrDefault();

            jsStat.Extension = new Dictionary<string, object>();


            jsStat.Extension.Add("copyright", new { name = thisItem.CprValue, code = thisItem.CprCode, href = thisItem.CprUrl });
            jsStat.Extension.Add("exceptional", thisItem.ExceptionalFlag);
            jsStat.Extension.Add("language", new { code = thisItem.LngIsoCode, name = thisItem.LngIsoName });
            jsStat.Extension.Add("matrix", thisItem.MtrCode);

            Format_DTO_Read fDtoMain = formats.Where(x => x.FrmType == Constants.C_SYSTEM_JSON_STAT_NAME && x.FrmVersion == Constants.C_SYSTEM_JSON_STAT_2X_VERSION).FirstOrDefault();// new Format_DTO_Read() { FrmDirection = Configuration_BSO.GetStaticConfig("APP_FORMAT_DOWNLOAD_NAME"), FrmType = Constants.C_SYSTEM_JSON_STAT_NAME };



            jsStat.Href = new Uri(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' + string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), thisItem.MtrCode, fDtoMain.FrmType, fDtoMain.FrmVersion, thisItem.LngIsoCode));
            jsStat.Label = thisItem.MtrTitle;
            jsStat.Updated = DataAdaptor.ConvertToString(thisItem.RlsLiveDatetimeFrom);



            var link = new DimensionLink
            {
                Alternate = new List<Alternate>()
            };
            foreach (var f in formats)
            {
                if (f.FrmType != fDtoMain.FrmType || f.FrmVersion != fDtoMain.FrmVersion)
                    link.Alternate.Add(new Alternate() { Href = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' + string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), thisItem.MtrCode, f.FrmType, f.FrmVersion, thisItem.LngIsoCode), Type = f.FrmMimetype });
            }
            jsStat.Link = link;

            var Frequency = new { name = thisItem.FrqValue, code = thisItem.FrqCode };

            var statDimension = new Dimension()
            {

                Label = Configuration_BSO.GetStaticConfig("APP_CSV_STATISTIC")

            };

            //jsStat.Dimension.Add(statDimension.Label, statDimension);

            var timeDimension = new Dimension()
            {

                Label = Frequency.name
            };

            // jsStat.Dimension.Add(Frequency.code, timeDimension);

            jsStat.Id.Add(Frequency.code);
            jsStat.Id.Add(Configuration_BSO.GetStaticConfig("APP_CSV_STATISTIC"));

            foreach (var s in classifications)
            {
                //Dictionary<string, string> dict = new Dictionary<string, string>();
                //dict.Add(s.ClsCode, s.ClsValue);
                //var clsDimension = new Dimension()
                //{

                //    Label = s.ClsValue
                //};


                jsStat.Id.Add(s.ClsCode);
            }

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

        /// <summary>
        /// Sort a Json object
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        internal JObject Sort(JObject jObj)
        {
            var props = jObj.Properties().ToList();
            foreach (var prop in props)
            {
                prop.Remove();
            }

            foreach (var prop in props.OrderBy(p => p.Name))
            {
                jObj.Add(prop);
                if (prop.Value is JObject)
                    Sort((JObject)prop.Value);
            }
            return jObj;

        }



        public void Dispose()
        {
            ado?.Dispose();
        }
    }
}
