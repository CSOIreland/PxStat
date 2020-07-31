using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PxStat.Data
{

    /// <summary>
    /// This is version 1.04 of the JSON-stat 2.0 Collection Schema (2018-09-05 10:55)
    /// </summary>
    public partial class JsonStatCollection
    {
        [JsonProperty("class", Required = Required.Always)]
        public JsonStatCollectionClass Class { get; set; }

        [JsonProperty("error", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Error { get; set; }

        [JsonProperty("extension", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Extension { get; set; }

        [JsonProperty("href", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri Href { get; set; }

        [JsonProperty("label", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        [JsonProperty("link", Required = Required.Always)]
        public JsonStatCollectionLink Link { get; set; }

        [JsonProperty("note", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Note { get; set; }

        [JsonProperty("source", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("updated", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Updated { get; set; }

        [JsonProperty("version", Required = Required.Always)]
        public Version Version { get; set; }
    }

    /// <summary>
    /// JsonStatCollectionLink
    /// </summary>
    public partial class JsonStatCollectionLink
    {
        [JsonProperty("item", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Item> Item { get; set; }
    }

    /// <summary>
    /// Item
    /// </summary>
    public partial class Item
    {
        [JsonProperty("category", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Category Category { get; set; }

        [JsonProperty("class", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ItemClass? Class { get; set; }

        [JsonProperty("dimension", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Dimension> Dimension { get; set; }

        [JsonProperty("extension", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Extension { get; set; }

        [JsonProperty("href", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri Href { get; set; }

        [JsonProperty("id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Id { get; set; }

        [JsonProperty("label", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        [JsonProperty("link", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public DimensionLink Link { get; set; }

        [JsonProperty("note", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Note { get; set; }

        [JsonProperty("role", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Role Role { get; set; }

        [JsonProperty("size", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<long> Size { get; set; }

        [JsonProperty("source", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("status", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Status? Status { get; set; }

        [JsonProperty("type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("updated", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Updated { get; set; }

        [JsonProperty("value", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ItemValue? Value { get; set; }
    }

    public partial class DimensionLink
    {
        [JsonProperty("alternate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Alternate> Alternate { get; set; }
    }



    /// <summary>
    /// Enum
    /// </summary>
    public enum JsonStatCollectionClass { Collection };


    /// <summary>
    /// Enum
    /// </summary>
    public enum ItemClass { Collection, Dataset, Dimension };



    /// <summary>
    /// Struct
    /// </summary>
    public partial struct ItemValue
    {
        public List<ValueElement> AnythingArray;
        public Dictionary<string, ValueElement> AnythingMap;

        public static implicit operator ItemValue(List<ValueElement> AnythingArray) => new ItemValue { AnythingArray = AnythingArray };
        public static implicit operator ItemValue(Dictionary<string, ValueElement> AnythingMap) => new ItemValue { AnythingMap = AnythingMap };
    }

    /// <summary>
    /// JsonStatCollection
    /// </summary>
    public partial class JsonStatCollection
    {
        public static JsonStatCollection FromJson(string json) => JsonConvert.DeserializeObject<JsonStatCollection>(json, Converter.Settings);
    }

    /// <summary>
    /// Serialize
    /// </summary>
    public static partial class Serialize
    {
        public static string ToJson(this JsonStatCollection self) => JsonConvert.SerializeObject(self, CollectionConverter.Settings);
    }

    /// <summary>
    /// CollectionConverter
    /// </summary>
    internal static class CollectionConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                JsonStatCollectionClassConverter.Singleton,
                IndexConverter.Singleton,
                PositionConverter.Singleton,
                ItemClassConverter.Singleton,
                StatusConverter.Singleton,
                ItemValueConverter.Singleton,
                ValueElementConverter.Singleton,
                VersionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    /// <summary>
    /// JsonStatCollectionClassConverter
    /// </summary>
    internal class JsonStatCollectionClassConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(JsonStatCollectionClass) || t == typeof(JsonStatCollectionClass?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "collection")
            {
                return JsonStatCollectionClass.Collection;
            }
            throw new Exception("Cannot unmarshal type JsonStatCollectionClass");
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
            var value = (JsonStatCollectionClass)untypedValue;
            if (value == JsonStatCollectionClass.Collection)
            {
                serializer.Serialize(writer, "collection");
                return;
            }
            throw new Exception("Cannot marshal type JsonStatCollectionClass");
        }

        public static readonly JsonStatCollectionClassConverter Singleton = new JsonStatCollectionClassConverter();
    }


    /// <summary>
    /// ItemClassConverter
    /// </summary>
    internal class ItemClassConverter : JsonConverter
    {
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool CanConvert(Type t) => t == typeof(ItemClass) || t == typeof(ItemClass?);

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
                case "collection":
                    return ItemClass.Collection;
                case "dataset":
                    return ItemClass.Dataset;
                case "dimension":
                    return ItemClass.Dimension;
            }
            throw new Exception("Cannot unmarshal type ItemClass");
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
            var value = (ItemClass)untypedValue;
            switch (value)
            {
                case ItemClass.Collection:
                    serializer.Serialize(writer, "collection");
                    return;
                case ItemClass.Dataset:
                    serializer.Serialize(writer, "dataset");
                    return;
                case ItemClass.Dimension:
                    serializer.Serialize(writer, "dimension");
                    return;
            }
            throw new Exception("Cannot marshal type ItemClass");
        }

        public static readonly ItemClassConverter Singleton = new ItemClassConverter();
    }


    /// <summary>
    /// ItemValueConverter
    /// </summary>
    internal class ItemValueConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ItemValue) || t == typeof(ItemValue?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<Dictionary<string, ValueElement>>(reader);
                    return new ItemValue { AnythingMap = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<ValueElement>>(reader);
                    return new ItemValue { AnythingArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ItemValue");
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="untypedValue"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ItemValue)untypedValue;
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
            throw new Exception("Cannot marshal type ItemValue");
        }

        public static readonly ItemValueConverter Singleton = new ItemValueConverter();
    }



}
