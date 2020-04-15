using API;
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



        public CubeQuery_DTO(dynamic parameters)
        {

            jStatQuery = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQuery>(parameters.ToString());
            jStatQueryExtension = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQueryExtension>(parameters.ToString());

            foreach (var v in jStatQuery.Dimensions)
            {
                v.Value.Id = v.Key;

            }

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
