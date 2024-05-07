using API;
using DocumentFormat.OpenXml.Presentation;
using Ganss.Xss;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;

namespace PxStat.Resources
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Sanitizer
    {
        /// <summary>
        /// Searches an object's properties and if it finds a property with a specific custom attribute, it performs an operation on the value of the property
        /// Also performs some default sanitizations.
        /// </summary>
        /// <param name="DTO"></param>
        /// <returns></returns>
        internal static dynamic Sanitize(dynamic DTO)
        {
            //For all non-null strings we apply some general sanitization rules
            var info = DTO.GetType().GetProperties();



            foreach (PropertyInfo propertyInfo in info)
            {

                string nspace = propertyInfo.PropertyType.Namespace;

                var pvalue = propertyInfo.GetValue(DTO);
                if (propertyInfo.PropertyType.Name.Equals("String"))
                {

                    if (pvalue != null)
                    {
                        var noSanitizeAttribute = propertyInfo.CustomAttributes.Where(x => x.AttributeType.Name == "DefaultSanitizer").FirstOrDefault();
                        if (noSanitizeAttribute==null)
                        {
                            pvalue = CleanValue(propertyInfo, pvalue);
                        }
                        else
                        {
                            //Do this in case somebody tries to go under the radar by using html codes..
                            //pvalue = pvalue.Replace("&lt;", "<");
                            //pvalue = pvalue.Replace("&gt;", ">");

                            pvalue = HttpUtility.HtmlDecode(pvalue);

                            //If we don't sanitize natively then be default we use the HtmlSanitizer library to delete any script tags etc
                            //We pass in the list of allowed tags - which is empty in our case - nothing allowed!

                            //First iteration - nuke all scripts
                            HtmlSanitizer sanitizer = new HtmlSanitizer();
                            pvalue = sanitizer.Sanitize(pvalue);

                            //Second iteration - remove all other tags but keep their contents
                            sanitizer = new HtmlSanitizer();
                            sanitizer.KeepChildNodes = true;
                            pvalue=sanitizer.Sanitize(pvalue);

                            //Allow end users to see tags instead of codes - the sanitizer will have replaced real signs with html codes
                            //pvalue = pvalue.Replace("\u00A0", " ");
                            //pvalue = pvalue.Replace("&gt;", ">");
                            //pvalue = pvalue.Replace("&amp;", "&");

                            pvalue= HttpUtility.HtmlDecode(pvalue);
                        }
                        //update the input object
                        propertyInfo.SetValue(DTO, pvalue);

                    }

                }

                if (nspace.Equals("System.Collections.Generic"))
                {
                    if (pvalue is List<string> && pvalue != null)
                    {
                        List<string> cleanList = new List<string>();

                        foreach (string s in pvalue)
                        {
                            if (s == null)
                            {
                                // Ignore any null values
                                continue;
                            }
                            string clean = CleanValue(propertyInfo, s);
                            cleanList.Add(clean);
                        }
                        propertyInfo.SetValue(DTO, cleanList);
                    }
                }

            }
            return DTO;
        }

        internal static dynamic SanitizePostValidation(dynamic DTO)
        {
            //For all non-null strings we apply some general sanitization rules
            var info = DTO.GetType().GetProperties();



            foreach (PropertyInfo propertyInfo in info)
            {

                string nspace = propertyInfo.PropertyType.Namespace;

                var pvalue = propertyInfo.GetValue(DTO);
                if (propertyInfo.PropertyType.Name.Equals("String"))
                {

                    if (pvalue != null)
                    {
                        //replace the non-breaking line space with a char(32) space
                        pvalue=pvalue.Replace((char)160, (char)32);
                    }
                    propertyInfo.SetValue(DTO, pvalue);
                }
            }

            return DTO;
        }


        /// <summary>
        /// Removes certain values from the string
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="pvalue"></param>
        /// <returns></returns>
        private static string CleanValue(PropertyInfo propertyInfo, string pvalue)
        {
            var t = propertyInfo.CustomAttributes;
            var attrNoHtmlStrip = propertyInfo.CustomAttributes.Where(CustomAttributeData => CustomAttributeData.AttributeType.Name == "NoHtmlStrip").FirstOrDefault();
            var attrNoTrim = propertyInfo.CustomAttributes.Where(CustomAttributeData => CustomAttributeData.AttributeType.Name == "NoTrim").FirstOrDefault();
            var attrLowerCase = propertyInfo.CustomAttributes.Where(CustomAttributeData => CustomAttributeData.AttributeType.Name == "LowerCase").FirstOrDefault();
            var attrUpperCase = propertyInfo.CustomAttributes.Where(CustomAttributeData => CustomAttributeData.AttributeType.Name == "UpperCase").FirstOrDefault();

            pvalue = pvalue.Replace((char)160, (char)32);

            //Strip out all html tags if the NoHtmlStrip attribute is NOT asserted
            if (attrNoHtmlStrip == null)
                pvalue = Regex.Replace(pvalue, @"<.*?>", "");
            if (attrNoTrim == null)
            {
                //left trim - if the NoTrim attribute is NOT asserted
                pvalue = Regex.Replace(pvalue, @"^\s+", "");
                //right trim - if the NoTrim attribute is NOT asserted
                pvalue = Regex.Replace(pvalue, @"\s+$", "");
                //replace double spaces with single ones - if the NoTrim attribute is NOT asserted
                pvalue = Regex.Replace(pvalue, @" {2,}", " ");
                
                
            }
            //Force the value to lower case if the LowerCase attribute is asserted
            if (attrLowerCase != null)
                pvalue = pvalue.ToLower();
            //Force the value to upper case if the UpperCase attribute is asserted
            if (attrUpperCase != null)
                pvalue = pvalue.ToUpper();

            return pvalue;
        }

        internal static string PrepareJsonValue(string readString)
        {
            return Regex.Replace(readString, @"\t|\n|\r", ""); ;
        }
    }


}
