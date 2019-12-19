using API;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PxStat.Data
{


    /// <summary>
    /// This is version 1.04 of the JSON-stat 2.0 Dataset Schema (2018-09-05 10:55)
    /// </summary>
    public partial class JsonStat
    {
        [JsonProperty("class", Required = Required.Always)]
        public Class Class { get; set; }

        [JsonProperty("dimension", Required = Required.Always)]
        public Dictionary<string, Dimension> Dimension { get; set; }

        [JsonProperty("error", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Error { get; set; }

        [JsonProperty("extension", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Extension { get; set; }

        [JsonProperty("href", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri Href { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public List<string> Id { get; set; }

        [JsonProperty("label", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        [JsonProperty("link", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Link Link { get; set; }

        [JsonProperty("note", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Note { get; set; }

        [JsonProperty("role", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Role Role { get; set; }

        [JsonProperty("size", Required = Required.Always)]
        public List<long> Size { get; set; }

        [JsonProperty("source", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("status", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Status? Status { get; set; }

        [JsonProperty("updated", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Updated { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public JsonStatValue Value { get; set; }

        [JsonProperty("version", Required = Required.Always)]
        public Version Version { get; set; }
    }

    /// <summary>
    /// Class
    /// </summary>
    public partial class Dimension
    {
        [JsonProperty("category", Required = Required.Always)]
        public Category Category { get; set; }

        [JsonProperty("extension", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Extension { get; set; }

        [JsonProperty("href", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri Href { get; set; }

        [JsonProperty("label", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        [JsonProperty("link", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Link Link { get; set; }

        [JsonProperty("alternate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Alternate Alternate { get; set; }

        [JsonProperty("note", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Note { get; set; }

        [JsonProperty("id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }

    /// <summary>
    /// Class
    /// </summary>
    public partial class Category
    {
        [JsonProperty("child", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<string>> Child { get; set; }

        [JsonProperty("coordinates", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<double>> Coordinates { get; set; }

        [JsonProperty("index", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Index? Index { get; set; }

        [JsonProperty("label", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Label { get; set; }

        [JsonProperty("note", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<string>> Note { get; set; }

        [JsonProperty("unit", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Unit> Unit { get; set; }
    }

    /// <summary>
    /// Class
    /// </summary>
    public partial class Unit
    {
        [JsonProperty("decimals", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Decimals { get; set; }

        [JsonProperty("label", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        [JsonProperty("position", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Position? Position { get; set; }
    }

    /// <summary>
    /// Class
    /// </summary>
    public partial class Link
    {
    }

    /// <summary>
    /// Class
    /// </summary>
    public partial class Role
    {
        [JsonProperty("geo", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Geo { get; set; }

        [JsonProperty("metric", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Metric { get; set; }

        [JsonProperty("time", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Time { get; set; }
    }

    /// <summary>
    /// Enum
    /// </summary>
    public enum Class { Dataset };

    /// <summary>
    /// Enum
    /// </summary>
    public enum Position { End, Start };

    /// <summary>
    /// Enum
    /// </summary>
    public enum Version { The20 };

    /// <summary>
    /// Struct
    /// </summary>
    public partial struct Index
    {
        public Dictionary<string, double> DoubleMap;
        public List<string> StringArray;

        public static implicit operator Index(Dictionary<string, double> DoubleMap) => new Index { DoubleMap = DoubleMap };
        public static implicit operator Index(List<string> StringArray) => new Index { StringArray = StringArray };
    }

    /// <summary>
    /// Struct
    /// </summary>
    public partial struct Status
    {
        public string String;
        public Dictionary<string, string> StringMap;
        public List<string> UnionArray;

        public static implicit operator Status(string String) => new Status { String = String };
        public static implicit operator Status(Dictionary<string, string> StringMap) => new Status { StringMap = StringMap };
        public static implicit operator Status(List<string> UnionArray) => new Status { UnionArray = UnionArray };
    }

    /// <summary>
    /// Struct
    /// </summary>
    public partial struct ValueElement
    {

        public double? Double;
        public string String;

        public static implicit operator ValueElement(double Double) => new ValueElement { Double = Double };
        public static implicit operator ValueElement(DBNull DbNull) => new ValueElement { String = null };

        public static implicit operator ValueElement(string String)
        {
            double result;
            if (string.IsNullOrEmpty(String))
                return new ValueElement { String = null };
            else if (double.TryParse(String, out result))
                return new ValueElement { Double = result };
            else if (String.Equals(Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE"))) return new ValueElement { String = null };
            else if (String.Contains("\""))
            {
                String = String.Replace("\"", String.Empty);
                return new ValueElement { String = String };
            }

            return new ValueElement { String = String };
        }
        public bool IsNull => Double == null && String == null;
    }

    /// <summary>
    /// Struct
    /// </summary>
    public partial struct JsonStatValue
    {
        public List<ValueElement> AnythingArray;
        public Dictionary<string, ValueElement> AnythingMap;

        public static implicit operator JsonStatValue(List<ValueElement> AnythingArray) => new JsonStatValue { AnythingArray = AnythingArray };
        public static implicit operator JsonStatValue(Dictionary<string, ValueElement> AnythingMap) => new JsonStatValue { AnythingMap = AnythingMap };
    }

    /// <summary>
    /// Class
    /// </summary>
    public partial class JsonStat
    {
        public static JsonStat FromJson(string json) => JsonConvert.DeserializeObject<JsonStat>(json, Converter.Settings);
    }

    /// <summary>
    /// Class
    /// </summary>
    public static partial class Serialize
    {
        public static string ToJson(this JsonStat self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    /// <summary>
    /// Class
    /// </summary>
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ClassConverter.Singleton,
                IndexConverter.Singleton,
                PositionConverter.Singleton,
                StatusConverter.Singleton,
                JsonStatValueConverter.Singleton,
                ValueElementConverter.Singleton,
                VersionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    /// <summary>
    /// Class
    /// </summary>
    internal class ClassConverter : JsonConverter
    {
        /// <summary>
        /// Tests if a type can be converted
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvert(Type t) => t == typeof(Class) || t == typeof(Class?);

        /// <summary>
        /// Returns an object based on a JsonReader object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="t"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "dataset")
            {
                return Class.Dataset;
            }
            throw new Exception("Cannot unmarshal type Class");
        }

        /// <summary>
        /// Write Json
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="untypedValue"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Class)untypedValue;
            if (value == Class.Dataset)
            {
                serializer.Serialize(writer, "dataset");
                return;
            }
            throw new Exception("Cannot marshal type Class");
        }

        /// <summary>
        /// ClassConverter
        /// </summary>
        public static readonly ClassConverter Singleton = new ClassConverter();
    }

    /// <summary>
    /// Class
    /// </summary>
    internal class IndexConverter : JsonConverter
    {
        /// <summary>
        /// Tests if Conversion is possible
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvert(Type t) => t == typeof(Index) || t == typeof(Index?);

        /// <summary>
        /// ReadJson as an object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="t"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<Dictionary<string, double>>(reader);
                    return new Index { DoubleMap = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<string>>(reader);
                    return new Index { StringArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type Index");
        }

        /// <summary>
        /// Write Json
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="untypedValue"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Index)untypedValue;
            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray);
                return;
            }
            if (value.DoubleMap != null)
            {
                serializer.Serialize(writer, value.DoubleMap);
                return;
            }
            throw new Exception("Cannot marshal type Index");
        }

        public static readonly IndexConverter Singleton = new IndexConverter();
    }

    /// <summary>
    /// Position converter
    /// </summary>
    internal class PositionConverter : JsonConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvert(Type t) => t == typeof(Position) || t == typeof(Position?);

        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="t"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "end":
                    return Position.End;
                case "start":
                    return Position.Start;
            }
            throw new Exception("Cannot unmarshal type Position");
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="untypedValue"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Position)untypedValue;
            switch (value)
            {
                case Position.End:
                    serializer.Serialize(writer, "end");
                    return;
                case Position.Start:
                    serializer.Serialize(writer, "start");
                    return;
            }
            throw new Exception("Cannot marshal type Position");
        }

        /// <summary>
        /// PositionConverter
        /// </summary>
        public static readonly PositionConverter Singleton = new PositionConverter();
    }

    /// <summary>
    /// StatusConverter
    /// </summary>
    internal class StatusConverter : JsonConverter
    {
        /// <summary>
        /// Tests CanConvert
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvert(Type t) => t == typeof(Status) || t == typeof(Status?);

        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="t"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Status { String = stringValue };
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<Dictionary<string, string>>(reader);
                    return new Status { StringMap = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<string>>(reader);
                    return new Status { UnionArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type Status");
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="untypedValue"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Status)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.UnionArray != null)
            {
                serializer.Serialize(writer, value.UnionArray);
                return;
            }
            if (value.StringMap != null)
            {
                serializer.Serialize(writer, value.StringMap);
                return;
            }
            throw new Exception("Cannot marshal type Status");
        }

        /// <summary>
        /// StatusConverter
        /// </summary>
        public static readonly StatusConverter Singleton = new StatusConverter();
    }

    /// <summary>
    /// JsonStatValueConverter
    /// </summary>
    internal class JsonStatValueConverter : JsonConverter
    {
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvert(Type t) => t == typeof(JsonStatValue) || t == typeof(JsonStatValue?);

        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="t"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<Dictionary<string, ValueElement>>(reader);
                    return new JsonStatValue { AnythingMap = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<ValueElement>>(reader);
                    return new JsonStatValue { AnythingArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type JsonStatValue");
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="untypedValue"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (JsonStatValue)untypedValue;
            if (value.AnythingArray != null)
            {
                serializer.Serialize(writer, value.AnythingArray);
                return;
            }
            if (value.AnythingMap != null)
            {
                serializer.Serialize(writer, value.AnythingMap);
                return;
            }
            throw new Exception("Cannot marshal type JsonStatValue");
        }

        /// <summary>
        /// JsonStatValueConverter
        /// </summary>
        public static readonly JsonStatValueConverter Singleton = new JsonStatValueConverter();
    }

    /// <summary>
    /// ValueElementConverter
    /// </summary>
    internal class ValueElementConverter : JsonConverter
    {
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvert(Type t) => t == typeof(ValueElement) || t == typeof(ValueElement?);

        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="t"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return new ValueElement { };
                case JsonToken.Integer:
                case JsonToken.Float:
                    var doubleValue = serializer.Deserialize<double>(reader);
                    return new ValueElement { Double = doubleValue };
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new ValueElement { String = stringValue };
            }
            throw new Exception("Cannot unmarshal type ValueElement");
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="untypedValue"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ValueElement)untypedValue;
            if (value.IsNull)
            {
                serializer.Serialize(writer, null);
                return;
            }
            if (value.Double != null)
            {
                serializer.Serialize(writer, value.Double.Value);
                return;
            }
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            throw new Exception("Cannot marshal type ValueElement");
        }

        /// <summary>
        /// ValueElementConverter
        /// </summary>
        public static readonly ValueElementConverter Singleton = new ValueElementConverter();
    }

    /// <summary>
    /// VersionConverter
    /// </summary>
    internal class VersionConverter : JsonConverter
    {
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvert(Type t) => t == typeof(Version) || t == typeof(Version?);

        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="t"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "2.0")
            {
                return Version.The20;
            }
            throw new Exception("Cannot unmarshal type Version");
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="untypedValue"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Version)untypedValue;
            if (value == Version.The20)
            {
                serializer.Serialize(writer, "2.0");
                return;
            }
            throw new Exception("Cannot marshal type Version");
        }

        /// <summary>
        /// VersionConverter
        /// </summary>
        public static readonly VersionConverter Singleton = new VersionConverter();
    }
}
