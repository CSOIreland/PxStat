using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxParser.Resources.Parser
{
    /// <summary>
    /// 
    /// </summary>
    public class PxDotValue : IPxSingleElement, IPxElement
    {
        /// <summary>
        /// 
        /// </summary>
        public PxDotValue()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic SingleValue
        {
            get
            {
                return Configuration_BSO.GetCustomConfig("px.confidential-value");
            }
            set { throw new NotImplementedException(); }

        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                return SingleValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
        {
            return Configuration_BSO.GetCustomConfig("px.confidential-value");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            List<string> returnList = new List<string>();
            returnList.Add(Value.ToString());
            return returnList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator string(PxDotValue x)
        {
            if (x == null)
                throw new NullReferenceException();
            return x.ToPxValue();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PxLiteralValue : IPxSingleElement, IPxElement
    {
        /// <summary>
        /// 
        /// </summary>
        public int LiteralValue { get; set; }

        /// <summary>
        /// SingleValue
        /// </summary>
        public dynamic SingleValue
        {
            get
            {
                return LiteralValue;
            }

            set
            {
                LiteralValue = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                return SingleValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PxLiteralValue(int value)
        {
            LiteralValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
        {
            return LiteralValue.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            return LiteralValue.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            return LiteralValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            List<string> returnList = new List<string>();

            returnList.Add(LiteralValue.ToString());
            return returnList;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator string(PxLiteralValue x)
        {
            if (x == null)
                throw new NullReferenceException();
            return x.ToPxValue();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PxStringValue : IPxSingleElement, IPxElement
    {
        public static implicit operator string(PxStringValue x)
        {
            if (x == null)
                throw new NullReferenceException();
            return x.ToPxValue();
        }

        /// <summary>
        /// 
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public dynamic SingleValue
        {
            get
            {
                return StringValue;
            }
            set
            {
                StringValue = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                return SingleValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PxStringValue(string value)
        {

            // Strip HTLML tags
            StringValue = Regex.Replace(value, @"<.*?>", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
        {
            return StringValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            if (StringValue != null)
            {
                return StringValue;
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            List<string> returnList = new List<string>();
            returnList.Add(Value.ToString());
            return returnList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            throw new NotImplementedException();
        }
    }





    /// <summary>
    /// 
    /// </summary>
    public class PxQuotedValue : IPxSingleElement, IPxElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator string(PxQuotedValue x)
        {
            if (x == null)
                throw new NullReferenceException();
            return x.ToPxValue();
        }

        /// <summary>
        /// 
        /// </summary>
        public string QuotedValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public dynamic SingleValue
        {
            get
            {
                return QuotedValue;
            }
            set
            {
                QuotedValue = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                return SingleValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PxQuotedValue(string value)
        {
            // Strip HTLML tags
            QuotedValue = Regex.Replace(value, @"<.*?>", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            if (QuotedValue != null)
            {
                return QuotedValue;
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
            => $"\"{QuotedValue}\"";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            List<string> returnList = new List<string>();
            returnList.Add(QuotedValue);
            return returnList;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class PxQuotedValueMultiline : IPxSingleElement, IPxElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator string(PxQuotedValueMultiline x)
        {
            if (x == null)
                throw new NullReferenceException();
            return x.ToPxValue();
        }

        /// <summary>
        /// 
        /// </summary>
        public string QuotedValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public dynamic SingleValue
        {
            get
            {
                return QuotedValue;
            }
            set
            {
                QuotedValue = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                return SingleValue;
            }
        }

        dynamic IPxElement.Value
        {
            get
            {
                throw new NotImplementedException();
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PxQuotedValueMultiline(string value)
        {
            // Strip HTLML tags
            QuotedValue = Regex.Replace(value, @"<.*?>", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            if (QuotedValue != null)
            {
                return QuotedValue;
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
            => $"\"{QuotedValue}\"";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            List<string> returnList = new List<string>();
            returnList.Add(QuotedValue);
            return returnList;
        }
    }




    /// <summary>
    /// 
    /// </summary>
    public class PxDoubleValue : IPxSingleElement, IPxElement
    {
        static public implicit operator string(PxDoubleValue x)
        {
            if (x == null)
                throw new NullReferenceException();
            return x.ToPxValue();
        }

        /// <summary>
        /// 
        /// </summary>
        public double DoubleValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PxDoubleValue(double value)
        {
            this.DoubleValue = value;
        }

        public PxDoubleValue(string value)
        {
            double output;
            if (double.TryParse(value, out output))
            {
                this.DoubleValue = output;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic SingleValue
        {
            get
            {
                return DoubleValue;
            }
            set
            {
                DoubleValue = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                return SingleValue;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            return DoubleValue.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            return DoubleValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
        {
            return DoubleValue.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            List<string> returnList = new List<string>();
            returnList.Add(Value.ToString());
            return returnList;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PxListOfValues : IPxMultipleElements, IPxElement
    {
        /// <summary>
        /// 
        /// </summary>
        IList<IPxSingleElement> Elements { get; }

        /// <summary>
        /// 
        /// </summary>
        public IList<dynamic> Values
        {
            get
            {
                return Elements.Cast<dynamic>().ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        public PxListOfValues(IList<IPxSingleElement> elements)
        {
            Elements = elements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        public PxListOfValues(IList<string> elements)
        {

            IList<IPxSingleElement> stringElements = new List<IPxSingleElement>();
            foreach (string str in elements)
            {
                PxStringValue pxe = new PxStringValue(str);
                stringElements.Add(pxe);
            }
            Elements = stringElements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
            => string.Join(",", Values.Select(e => e.ToPxString()));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
            => $"{string.Join(",", Values.Select(e => e.ToPxString()))}";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxQuotedString()
           => string.Join(",", Values.Select(e => '"' + e + '"'));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newlineLengthLimit"></param>
        /// <returns></returns>  
        public string ToPxQuotedString(string key, int newlineLengthLimit)
        {
            List<string> formatList = new List<string>();
            int charCount = key.Length + 1;
            string line = "";
            foreach (var v in Values)
            {
                line = '"' + v.ToPxString() + '"';
                charCount = charCount + line.Length;
                if (charCount > newlineLengthLimit)
                {
                    charCount = 0;
                    line = Environment.NewLine + line;
                }
                formatList.Add(line);
            }
            return string.Join(",", formatList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newlineItemLimit"></param>
        /// <returns></returns>
        public string ToPxDataString(int newlineItemLimit)
        {
            int counter = 0;
            string line = "";
            List<string> formatList = new List<string>();
            foreach (var v in Values)
            {
                line = v.ToPxString();
                if (counter % newlineItemLimit == 0)
                {
                    line = Environment.NewLine + line;
                }
                counter++;
                formatList.Add(line);
            }
            return string.Join(" ", formatList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            return Elements.Select(x => x.ToPxValue()).ToList<string>();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PxValueMultiline : IPxMultipleElements, IPxElement
    {
        /// <summary>
        /// 
        /// </summary>
        IList<string> Lines { get; }

        /// <summary>
        /// 
        /// </summary>
        public IList<dynamic> Values
        {
            get
            {
                return Lines.Cast<dynamic>().ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                return string.Join(" ", Lines.Select(e => e.ToString()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        public PxValueMultiline(IList<string> elements)
        {
            Lines = elements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            return string.Join(" ", Lines.Select(e => e.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValueUnquoted()
        {
            return string.Join(" ", Lines);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
            => $"\"{string.Join("\"\r\n\"", Lines.Select(e => e.ToString()))}\"";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            return Lines;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PxDataValue : IPxMultipleElements, IPxElement
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<IPxSingleElement> Cells { get; }

        /// <summary>
        /// 
        /// </summary>
        public IList<dynamic> Values
        {
            get
            {
                return Cells.Cast<dynamic>().ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cells"></param>
        public PxDataValue(IList<IPxSingleElement> cells)
        {
            Cells = cells;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
            => string.Join(" ", Cells.Select(e => e.ToPxString()));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxString()
            => $"\r\n{string.Join(" ", Cells.Select(e => e.ToPxString()))}";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ToList()
        {
            List<string> returnList = new List<string>();
            foreach (var v in Values)
            {
                returnList.Add(v.ToString());
            }
            return returnList;
        }

    }

}
