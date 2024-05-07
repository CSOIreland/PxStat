using API;
using Newtonsoft.Json.Linq;
using PxStat.JsonQuery;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Data
{
    /// <summary>
    /// Contains methods for mapping across Cube requests, i.e. RESTful to Json-rpc
    /// </summary>
    internal class Cube_MAP
    {
        internal string MimeType { get; set; }
        internal string FileName { get; set; }
        internal Cube_MAP() { }
        /// <summary>
        /// Map RESTful parameters to JsonRpc parameters
        /// </summary>
        /// <param name="restfulParameters"></param>
        /// <returns></returns>
        internal dynamic ReadCollection_MapParameters(dynamic restfulParameters)
        {

            var prm = JObject.FromObject(new
            {
                datefrom = restfulParameters.Count >= 2 ? restfulParameters[1] : null,
                language = restfulParameters.Count >= 3 ? restfulParameters[2] : null
            }
            );

            // To make the parameters agnostic of trailing slashes:
            if (restfulParameters.Count >= 2)
                if (restfulParameters[1].ToString().Length == 0) prm["datefrom"] = default;
            if (restfulParameters.Count >= 3)
                if (restfulParameters[2].ToString().Length == 0) prm["language"] = null;



            return prm;


        }

        /// <summary>
        /// Map RESTful parameters to JsonRpc parameters
        /// </summary>
        /// <param name="restfulParameters"></param>
        /// <returns></returns>
        internal dynamic ReadMetadata_MapParameters(dynamic restfulParameters)
        {
            Format_DTO_Read format;
            using (Format_BSO bso = new Format_BSO(AppServicesHelper.StaticADO))
            {
                format = bso.Read(new Format_DTO_Read() { FrmType = Constants.C_SYSTEM_JSON_STAT_NAME, FrmDirection = Format_DTO_Read.FormatDirection.DOWNLOAD.ToString() }).FirstOrDefault();
            }

            //Create a null format if one is not found. This will be caught in validation
            if (format == null) format = new Format_DTO_Read();

            var prm = JObject.FromObject(new
            {
                matrix = restfulParameters.Count >= 2 ? restfulParameters[1] : null,
                language = restfulParameters.Count >= 3 ? restfulParameters[2] : null,
                format = new { type = format.FrmType, version = format.FrmVersion }

            });

            // To make the parameters agnostic of trailing slashes:
            if (restfulParameters.Count >= 2)
                if (restfulParameters[1].ToString().Length == 0) prm["matrix"] = null;
            if (restfulParameters.Count >= 3)
                if (restfulParameters[2].ToString().Length == 0) prm["language"] = null;

            return prm;



        }



        internal JsonStatQuery SortJsonStatQuery(JsonStatQuery jsQuery)
        {

            JsonStatQuery jsSorted = new JsonStatQuery();
            jsSorted.Class = jsQuery.Class;
            jsSorted.Id = jsQuery.Id;
            jsSorted.Id.Sort();
            jsSorted.Dimensions = new Dictionary<string, JsonQuery.Dimension>();
            List<string> klist = jsQuery.Dimensions.Keys.ToList();
            klist.Sort();
            foreach (string k in klist)
            {
                jsSorted.Dimensions.Add(k, jsQuery.Dimensions[k]);
            }
            foreach (var dim in jsSorted.Dimensions)
            {
                dim.Value.Category.Index.Sort();
            }
            jsSorted.Extension = new Dictionary<string, object>();
            jsSorted.Extension.Add("pivot", jsQuery.Extension.ContainsKey("pivot") ? jsQuery.Extension["pivot"] : null);
            jsSorted.Extension.Add("codes", jsQuery.Extension.ContainsKey("codes") ? jsQuery.Extension["codes"] : null);
            jsSorted.Extension.Add("language", jsQuery.Extension["language"]);

            JObject jobj = (JObject)jsSorted.Extension["language"];
            if (!jobj.ContainsKey("culture"))
            {
                jobj.Add("culture", null);
            }

            jsSorted.Extension.Add("format", jsQuery.Extension["format"]);

            jsSorted.Version = jsQuery.Version;

            return jsSorted;

        }

        private void Sort(JObject jObj)
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
        }

        internal JsonStatQuery ReadMetadata_MapParametersToQuery(dynamic restfulParameters)
        {

            var parameters = ((List<string>)restfulParameters).Where(x => !string.IsNullOrWhiteSpace(x));
            int pcount = parameters.Count() - 1;
            JsonStatQuery jsQuery = new JsonStatQuery();
            jsQuery.Class = Constants.C_JSON_STAT_QUERY_CLASS;
            jsQuery.Id = new List<string>();
            jsQuery.Dimensions = new Dictionary<string, JsonQuery.Dimension>();
            jsQuery.Extension = new Dictionary<string, object>();

            jsQuery.Extension.Add("matrix", restfulParameters[Constants.C_DATA_RESTFUL_MATRIX]); //1
            jsQuery.Extension.Add("language", new Language() { Code = restfulParameters[Constants.C_DATA_RESTFUL_LANGUAGE], Culture = null }); //4
            jsQuery.Extension.Add("codes", false);
            jsQuery.Extension.Add("format", new JsonQuery.Format() { Type = restfulParameters[Constants.C_DATA_RESTFUL_FORMAT_TYPE], Version = restfulParameters[Constants.C_DATA_RESTFUL_FORMAT_VERSION] }); //2,3
            if (pcount >= Constants.C_DATA_RESTFUL_PIVOT)
            {
                jsQuery.Extension.Add("pivot", restfulParameters[Constants.C_DATA_RESTFUL_PIVOT]); //5
            }
            jsQuery.Version = Constants.C_JSON_STAT_QUERY_VERSION;


            return jsQuery;
        }

        /// <summary>
        /// Map RESTful parameters to JsonRpc parameters 
        /// </summary>
        /// <param name="restfulParameters"></param>
        /// <returns></returns>
        internal dynamic ReadDataset_MapParameters(dynamic restfulParameters)
        {
            dynamic jsonStatParameters = new ExpandoObject();
            dynamic obj = new ExpandoObject();
            var dict = (IDictionary<string, object>)obj;


            dynamic extension = new ExpandoObject();
            dynamic dimension = new ExpandoObject();

            extension.codes = false;

            extension.matrix = restfulParameters[1];

            extension.language = restfulParameters.Count >= 5 ? new { code = restfulParameters[4] } : new { code = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") };

            if (((string)extension.language.code).Length == 0) extension.language = new { code = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") };


            extension.format = new { type = restfulParameters[2], version = restfulParameters[3] };

            if (restfulParameters.Count >= 6)
                extension.pivot = restfulParameters[5];
            else
                extension.pivot = null;

            jsonStatParameters.extension = extension;
            jsonStatParameters.dimension = dimension;

            dict["class"] = Constants.C_JSON_STAT_QUERY_CLASS;
            dict["id"] = new string[] { };
            dict["dimension"] = dimension;
            dict["extension"] = extension;
            dict["version"] = Constants.C_JSON_STAT_QUERY_VERSION;
            dict["m2m"] = true;


            string suffix;
            using (Format_BSO bso = new Format_BSO(AppServicesHelper.StaticADO))
            {
                this.MimeType = bso.GetMimetypeForFormat(new Format_DTO_Read() { FrmType = restfulParameters[2], FrmVersion = restfulParameters[3] });

                suffix = bso.GetFileSuffixForFormat(new Format_DTO_Read() { FrmType = restfulParameters[2], FrmVersion = restfulParameters[3] });
            };


            if (suffix != null) this.FileName = extension.matrix + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;

            return Utility.JsonSerialize_IgnoreLoopingReference(dict);
        }
    }
}
