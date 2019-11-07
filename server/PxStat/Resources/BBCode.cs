using CodeKicker.BBCode;
using System;


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
        internal string Transform(string inString)
        {
            //Set the tags that we are interested in translating
            var parser = new BBCodeParser(new[]
                {
                    new BBTag("b", "<b>", "</b>"),
                    new BBTag("i", "<i>", "</i>"),
                    new BBTag("u", "<u>", "</u>"),
                    new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
                });

            try
            {
                inString = parser.ToHtml(inString);
            }
            catch
            {
                throw new FormatException();
            }
            return inString;
        }
    }

}