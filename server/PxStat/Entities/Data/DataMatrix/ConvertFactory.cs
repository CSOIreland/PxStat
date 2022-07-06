using PxParser.Resources.Parser;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PxStat.Data
{

    public static class ConvertFactory
    {
        /// <summary>
        /// Converts a list of IPxSingleElement based objects to a collection of string based objects
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ICollection<KeyValuePair<string, ICollection<string>>> Convert(IList<KeyValuePair<string, IList<IPxSingleElement>>> value)
        {
            ICollection<KeyValuePair<string, ICollection<string>>> result = new List<KeyValuePair<string, ICollection<string>>>();
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

        public static IList<KeyValuePair<string, IList<IPxSingleElement>>> Convert(ICollection<KeyValuePair<string, ICollection<string>>> value)
        {
            IList<KeyValuePair<string, IList<IPxSingleElement>>> result = new List<KeyValuePair<string, IList<IPxSingleElement>>>();
            foreach (var keyValues in value)
            {
                IList<IPxSingleElement> elements = new List<IPxSingleElement>();
                foreach (var item in keyValues.Value)
                {
                    elements.Add(new PxStringValue(item));
                }
                KeyValuePair<string, IList<IPxSingleElement>> keyValuePair = new KeyValuePair<string, IList<IPxSingleElement>>(keyValues.Key, elements);
                result.Add(keyValuePair);
            }
            return result;
        }

        public static List<KeyValuePair<string, string>> Convert(List<SqlBulkCopyColumnMapping> value)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            foreach (var item in value)
            {
                KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(item.SourceColumn, item.DestinationColumn);
                result.Add((keyValuePair));
            }
            return result;
        }
    }
}
