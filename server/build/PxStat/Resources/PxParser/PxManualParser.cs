using API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PxParser.Resources.Parser
{
    public class PxManualParser : IPxManualParser
    {
        private const string LEFT_SQUARE_BRACKET = "[";
        private const string RIGHT_SQUARE_BRACKET = "]";
        private const string LEFT_PARENTHESES = "(";
        private const string RIGHT_PARENTHESES = ")";
        private const string EQUALS = "=";
        private const string CONFIDENTIAL = "CONFIDENTIAL";
        private const string DEFAULT_GRAPH = "DEFAULT-GRAPH";
        private const string ATTRIBUTE_ID = "ATTRIBUTE-ID";
        private const string ATTRIBUTE_TEXT = "ATTRIBUTE-TEXT";
        private const string ATTRIBUTES = "ATTRIBUTES";
        private const string CODES = "CODES";
        private const string DATA = "DATA";
        private const string HIERARCHYLEVELS = "HIERARCHYLEVELS";
        private const string HIERARCHYLEVELSOPEN = "HIERARCHYLEVELSOPEN";
        private const string LANGUAGES = "LANGUAGES";
        private const string DECIMALS = "DECIMALS";
        private const string SHOWDECIMALS = "SHOWDECIMALS";
        private const string PRECISION = "PRECISION";
        private const string PRESTEXT = "PRESTEXT";
        private const string STUB = "STUB";
        private const string HEADING = "HEADING";
        private const string PARTITIONED = "PARTITIONED";
        private const string ROUNDING = "ROUNDING";
        private const string TIMEVAL = "TIMEVAL";
        private const string VALUES = "VALUES";
        private const string NOTE = "NOTE";
        private const string NOTEX = "NOTEX";
        private const string BASEPERIOD = "BASEPERIOD";
        private const string CONTACT = "CONTACT";
        private const string CELLNOTE = "CELLNOTE";
        private const string CELLNOTEX = "CELLNOTEX";
        private const string DATANOTE = "DATANOTE";
        private const string REFPERIOD = "REFPERIOD";
        private const string VALUENOTE = "VALUENOTE";
        private const string VALUENOTEX = "VALUENOTEX";

        private string Input { get; set; }

        public PxManualParser(string input)
        {
            Input = input;
        }

        public PxDocument Parse()
        {
            return Parse(Input);
        }

        /// <summary>
        /// Pre process the input string. This method removes the DATA property value from the
        /// input string before processing. This pre-process is to make sure that all the line
        /// endings in the input string are terminated with \r\n. It protects against
        /// name value pairs in the input string written over multiple lines.
        /// 
        /// Note - the HTML sanitizer converts \r\n to \n by default
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string PreProcessInput(string input)
        {
            string hexString = "";
            if (input.Contains(DATA + EQUALS))
            {
                string processString = input.Split(new string[] { DATA + EQUALS }, StringSplitOptions.None)[0];
                hexString = ConvertToHexUsingRegex(processString);
                return ConvertHexStringToStringWithRegex(hexString) + DATA + EQUALS + input.Split(new string[] { DATA + EQUALS }, StringSplitOptions.None)[1];
            }
            else
            {
                hexString = ConvertToHexUsingRegex(input);
                return ConvertHexStringToStringWithRegex(hexString);
            }
        }


        public string ConvertToHexUsingRegex(string input)
        {
            StringBuilder sb = new StringBuilder();
            string output = input;
            string line;
            using (StringReader lineReader = new StringReader(input))
            {

                while ((line = lineReader.ReadLine()) != null)
                {
                    
                    Regex regex = new Regex("\"[^\"]+\"");
                    MatchCollection mc = regex.Matches(line);
                    foreach (var s in mc)
                    {
                        string sNoCommmas = s.ToString();
                        line = line.Replace(sNoCommmas,'"' + ConvertStringToHexString(sNoCommmas) + '"');
                        
                    }
                    sb.AppendLine(line);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts str string to a string of HEX
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string ConvertStringToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }


        private string ConvertHexStringToStringWithRegex(string input)
        {
            StringBuilder sb = new StringBuilder();
            string output = input;
            string line;
            using (StringReader lineReader = new StringReader(input))
            {
                
                while ((line = lineReader.ReadLine()) != null)
                {
                    Regex regex = new Regex("\"[^\"]+\"");
                    MatchCollection mc= regex.Matches(line);
                    foreach(var s in mc)
                    {
                        string sNoCommas = s.ToString().Replace("\"", "");
                        if (!String.IsNullOrEmpty(sNoCommas))
                        {
                            var conversion = ConvertHexStringToString(sNoCommas).Replace("\"", ""); 
                            line = line.Replace(sNoCommas, conversion);
                        }
                            
                    }
                    sb.AppendLine(line);
                }
            }
            return sb.ToString() ;
        } 

        /// <summary>
        /// Converts temp HEX string to a string
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private static string ConvertHexStringToString(string temp)
        {
            try
            {
                // Convert hex string contained in inverted commas to string
                var bytes = new byte[temp.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(temp.Substring(i * 2, 2), 16);
                }

                string result = Encoding.Unicode.GetString(bytes);
                return result;
            }
            catch (Exception ex)
            {
                var badLine = temp;
                throw ex;
            }
        }

        public PxDocument Parse(string input)
        {
            input = PreProcessInput(input);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Log.Instance.Debug("Start parsing");
            Input = input;
            List<string> keywords = new List<string>();
            List<string> values = new List<string>();
            IList<IPxKeywordElement> pxKeywordElements = new List<IPxKeywordElement>();

            if (Input.Length == 0)
            {
                Log.Instance.Debug("PxDocument is blank");
                return new PxDocument(pxKeywordElements);
            }

            // Split Input string using semicolon and linefeed carriage return characters to parse each line
            List<string> inputList = Input.Split(new string[] { ";\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string keywordAndValue in inputList)
            {
                string keyword;
                string value;


                if (String.IsNullOrEmpty(keywordAndValue))
                {
                    break;
                }

                // Special case i.e. UNITS("Industrial Production Index (Base 2015=100)")="Base 2015=100"
                if (keywordAndValue.Contains(RIGHT_PARENTHESES + EQUALS))
                {
                    int index = keywordAndValue.IndexOf(RIGHT_PARENTHESES + EQUALS);
                    keyword = keywordAndValue.Substring(0, index + 1);
                    var length = keywordAndValue.Length - (index + 2);
                    value = keywordAndValue.Substring(index + 2, length);
                }
                else
                {
                    // Split and return first result as keyword
                    // This is needed because there could be an equals character in the value
                    keyword = keywordAndValue.Split('=')[0];

                    // Get the substring from the equals character to the end of the keywordAndValue
                    // string as value
                    value = keywordAndValue.Substring(keywordAndValue.IndexOf('=') + 1);
                }

                if (!String.IsNullOrEmpty(keyword))
                {
                    keywords.Add(keyword);
                    values.Add(value);
                }
            }

            for (int i = 0; i < keywords.Count; i++)
            {
                
                IPxKeywordElement keywordElement = ProcessKeywordElement(keywords[i], values[i]);
                pxKeywordElements.Add(keywordElement);
            }

            PxDocument pxDocument = new PxDocument(pxKeywordElements);
            Log.Instance.Debug("Finished parsing");
            sw.Stop();
            Log.Instance.Debug("Parsing completed in " + (int)sw.ElapsedMilliseconds + " ms");
            return pxDocument;
        }

        public IPxKeywordElement ProcessKeywordElement(string keyword, string value)
        {
            string key = keyword;
            string language = null;
            string subKey = null;
            bool islanguageFound = false;
            if (keyword.Contains(LEFT_SQUARE_BRACKET) && keyword.Contains(RIGHT_SQUARE_BRACKET))
            {
                var start = keyword.IndexOf(LEFT_SQUARE_BRACKET) + 1;
                var end = keyword.IndexOf(RIGHT_SQUARE_BRACKET);
                key = keyword.Substring(0, keyword.IndexOf(LEFT_SQUARE_BRACKET));
                language = keyword.Substring(start, end - start);
                islanguageFound = true;
            }

            if (keyword.Contains(LEFT_PARENTHESES) && keyword.Contains(RIGHT_PARENTHESES))
            {
                var start = keyword.IndexOf(LEFT_PARENTHESES) + 1;
                var end = keyword.LastIndexOf(RIGHT_PARENTHESES);

                // Check if keyword contained language
                if (!islanguageFound)
                {
                    key = keyword.Substring(0, keyword.IndexOf(LEFT_PARENTHESES));
                }
                subKey = keyword.Substring(start, end - start);
            }
            key = RemoveCarriageReturnsAndLineFeeds(key);

            switch (key)
            {
                case DATA:
                    IList<IPxSingleElement> cells = new List<IPxSingleElement>();
                  
                    //List<dynamic> dataRows = value.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<dynamic>();
                    // Change the split to just \n and remove the \r in the next stage
                    //the Html sanitizer has already removed the \r 
                    List<dynamic> dataRows = value.Split(new string[] { "\n" }, StringSplitOptions.None).ToList<dynamic>();
                    foreach (string data in dataRows)
                    {
                        if (String.IsNullOrEmpty(data) || data.Equals(";"))
                        {
                            continue;
                        }
                        if (data.Contains(","))
                        {
                            ProcessKeys(cells, data);
                        }
                        else
                        {
                            ProcessTail(cells, data);
                        }
                    }
                    return new PxKeywordElement(new PxKey(key, language), new PxDataValue(new List<IPxSingleElement>(cells)));
                case DECIMALS:
                case CONFIDENTIAL:
                case DEFAULT_GRAPH:
                case HIERARCHYLEVELS:
                case HIERARCHYLEVELSOPEN:
                case PRESTEXT:
                case ROUNDING:
                case SHOWDECIMALS:
                    short decimals;
                    short.TryParse(value, out decimals);
                    if (subKey != null)
                    {
                        return new PxKeywordElement(new PxKey(key, language, new PxSubKey(subKey.Replace("\"", ""))), new PxLiteralValue(decimals));
                    }
                    else
                    {
                        return new PxKeywordElement(new PxKey(key, language), new PxLiteralValue(decimals));
                    }
                case ATTRIBUTE_ID:
                case ATTRIBUTE_TEXT:
                case ATTRIBUTES:
                case LANGUAGES:
                case CODES:
                case TIMEVAL:
                case HEADING:
                case STUB:
                case PARTITIONED:
                case VALUES:
                    IList<IPxSingleElement> elements = new List<IPxSingleElement>();
                    value = RemoveCarriageReturnsAndLineFeeds(value);

                    // If the first element is not a string i.e. TLIST(M1)
                    if (!value.ElementAt(0).Equals('\"'))
                    {
                        var firstItem = value.Split(',')[0];

                        // Remove firstItem from start of value
                        value = value.Replace(firstItem + ",", "");
                        elements.Add(new PxStringValue(firstItem));
                    }

                    // Separate the values with ","
                    string[] separator = { "\",\"" };
                    var items = value.Split(separator, StringSplitOptions.None).ToList();
                    string temp;
                    foreach (string item in items)
                    {
                        temp = item;
                        // Remove all inverted commas from item
                        temp = temp.Replace("\"", "");
                        elements.Add(new PxStringValue(temp, false));
                    }
                    if (subKey != null)
                    {
                        return new PxKeywordElement(new PxKey(key, language, new PxSubKey(subKey.Replace("\"", ""))), new PxListOfValues(elements));
                    }
                    else
                    {
                        return new PxKeywordElement(new PxKey(key, language), new PxListOfValues(elements));
                    }
                case NOTE:
                case NOTEX:
                case BASEPERIOD:
                case CONTACT:
                case CELLNOTE:
                case CELLNOTEX:
                case DATANOTE:
                case REFPERIOD:
                case VALUENOTE:
                case VALUENOTEX:
                    // Remove all inverted commas from value
                    value = value.Replace("\"", "");
                    List<string> multiline = value.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                    if (subKey != null)
                    {
                        return new PxKeywordElement(new PxKey(key, language, new PxSubKey(subKey.Replace("\"", ""))), new PxValueMultiline(multiline));
                    }
                    else
                    {
                        return new PxKeywordElement(new PxKey(key, language), new PxValueMultiline(multiline));
                    }

                case PRECISION:
                    short dcmls;
                    short.TryParse(value, out dcmls);
                    if (subKey != null)
                    {
                        // Subkey has both key and value separated by a comma
                        if (subKey.Contains(','))
                        {
                            string[] s = { "\",\"" };

                            List<string> keyList = subKey.Split(s, StringSplitOptions.None).ToList();
                            keyList = keyList.Select(k => k.Replace("\"", "")).ToList();

                            return new PxKeywordElement(new PxKey(key, language, new PxSubKey(keyList[0], keyList[1])), new PxLiteralValue(dcmls));
                        }
                        else
                            return new PxKeywordElement(new PxKey(key, language, new PxSubKey(subKey)), new PxLiteralValue(dcmls));
                    }
                    else
                    {
                        return new PxKeywordElement(new PxKey(key, language), new PxLiteralValue(dcmls));
                    }
                default:
                    // Remove leading and trailing inverted commas from value
                    value = value.TrimStart('\"').TrimEnd('\"');
                    if (subKey != null)
                    {
                        return new PxKeywordElement(new PxKey(key, language, new PxSubKey(subKey.Replace("\"", ""))), new PxStringValue(value));
                    }
                    else
                    {
                        return new PxKeywordElement(new PxKey(key, language), new PxStringValue(value));
                    }
            }
        }

        /// <summary>
        /// If keys are present in the data they need to be processed
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        private void ProcessKeys(IList<IPxSingleElement> elements, string data)
        {
            if (data.LastIndexOf("\",") != -1)
            {
                var keys = data.Substring(0, data.LastIndexOf("\",") + 1);
                // Parse data to remove keys
                data = data.Substring(keys.Length + 1, data.Length - keys.Length - 1);
                var filteredKeys = keys.Split(',').ToList();
                foreach (string k in filteredKeys)
                {
                    string trimedString = k.Trim();
                    elements.Add(new PxStringValue(trimedString));
                }
            }
            ProcessTail(elements, data);
        }

        /// <summary>
        /// Replaces blanks in string with semicolons
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ReplaceBlanksInStrings(string data)
        {
            bool foundInvertedComma = false;
            bool first = false;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char ch in data)
            {
                if (ch.Equals('"'))
                {
                    foundInvertedComma = true;
                    first = !first;
                }

                if (foundInvertedComma && ch.Equals(' ') && first)
                {
                    stringBuilder.Append(";");
                }
                else
                {
                    stringBuilder.Append(ch);
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Process the data
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        private void ProcessTail(IList<IPxSingleElement> elements, string data)
        {

            // Remove semicolon line feeds and carriage returns from data, if present
            data = data.Replace(";", "").Replace("\r", "").Replace("\n", "");



            // Check for strings in data
            if (data.Contains("\""))
            {
                data = ReplaceBlanksInStrings(data);
            }

            //We need to treat spaces and non-breaking spaces the same, so we replace any space with a standard space
            Regex rx = new Regex("[\\s]");
            data = rx.Replace(data, " ");
            var filteredData = data.Split(' ').ToList();

            foreach (string d in filteredData)
            {
                if (String.IsNullOrEmpty(d))
                {
                    continue;
                }
                double number;
                if (double.TryParse(d, out number))
                {
                    elements.Add(new PxDoubleValue(number));
                }
                else
                {
                    //change  nok - string may already be in quotes
                    // Dot values need to be in a quoted string
                    //(a) Dot (and any other string) values may be already in quotes
                    //(b) dot values aren't the only things that could be in quotes
                    if (!d.Contains("\""))
                    {
                        elements.Add(new PxQuotedValue(d));
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(d))
                        {
                            elements.Add(new PxStringValue(d.Replace("\"", "").Replace(";", " ")));
                        }
                    }
                }
            }
        }

        public static string RemoveWhitespaces(string input)
        {
            return new string(input.ToCharArray()
                       .Where(c => !Char.IsWhiteSpace(c))
                       .ToArray());
        }

        public static string RemoveCarriageReturnsAndLineFeeds(string input)
        {
            return input.Replace("\r", "").Replace("\n", "");
        }
    }
}
