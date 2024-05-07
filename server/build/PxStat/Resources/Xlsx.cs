using API;
using PxStat.Data;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using XLsxHelper;


namespace PxStat.Resources
{

    internal class Xlsx
    {

        /// <summary>
        /// Get a CSV representation of the matrix table. 
        /// The quote parameter is optional, if it isn't sent then it will not be implemented
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="rowLists"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        internal string GetCsv(List<List<XlsxValue>> rowLists, string quote = "", CultureInfo ci = null)
        {

            
            //This will be language dependent
            string sep = Label.Get("default.csv.separator");

            
            string lngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            StringBuilder sb = new StringBuilder();

            foreach (var row in rowLists)
            {
                int counter = 1;
                string line = "";
                foreach (XlsxValue column in row)
                {

                    //check if we need to reformat this by language
                    string word = column.Value;

                    //Doubles will vary by language. However, this should only be applied to the last column, i.e. the VALUE
                    if (Double.TryParse(word, out double result) && counter == row.Count)
                    {

                        word = result.ToString(ci != null ? ci : CultureInfo.CreateSpecificCulture(lngIsoCode));
                    }


                    line = line + quote + word + quote + (counter < row.Count ? sep : "");
                    counter++;

                }
                sb.AppendLine(line);
            }

            return ConvertStringToUtf8Bom(sb.ToString());
        }

        internal string ConvertStringToUtf8Bom(string source)
        {
            var data = Encoding.UTF8.GetBytes(source);
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
            var encoder = new UTF8Encoding(true);

            return encoder.GetString(result);
        }

        private string EncodeBase64FromByteArray(byte[] byteArray, string mimeType = null)
        {
            try
            {
                if (byteArray == null)
                {
                    return null;
                }

                if (String.IsNullOrEmpty(mimeType))
                {
                    return Convert.ToBase64String(byteArray);
                }
                else
                {
                    return "data:" + mimeType + ";base64," + Convert.ToBase64String(byteArray);
                }
            }
            catch (Exception)
            {
                //Do not trow nor log. Instead, return null if data cannot be decoded
                return null;
            }
        }


    }
}
