using API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxStat.Data;
using PxStat.JsonQuery;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using PxStat;

namespace PxStat.JsonStatSchema
{

    public class CubeQuery_DTO
    {
        public string Class { get; set; }
        public List<Dimension> Dimensions { get; set; }
        public string Version { get; set; }
        public Extension Extension { get; set; }
        public Role Role { get; set; }
        public JsonStatQuery jStatQuery { get; set; }
        public JsonStatQueryExtension jStatQueryExtension { get; set; }

        public bool? m2m { get; set; }

        public bool widget { get; set; }
        public bool user { get; set; }

        /// <summary>
        /// Indicates whether or not the Notes should be returned with Comments
        /// If true, then notes only
        /// </summary>
        public bool build { get; set; }

        public CubeQuery_DTO()
        {
        }

        public CubeQuery_DTO(dynamic parameters)
        {
            Cube_MAP map = new Cube_MAP();

            

            //Cheeck if the parameters are in key value pairs (e.g. JSON-rpc) or in a list (e.g. RESTful)
            if (!Resources.Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {

                if (parameters.Count <= Constants.C_DATA_RESTFUL_LANGUAGE)
                {
                    parameters.Add(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"));
                }

                //A trailing '/' where language should be may be picked up as a blank last parameter, so let's deal with that...
                if (parameters.Count > Constants.C_DATA_RESTFUL_LANGUAGE)
                {
                    if (String.IsNullOrEmpty(parameters[Constants.C_DATA_RESTFUL_LANGUAGE]))
                        parameters[Constants.C_DATA_RESTFUL_LANGUAGE] = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
                }

                JsonStatQuery jq = map.ReadMetadata_MapParametersToQuery(parameters);

                jq.Extension["codes"] = true;

                parameters = Utility.JsonSerialize_IgnoreLoopingReference(jq);
            }



            Cube_BSO cBso = new Cube_BSO();

            string sortedObject = cBso.Sort((JObject)JsonConvert.DeserializeObject(parameters.ToString())).ToString();
            jStatQuery = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQuery>(sortedObject);
            jStatQueryExtension = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQueryExtension>(sortedObject);

            jStatQuery = new Cube_MAP().SortJsonStatQuery(jStatQuery);


            foreach (var v in jStatQuery.Dimensions)
            {
                v.Value.Id = v.Key;

            }

            /*User : Person consuming from PxStat
            m2m: false (existing behaviour) && referrer == global url application (additional requirement on server)

            M2m : Anything consuming raw API’s
            M2m missing or == true (existing behaviour) 

            Widget: User consuming via snippet code
            m2m: false (additional requirement) && referrer != global url application (additional requirement on server)*/

            //Set some analytic properties if they exist
            var param = Utility.JsonDeserialize_IgnoreLoopingReference<dynamic>(parameters.ToString());
            if (param.m2m != null)
                m2m = param.m2m;
            else
                m2m = true;

            if (param.build != null)
                this.build = parameters.build;


        }




    }

    public class Dimension
    {
        public Category Category { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
    }


    public class Category
    {
        public List<string> Index { get; set; }
    }

    public class Extension
    {
        public string Matrix { get; set; }
        public Language Language { get; set; }
        public Format Format { get; set; }
        public string RlsCode { get; set; }
        public string Pivot { get; set; }
        public bool? Codes { get; set; }
    }

    public class Language
    {
        public string Code { get; set; }
        public CultureInfo Culture { get; set; }
        public string CultureCode { get; set; }
    }

    public class Format
    {
        public string Type { get; set; }
        public string Version { get; set; }
    }
}
