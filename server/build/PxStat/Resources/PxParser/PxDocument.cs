using PxStat;
using PxStat.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PxParser.Resources.Parser
{
    /// <summary>
    /// The PxDocument is composed by a sequence of keyword objects (minimum of one)
    /// <PxDocument> ::= <PxKeywordElement> { <PxKeywordElement> }
    /// </summary>
    public class PxDocument : IPx, IDocument
    {
        /// <summary>
        /// 
        /// The Keywords that compose the Px File
        /// 
        /// Includes all the Metadata and Data Keywords
        /// 
        /// </summary>
        public IList<IPxKeywordElement> Keywords { get; }

        /// <summary>
        /// Constructs a PxDocument from a IList of PxKeywordElement objects.
        /// </summary>
        /// <param name="keywords"></param>
        public PxDocument(IList<IPxKeywordElement> keywords)
        {
            Keywords = keywords;
        }

        /// <summary>
        /// 
        /// Gets the string value .ToPxString() for each keyword object in the file, separated and terminated with ;\r\n
        /// 
        /// </summary>
        /// <returns>A string with all the textual Px representation of the file in a Px Format</returns>
        public string ToPxString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var k in Keywords)
            {
                sb.AppendFormat("{0};\r\n", k.ToPxString());
            }
            return sb.ToString();
        }


        /// <summary>
        /// Overrides the base class ToString to return the actual Px Representation of the document.
        /// 
        /// Can and should be used to save the correspondent document object to a valid Px File!
        /// 
        /// </summary>
        /// <returns>A string containing a valid representation as a Px File</returns>
        public override string ToString()
        {
            return ToPxString();
        }


        /// <summary>
        /// Gets the string value of the Keyword element that matches the keyword identifier.
        /// 
        /// Potential Exception if there is no keyword that matches the supplied identifier.
        /// 
        /// Please <seealso cref="GetStringValueIfExist"/>
        /// </summary>
        /// <param name="keywordIdentifier">The identifier of the Keyword</param>
        /// <returns>A string with the value of the matched keyword</returns>
        public string GetStringElementValue(string keywordIdentifier, string language = null)
        {
            var keys = GetAllKeywordElementsThatMatchTheIdentifier(this, keywordIdentifier, language);
            var firstKey = keys.First();
            var firstKeyElement = firstKey.Element;

            return firstKeyElement.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywordIdentifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public short GetShortElementValue(string keywordIdentifier, string language = null)
        {
            var keys = GetAllKeywordElementsThatMatchTheIdentifier(this, keywordIdentifier, language);
            var firstKey = keys.First();
            var firstKeyElement = firstKey.Element;
            return (short)firstKeyElement.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywordIdentifier"></param>
        /// <param name="language"></param>
        /// <param name="aShortNumber"></param>
        /// <returns></returns>
        public bool GetShortElementValueIfExist(string keywordIdentifier, string language, ref short aShortNumber)
        {
            var keys = Keywords.Where(k => k.Key.Identifier == keywordIdentifier && (String.IsNullOrEmpty(language) || k.Key.Language == language));

            if (!keys.Any())
            {
                return false;
            }

            aShortNumber = GetShortElementValue(keywordIdentifier, language);

            return true;
        }

        /// <summary>
        /// Gets the string value of the Keyword element that matches the keyword identifier.
        /// 
        /// Validates the given keyword exists.
        /// 
        /// Please <seealso cref="GetStringElementValue"/>
        /// </summary>
        /// <param name="keywordIdentifier">The identifier of the Keyword</param>
        /// <returns>A string with the value of the matched keyword, or null in case no keyword is found.</returns>
        public string GetStringValueIfExist(string keywordIdentifier, string language = null)
        {
            var keys = Keywords.Where(k => k.Key.Identifier == keywordIdentifier && (String.IsNullOrEmpty(language) || k.Key.Language == language));

            if (!keys.Any())
            {
                return null;
            }

            return GetStringElementValue(keywordIdentifier, language);
        }

        /// <summary>
        /// Return a list of IPxSingleElements that match the supplied keywordIdentifier
        /// </summary>
        /// <param name="keywordIdentifier">The keyword identifier to match</param>
        /// <returns>The list of objects that implement IPxSingleElement interface</returns>
        public IList<IPxSingleElement> GetListOfElements(string keywordIdentifier, string language = null)
        {
            IPxMultipleElements multipleElements = GetPxMultipleElements(keywordIdentifier, language);

            IList<IPxSingleElement> listOfSingleElements = CastToListOfPxSingleElements(multipleElements);

            return listOfSingleElements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywordIdentifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<string> GetListOfStringValues(string keywordIdentifier, string language = null)
        {
            IPxMultipleElements multipleElements = GetPxMultipleElements(keywordIdentifier, language);

            var enumerator = multipleElements.Values.Cast<string>();
            IList<string> listOfStrings = multipleElements.ToList();
            return listOfStrings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywordIdentifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<IPxSingleElement> GetListOfElementsIfExist(string keywordIdentifier, string language = null)
        {
            var keys = Keywords.Where(k => k.Key.Identifier == keywordIdentifier && (String.IsNullOrEmpty(language) || k.Key.Language == language));

            if (!keys.Any())
            {
                return null;
            }

            return GetListOfElements(keywordIdentifier, language);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywordIdentifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<string> GetListOfStringValuesIfExist(string keywordIdentifier, string language = null)
        {
            var keys = Keywords.Where(k => k.Key.Identifier == keywordIdentifier && (String.IsNullOrEmpty(language) || k.Key.Language == language));

            if (!keys.Any())
            {
                return null;
            }

            return GetListOfStringValues(keywordIdentifier, language);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywordIdentifier"></param>
        /// <returns></returns>
        public IList<dynamic> GetData(string keywordIdentifier)
        {
            IPxElement elem = GetAllKeywordElementsThatMatchTheIdentifier(this, keywordIdentifier).First().Element;

            PxDataValue lv = (PxDataValue)elem;

            IPxMultipleElements elems = lv;

            return elems.Values;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<KeyValuePair<string, IList<IPxSingleElement>>> GetSingleElementWithSubkeys(string identifier, string language = null)
        {
            IList<KeyValuePair<string, IList<IPxSingleElement>>> r = new List<KeyValuePair<string, IList<IPxSingleElement>>>();

            foreach (var k in GetAllKeywordElementsThatMatchTheIdentifier(this, identifier, language))
            {
                IList<IPxSingleElement> list = new List<IPxSingleElement>();
                list.Add((IPxSingleElement)k.Element);
                if (k.Key.SubKey != null)
                {
                    var kv = new KeyValuePair<string, IList<IPxSingleElement>>((k.Key.SubKey.Value ?? k.Key.SubKey.Name), list);
                    r.Add(kv);
                }
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<KeyValuePair<string, IList<IPxSingleElement>>> GetSingleElementWithSubkeysIfExist(string identifier, string language = null)
        {
            var keys = Keywords.Where(k => k.Key.Identifier == identifier && (String.IsNullOrEmpty(language) || k.Key.Language == language));

            if (!keys.Any())
            {
                return null;
            }

            return GetSingleElementWithSubkeys(identifier);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeysIfExist(string identifier, string language = null)
        {
            var keys = Keywords.Where(k => k.Key.Identifier == identifier && (String.IsNullOrEmpty(language) || k.Key.Language == language));

            if (!keys.Any())
            {
                return null;
            }

            return GetMultiValuesWithSubkeys(identifier, language);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeys(string identifier, string language = null)
        {
            IList<KeyValuePair<string, IList<IPxSingleElement>>> r = new List<KeyValuePair<string, IList<IPxSingleElement>>>();

            foreach (var k in GetAllKeywordElementsThatMatchTheIdentifier(this, identifier, language))
            {
                var list = CastToListOfPxSingleElements((IPxMultipleElements)k.Element);
                var kv = new KeyValuePair<string, IList<IPxSingleElement>>(k.Key.SubKey.Name, list);
                r.Add(kv);
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeysOnlyIfLanguageMatches(string identifier, string language = null)
        {
            IList<KeyValuePair<string, IList<IPxSingleElement>>> r = new List<KeyValuePair<string, IList<IPxSingleElement>>>();

            foreach (var k in GetAllKeywordElementsThatMatchTheIdentifierStrictly(this, identifier, language))
            {
                var list = CastToListOfPxSingleElements((IPxMultipleElements)k.Element);
                var kv = new KeyValuePair<string, IList<IPxSingleElement>>(k.Key.SubKey.Name, list);
                //The newline character must not end up in the SingleValue:
                foreach (var vl in kv.Value)
                {
                    vl.SingleValue = vl.SingleValue.Replace("\r\n", string.Empty);
                }
                r.Add(kv);
            }

            return r;
        }


        public IList<KeyValuePair<string, IPxSingleElement>> GetManySingleValuesWithSubkeysOnlyIfLanguageMatches(string identifier, string language = null)
        {
            IList<KeyValuePair<string, IPxSingleElement>> r = new List<KeyValuePair<string, IPxSingleElement>>();

            foreach (var k in GetAllKeywordElementsThatMatchTheIdentifierOptionally(this, identifier, language))
            {

                var kv = new KeyValuePair<string, IPxSingleElement>(k.Key.SubKey.Name, (IPxSingleElement)k.Element);
                r.Add(kv);
            }
            return r;
        }


        public IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeysOnlyIfLanguageIsEmpty(string identifier)
        {
            IList<KeyValuePair<string, IList<IPxSingleElement>>> r = new List<KeyValuePair<string, IList<IPxSingleElement>>>();

            foreach (var k in GetAllKeywordElementsThatMatchTheIdentifierLanguageEmpty(this, identifier))
            {
                var list = CastToListOfPxSingleElements((IPxMultipleElements)k.Element);
                var kv = new KeyValuePair<string, IList<IPxSingleElement>>(k.Key.SubKey.Name, list);
                r.Add(kv);
            }

            return r;
        }

        protected IEnumerable<IPxKeywordElement> GetAllKeywordElementsThatMatchTheIdentifierLanguageEmpty(PxDocument pxDoc, string keywordIdentifier, string language = null)
        {
            var keys = pxDoc.Keywords.Where(k => k.Key.Identifier == keywordIdentifier && (String.IsNullOrEmpty(k.Key.Language)));


            if (!keys.Any())
            {
                throw new FormatException(string.Format(Label.Get("px.schema.key-not-found"), keywordIdentifier, language));
            }
            return keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeysOnlyIfLanguageMatchesNullable(string identifier, string language = null)
        {
            IList<KeyValuePair<string, IList<IPxSingleElement>>> r = new List<KeyValuePair<string, IList<IPxSingleElement>>>();

            var elements = GetAllKeywordElementsThatMatchTheIdentifierNulllable(this, identifier, language);

            if (elements == null) return null;

            foreach (var k in elements)
            {
                //IList<IPxSingleElement> list = new List<IPxSingleElement>();
                var list = CastToListOfPxSingleElements((IPxMultipleElements)k.Element);
                var kv = new KeyValuePair<string, IList<IPxSingleElement>>(k.Key.SubKey.Name, list);
                r.Add(kv);
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elems"></param>
        /// <returns></returns>
        protected static IList<IPxSingleElement> CastToListOfPxSingleElements(IPxMultipleElements elems)
        {
            var enumerator = elems.Values.Cast<IPxSingleElement>();

            IList<IPxSingleElement> s = enumerator.ToList<IPxSingleElement>();

            foreach (var v in s)
            {
                v.SingleValue = v.SingleValue.Replace("\"\r\n\"", "");
                v.SingleValue = v.SingleValue.Replace("\"", "");
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywordIdentifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected IPxMultipleElements GetPxMultipleElements(string keywordIdentifier, string language = null)
        {
            var keywordElement = GetAllKeywordElementsThatMatchTheIdentifier(this, keywordIdentifier, language).First();
            var multipleElements = (IPxMultipleElements)keywordElement.Element;
            return multipleElements;
        }

        /// <summary>
        /// Returns all the Keyword elements that match the given keyword identifier in the PxDocument.
        /// Throws exception if no keyword if found.
        /// </summary>
        /// <param name="pxDoc">The Px Document to search for the given keyword identifier</param>
        /// <param name="keywordIdentifier">The identifier of the keyword to find</param>
        /// <returns>A <typeparamref name="IEnumerable"/> with all the matched <typeparamref name="IPxKeywordElement"/>elements</returns>
        protected IEnumerable<IPxKeywordElement> GetAllKeywordElementsThatMatchTheIdentifier(PxDocument pxDoc, string keywordIdentifier, string language = null)
        {

            var keys = pxDoc.Keywords.Where(k => k.Key.Identifier == keywordIdentifier && (String.IsNullOrEmpty(language) || k.Key.Language == language));
            if (!keys.Any())
            {
                throw new FormatException(string.Format(Label.Get("px.schema.key-not-found"), keywordIdentifier, language));
            }
            return keys;
        }

        /// <summary>
        /// Similar to GetAllKeywordElementsThatMatchTheIdentifier but will not return values with a language subkey where no language parameter is supplied
        /// </summary>
        /// <param name="pxDoc"></param>
        /// <param name="keywordIdentifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected IEnumerable<IPxKeywordElement> GetAllKeywordElementsThatMatchTheIdentifierStrictly(PxDocument pxDoc, string keywordIdentifier, string language = null)
        {
            var keys = pxDoc.Keywords.Where(k => k.Key.Identifier == keywordIdentifier && ((String.IsNullOrEmpty(language) && String.IsNullOrEmpty(k.Key.Language)) || k.Key.Language == language));


            if (!keys.Any())
            {
                throw new FormatException(string.Format(Label.Get("px.schema.key-not-found"), keywordIdentifier, language));
            }
            return keys;
        }

        protected IEnumerable<IPxKeywordElement> GetAllKeywordElementsThatMatchTheIdentifierOptionally(PxDocument pxDoc, string keywordIdentifier, string language = null)
        {
            var keys = pxDoc.Keywords.Where(k => k.Key.Identifier == keywordIdentifier && ((String.IsNullOrEmpty(language) && String.IsNullOrEmpty(k.Key.Language)) || k.Key.Language == language));


            if (!keys.Any())
            {
                return new List<PxKeywordElement>();
            }
            return keys;
        }

        /// <summary>
        /// Similar to GetAllKeywordElementsThatMatchTheIdentifier but will not return values with a language subkey where no language parameter is supplied
        /// </summary>
        /// <param name="pxDoc"></param>
        /// <param name="keywordIdentifier"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected IEnumerable<IPxKeywordElement> GetAllKeywordElementsThatMatchTheIdentifierNulllable(PxDocument pxDoc, string keywordIdentifier, string language = null)
        {
            var keys = pxDoc.Keywords.Where(k => k.Key.Identifier == keywordIdentifier && ((String.IsNullOrEmpty(language) && String.IsNullOrEmpty(k.Key.Language)) || k.Key.Language == language));

            if (keys == null) return null;

            if (!keys.Any())
            {
                return null;
            }
            return keys;
        }

    }
}
