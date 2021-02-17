
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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

            var typ = DTO.GetType();

            string so = JsonConvert.SerializeObject(DTO);

            foreach (PropertyInfo propertyInfo in info)
            {

                string nspace = propertyInfo.PropertyType.Namespace;

                var pvalue = propertyInfo.GetValue(DTO);
                if (propertyInfo.PropertyType.Name.Equals("String"))
                {

                    if (pvalue != null)
                    {
                        pvalue = CleanValue(propertyInfo, pvalue);
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
                            string clean = CleanValue(propertyInfo, s);
                            cleanList.Add(clean);
                        }
                        propertyInfo.SetValue(DTO, cleanList);
                    }
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
    }
}
