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

    public partial class PxApiMetadataJsonDmatrix
    {
        [JsonProperty("title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty("variables", Required = Required.Always)]
        public List<DVariable> Variables { get; set; }
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
        public List<string> valueTexts { get; set; }

        [JsonProperty("elimination", Required = Required.AllowNull)]
        public bool Elimination { get; set; }

    }

    public class DVariable
    {
        //code text values
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; set; }
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }
        [JsonProperty("values", Required = Required.Always)]
        public List<string> Values { get; set; }
        public List<string> valueTexts { get; set; }

        [JsonProperty("elimination", Required = Required.AllowNull)]
        public bool Elimination { get; set; }

    }


    internal class ApiMetadata
    {
        internal PxApiMetadataJsonDmatrix GetApiMetadata(IDmatrix dmatrix, string LngIsoCode)
        {
            Dspec theSpec = dmatrix.Dspecs[LngIsoCode];


            PxApiMetadataJsonDmatrix json = new PxApiMetadataJsonDmatrix();
            json.Title = theSpec.Title;

            json.Variables = new List<DVariable>();



            foreach (var dim in theSpec.Dimensions)
            {
                DVariable dimvar = new DVariable() { Code = dim.Code, Text = dim.Value };

                dimvar.Values = new List<string>();
                dimvar.valueTexts = new List<string>();
                foreach (var v in dim.Variables)
                {
                    dimvar.Values.Add(v.Code);
                    dimvar.valueTexts.Add(v.Value);
                }
                json.Variables.Add(dimvar);
            }

            return json;
        }
        internal PxApiMetadataJson GetApiMetadata(Matrix theMatrix, string LngIsoCode)
        {
            Specification theSpec = theMatrix.GetSpecFromLanguage(LngIsoCode);


            PxApiMetadataJson json = new PxApiMetadataJson();
            json.Title = theSpec.Title;

            json.Variables = new List<Variable>();

            Variable statVariable = new Variable() { Code = theSpec.ContentVariable, Text = theSpec.Title };
            statVariable.Values = new List<string>();
            statVariable.valueTexts = new List<string>();
            foreach (var stat in theSpec.Statistic)
            {
                statVariable.Values.Add(stat.Code);
                statVariable.valueTexts.Add(stat.Value);
            }
            json.Variables.Add(statVariable);

            Variable perVariable = new Variable() { Code = theSpec.Frequency.Code, Text = theSpec.Frequency.Value };
            perVariable.Values = new List<string>();
            perVariable.valueTexts = new List<string>();
            foreach (var period in theSpec.Frequency.Period)
            {
                perVariable.Values.Add(period.Code);
                perVariable.valueTexts.Add(period.Value);
            }
            json.Variables.Add(perVariable);

            foreach (var cls in theSpec.Classification)
            {
                Variable clsVariable = new Variable() { Code = cls.Code, Text = cls.Value };
                clsVariable.Values = new List<string>();
                clsVariable.valueTexts = new List<string>();
                foreach (var v in cls.Variable)
                {
                    clsVariable.Values.Add(v.Code);
                    clsVariable.valueTexts.Add(v.Value);
                }
                json.Variables.Add(clsVariable);
            }

            return json;
        }
    }
}
