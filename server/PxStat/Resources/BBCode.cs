using CodeKicker.BBCode;
using System;
using System.Collections.Generic;

namespace PxStat.Resources
{
    /// <summary>
    /// BBCode translator. Translates from bb to http. This is to avoid sending raw html to an API.
    /// </summary>
    internal class BBCode
    {
        /// <summary>
        /// Translates a string from bbCode to http
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        internal string Transform(string inString, bool keepAllChars = false)
        {
            //Set the tags that we are interested in translating
            var parser = new BBCodeParser(new[]
            {
                // A newline is automatically parsed when \n is encountered. Hence [p] is not necessary.
                new BBTag("b", "<b>", "</b>"),
                new BBTag("i", "<i>", "</i>"),
                new BBTag("u", "<u>", "</u>"),
                new BBTag("url", "", "(${url})",  new BBAttribute("url", ""))
            });

            try
            {
                inString = parser.ToHtml(inString);
                if (inString.Contains("&#") && keepAllChars)
                    inString = BBcodeClean(inString);
            }
            catch
            {
                throw new FormatException();
            }
            return inString;
        }


        private string BBcodeClean(string inString)
        {
            List<int> startPostions = new List<int>();
            List<int> utfValue = new List<int>();
            int pos = inString.IndexOf("&#", 0);
            int fuse = 0; //just in case we end up with an infinite loop!
            while (pos > -1 && fuse<=inString.Length)
            {
                if(!startPostions.Contains(pos))
                    startPostions.Add(pos);
                pos = inString.IndexOf("&#", pos + 1);
                fuse++;
            }
            foreach (var item in startPostions)
            {
                int endPos = inString.IndexOf(";", item);
                string midValue = inString.Substring(item + 2, endPos - (item + 2));
                if (Int32.TryParse(midValue, out int result))
                    utfValue.Add(Convert.ToInt32(midValue));
            }
            foreach (int i in utfValue)
            {
                inString = inString.Replace("&#" + i + ";", Convert.ToChar(i).ToString());
            }
            return inString;
        }

    }

}
