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
using static PxStat.Data.Matrix;


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

            string lngIsoCode = Utility.GetUserAcceptLanguage();

            //This will be language dependent
            string sep = Label.Get("default.csv.separator");

            if (String.IsNullOrEmpty(lngIsoCode))
                lngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

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

        /// <summary>
        /// Create and get a spreadsheet as a serialized string based on the Matrix
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="rowLists"></param>
        /// <param name="lngIsoCode"></param>
        /// <param name="ci"></param>
        /// <returns></returns>
        internal string GetXlsx(Matrix theMatrix, string lngIsoCode = null, CultureInfo ci = null, string pivot = null, bool viewCodes = true)
        {

            Specification spec = theMatrix.GetSpecFromLanguage(lngIsoCode);
            if (spec == null) spec = theMatrix.MainSpec;

            XlsxClosedXL xcl = new XlsxClosedXL();

            MemoryStream documentStream = xcl.CreatAboutPage(theMatrix, "About", lngIsoCode);


            if (pivot != null)
            {
                //We must get the dimension name from the dimension code - the pivot variable contains only the code
                if (pivot.Equals(spec.Frequency.Code)) pivot = spec.Frequency.Value;
                foreach (var cls in spec.Classification)
                {
                    if (pivot.Equals(cls.Code))
                    {
                        pivot = cls.Value;
                        break;
                    }
                }
            }

            xcl = new XlsxClosedXL(documentStream);



            documentStream = xcl.InsertTableAndPivotSheet(theMatrix.GetMatrixDataTable(null, false, 1, viewCodes), Label.Get("xlsx.pivoted", lngIsoCode), pivot, Label.Get("xlsx.unpivoted", lngIsoCode), Label.Get("xlsx.value", lngIsoCode), Label.Get("xlsx.unit", lngIsoCode), spec);

            //Test option...get a local version of the xlsx file
            //SaveToFile(@"C:\nok\Schemas\" + theMatrix.Code + ".xlsx", documentStream);

            //return the serialized version of the spreadsheet
            return SerializeSpreadsheetFromByteArrayBase64(documentStream);

        }

        internal void SaveToFile(string fileNamePath, MemoryStream documentStream)
        {
            documentStream.Seek(0, SeekOrigin.Begin);
            FileStream fs = new FileStream(fileNamePath, FileMode.CreateNew);
            documentStream.WriteTo(fs);
            fs.Close();
        }

        internal string SerializeSpreadsheetFromByteArrayBase64(MemoryStream documentStream)
        {
            documentStream.Seek(0, SeekOrigin.Begin);
            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                documentStream.CopyTo(ms);
                bytes = ms.ToArray();

            }

            return EncodeBase64FromByteArray(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
