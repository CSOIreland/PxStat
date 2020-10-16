using API;
using Newtonsoft.Json.Linq;
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
            using (Format_BSO bso = new Format_BSO(new ADO("defaultConnection")))
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



            extension.matrix = restfulParameters[1];

            extension.language = restfulParameters.Count >= 5 ? new { code = restfulParameters[4] } : new { code = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code") };

            if (((string)extension.language.code).Length == 0) extension.language = new { code = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code") };

            extension.format = new { type = restfulParameters[2], version = restfulParameters[3] };


            jsonStatParameters.extension = extension;
            jsonStatParameters.dimension = dimension;

            dict["class"] = "query";
            dict["id"] = new string[] { };
            dict["dimension"] = dimension;
            dict["extension"] = extension;
            dict["version"] = "2.0";
            dict["m2m"] = true;


            string suffix;
            using (Format_BSO bso = new Format_BSO(new ADO("defaultConnection")))
            {
                this.MimeType = bso.GetMimetypeForFormat(new Format_DTO_Read() { FrmType = restfulParameters[2], FrmVersion = restfulParameters[3] });

                suffix = bso.GetFileSuffixForFormat(new Format_DTO_Read() { FrmType = restfulParameters[2], FrmVersion = restfulParameters[3] });
            };


            if (suffix != null) this.FileName = extension.matrix + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;

            return Utility.JsonSerialize_IgnoreLoopingReference(dict);
        }
    }
}
