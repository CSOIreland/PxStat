using Newtonsoft.Json;
using System.Collections.Generic;
using static PxStat.Data.Matrix;

namespace PxStat.Data
{
    public partial class PxApiMetadataJson
    {
        [JsonProperty("title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty("variables", Required = Required.Always)]
        public List<Variable> Variables { get; set; }
    }

    public partial class Variable
    {
        //code text values
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; set; }
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }
        [JsonProperty("values", Required = Required.Always)]
        public List<string> Values { get; set; }

        [JsonProperty("elimination", Required = Required.AllowNull)]
        public bool Elimination { get; set; }

    }


    internal class ApiMetadata
    {
        internal PxApiMetadataJson GetApiMetadata(Matrix theMatrix, string LngIsoCode)
        {
            Specification theSpec = theMatrix.GetSpecFromLanguage(LngIsoCode);


            PxApiMetadataJson json = new PxApiMetadataJson();
            json.Title = theSpec.Title;

            json.Variables = new List<Variable>();

            Variable statVariable = new Variable() { Code = theSpec.ContentVariable, Text = theSpec.Title };
            statVariable.Values = new List<string>();
            foreach (var stat in theSpec.Statistic)
            {
                statVariable.Values.Add(stat.Code);
            }
            json.Variables.Add(statVariable);

            Variable perVariable = new Variable() { Code = theSpec.Frequency.Code, Text = theSpec.Frequency.Value };
            perVariable.Values = new List<string>();
            foreach (var period in theSpec.Frequency.Period)
            {
                perVariable.Values.Add(period.Code);
            }
            json.Variables.Add(perVariable);

            foreach (var cls in theSpec.Classification)
            {
                Variable clsVariable = new Variable() { Code = cls.Code, Text = cls.Value };
                clsVariable.Values = new List<string>();
                foreach (var v in cls.Variable)
                {
                    clsVariable.Values.Add(v.Code);
                }
                json.Variables.Add(clsVariable);
            }

            return json;
        }
    }
}
