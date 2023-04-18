using System;
using PxParser.Resources.Parser;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using PxStat.Security;

namespace PxStat.Data
{

    public static class ConvertFactory
    {
        /// <summary>
        /// Converts a list of IPxSingleElement based objects to a collection of string based objects
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ICollection<KeyValuePair<string, ICollection<string>>> Convert(
            IList<KeyValuePair<string, IList<IPxSingleElement>>> value)
        {
            ICollection<KeyValuePair<string, ICollection<string>>> result =
                new List<KeyValuePair<string, ICollection<string>>>();

            if (value == null) {
                return result;
            }

            foreach (var item in value)
            {
                IList<IPxSingleElement> elements = item.Value;
                ICollection<string> strings = new List<string>();
                foreach (var element in elements)
                {
                    strings.Add(element.ToPxString().ToString());
                }

                result.Add(new KeyValuePair<string, ICollection<string>>(item.Key, strings));
            }

            return result;
        }

        public static IList<KeyValuePair<string, IList<IPxSingleElement>>> Convert(
            ICollection<KeyValuePair<string, ICollection<string>>> value)
        {
            IList<KeyValuePair<string, IList<IPxSingleElement>>> result =
                new List<KeyValuePair<string, IList<IPxSingleElement>>>();
            foreach (var keyValues in value)
            {
                IList<IPxSingleElement> elements = new List<IPxSingleElement>();
                foreach (var item in keyValues.Value)
                {
                    elements.Add(new PxStringValue(item));
                }

                KeyValuePair<string, IList<IPxSingleElement>> keyValuePair =
                    new KeyValuePair<string, IList<IPxSingleElement>>(keyValues.Key, elements);
                result.Add(keyValuePair);
            }

            return result;
        }

        public static List<KeyValuePair<string, string>> Convert(List<SqlBulkCopyColumnMapping> value)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            foreach (var item in value)
            {
                KeyValuePair<string, string> keyValuePair =
                    new KeyValuePair<string, string>(item.SourceColumn, item.DestinationColumn);
                result.Add((keyValuePair));
            }

            return result;
        }

        /// <summary>
        /// Get the values from the dimensions with role CLASSIFICATION and TIME and comma separate them in a string
        /// </summary>
        /// <param name="title"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        public static string GetDimensionValues(string title, ICollection<StatDimension> dimensions, IMetaData metaData,string lngIsoCode=null)
        {
            if (dimensions.Count == 0)
            {
                return "";
            }

            if (lngIsoCode == null)
                lngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

            List<string> dims = new List<string>();
            List<string> times = new List<string>();
            string time = "";
            foreach (var dim in dimensions)
            {
                if (dim.Role.Equals("CLASSIFICATION"))
                {
                    dims.Add(dim.Value);
                }
                else if (dim.Role.Equals("TIME"))
                {
                    time = dim.Value;
                    foreach (var t in dim.Variables)
                    {
                        times.Add(t.Value);
                    }
                }
            }

            if (!string.IsNullOrEmpty(time))
            {
                // Add dimension with time role to the end of the dims array
                dims.Add(time);
            }

            StringBuilder sb = new StringBuilder();
            string separator = " " + metaData.GetTitleBy() + " ";

            if (dims.Count == 1)
            {
                return separator + dims[0];
            }

            for (var i = 0; i < dims.Count; i++)
            {
                if (i != dims.Count - 1)
                {
                    // Do not include the comma if the item is second last key in the list
                    if (i != dims.Count - 2)
                    {
                        sb.Append(dims[i] + ", ");
                    }
                    else
                    {
                        sb.Append(dims[i] + " ");
                    }
                }
                else
                {
                    sb.Append(Label.Get("label.and",lngIsoCode) + " " + dims[i]);
                }
            }

            // If the title is not blank and is included in the string builder return a blank string
            if (!string.IsNullOrEmpty(title) && sb.ToString().Contains(title))
            {
                return "";
            }

            string value = "";
            if (times.Count > 0)
            {
                // Get time ranges and output the first and last range by default
                switch (times.Count)
                {
                    case 0: value = "";
                        break;

                    case 1: value = times[0]; 
                        break;

                    default: value = times[0] + separator + times[times.Count - 1];
                        break;
                }
            }
            return times.Count == 0 ? separator + sb.ToString() : separator + sb.ToString() + " " + value;
        }
    }
}
