
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;


namespace PxStat.Resources
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Cleanser
    {
        /// <summary>
        /// This function runs a number of regex replace operations on the Json string of the parameters.
        /// The operations are:
        /// Remove possible HTML characters
        /// Remove double spaces from anywhere in the string
        /// Left trim anything between double quotes
        /// Right trim anything between double quotes
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static JObject Cleanse(dynamic parameters)
        {
            return JObject.Parse(Cleanse(parameters.ToString()));
        }

        /// <summary>
        /// Run Cleanse operations
        /// </summary>
        /// <param name="aValue"></param>
        /// <returns></returns>
        public static string Cleanse(string aValue, bool htmlStrip = true)
        {
            string pstring = "";

            if (aValue != null)
            {
                pstring = aValue;
                //Remove HTML parameters
                //pstring = Regex.Replace(aValue, @"<.*?>", "");
                if (htmlStrip)
                    pstring = aValue.Replace("<", "").Replace(">", "");

                //Remove double spaces
                pstring = Regex.Replace(pstring, @" {2,}", " ");

                // left trim anything between quotes
                pstring = Regex.Replace(pstring, "\"\\s+", "\"");

                //Right trim anything between quotes
                pstring = Regex.Replace(pstring, "\\s+\"", "\"");
            }

            return pstring;
        }


        /// <summary>
        /// This version will cleanse the parameters individually rather than as a single string
        /// Also, if any parameter has a corresponding DTO attriube of NoHtmlStrip then the HTML cleanse will not be applied to that parameter
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static dynamic Cleanse(dynamic parameters, dynamic dto)
        {
            var info = dto.GetType().GetProperties();

            List<string> props = new List<string>();

            //Cycle through properties
            foreach (PropertyInfo propertyInfo in info)
            {
                //If we find NoHtmlStrip then we add it to our list
                var attrNoHtmlStrip = propertyInfo.CustomAttributes.Where(CustomAttributeData => CustomAttributeData.AttributeType.Name == "NoHtmlStrip").FirstOrDefault();
                if (attrNoHtmlStrip != null)
                    props.Add(propertyInfo.Name);
            }


            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(parameters))
            {
                string pName = prop.Name;
                string pVal = prop.GetValue(parameters);
                //If the Property corresponds to one in the NoHtmlStrip list then we set the parameter to false:
                //The parameter will not be html cleansed in that case
                string cVal = Cleanse(pVal, !props.Contains(pName));
                prop.SetValue(parameters, cVal);

            }
            return parameters;
        }

    }
}
