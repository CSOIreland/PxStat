using API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxStat.Data;
using PxStat.JsonQuery;
using System.Collections.Generic;
using System.Globalization;

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

        public bool m2m { get; set; }


        public CubeQuery_DTO(dynamic parameters)
        {

            Cube_BSO cBso = new Cube_BSO();

            string sortedObject = cBso.Sort((JObject)JsonConvert.DeserializeObject(parameters.ToString())).ToString();
            jStatQuery = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQuery>(sortedObject);
            jStatQueryExtension = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQueryExtension>(sortedObject);

            jStatQuery = new Cube_MAP().SortJsonStatQuery(jStatQuery);


            foreach (var v in jStatQuery.Dimensions)
            {
                v.Value.Id = v.Key;

            }


            var param = Utility.JsonDeserialize_IgnoreLoopingReference<dynamic>(parameters.ToString());
            if (param.m2m != null)
                m2m = param.m2m;
            else
                m2m = true;

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
