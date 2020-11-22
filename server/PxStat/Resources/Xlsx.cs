using API;
using DocumentFormat.OpenXml.Spreadsheet;
using PxStat.Data;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        internal string GetXlsx(Matrix theMatrix, List<List<XlsxValue>> rowLists, string lngIsoCode = null, CultureInfo ci = null)
        {
            ExcelDocument xl = new ExcelDocument();
            xl.CreateSpreadsheet();
            Specification spec = theMatrix.GetSpecFromLanguage(lngIsoCode);
            if (spec == null) spec = theMatrix.MainSpec;

            //First we create a contents page. This is built up piecemeal
            List<XlsxValue> line;
            List<List<XlsxValue>> matrix = new List<List<XlsxValue>>();

            //some blank lines first
            for (int i = 0; i < 7; i++)
            {
                line = new List<XlsxValue>();
                int headerId = i == 0 ? 1 : 0;
                for (int j = 0; j < 1; j++)
                {
                    XlsxValue xval = new XlsxValue() { Value = "", StyleId = 0, DataType = CellValues.String };

                    line.Add(xval);
                }

                matrix.Add(line);
            }

            //Now create the actual entries in the contents page
            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, CellWidth = 20, Value = Label.Get("xlsx.table", lngIsoCode), StyleId = 8 });
            line.Add(new XlsxValue() { DataType = CellValues.String, CellWidth = 20, Value = "", StyleId = 8 });
            matrix.Add(line);


            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.code", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Code, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.name", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = spec.Contents, StyleId = 0 });
            matrix.Add(line);


            if (theMatrix.Release != null)
            {
                if (theMatrix.Release.RlsLiveFlag && theMatrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                {

                    string dateString = "";
                    if (theMatrix.Release.RlsLiveDatetimeFrom != default)
                    {

                        dateString = theMatrix.Release.RlsLiveDatetimeFrom.ToString(ci != null ? ci : CultureInfo.InvariantCulture);
                        line = new List<XlsxValue>();
                        line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.last-updated", lngIsoCode), StyleId = 1 });
                        line.Add(new XlsxValue() { DataType = CellValues.String, Value = dateString, StyleId = 0 });
                        matrix.Add(line);
                    }

                }
            }

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.note", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = spec.Notes != null ? String.Join(" ", spec.Notes) : spec.NotesAsString != null ? spec.NotesAsString : "", StyleId = 0 });
            matrix.Add(line);

            if (theMatrix.Release != null)
            {
                if (theMatrix.Release.RlsLiveFlag && theMatrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                {
                    string Href = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.restful") +
string.Format(Utility.GetCustomConfig("APP_RESTFUL_DATASET"), Utility.GetCustomConfig("APP_READ_DATASET_API"), theMatrix.Code, Constants.C_SYSTEM_XLSX_NAME, Constants.C_SYSTEM_XLSX_VERSION, spec.Language), Type = Utility.GetCustomConfig("APP_XLSX_MIMETYPE");
                    line = new List<XlsxValue>();
                    line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.url", lngIsoCode), StyleId = 1 });
                    line.Add(new XlsxValue() { DataType = CellValues.String, Value = Href, StyleId = 0 });
                    matrix.Add(line);
                }
            }

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.product", lngIsoCode), StyleId = 8 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = " ", StyleId = 8 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.code", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.PrcCode, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.name", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.PrcValue, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.contacts", lngIsoCode), StyleId = 8 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = " ", StyleId = 8 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.name", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.GrpContactName, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.email", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.GrpContactEmail, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.phone", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.GrpContactPhone, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.copyright", lngIsoCode), StyleId = 8 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = " ", StyleId = 8 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.code", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Copyright.CprCode, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.name", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Copyright.CprValue, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.url", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Copyright.CprUrl, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.properties", lngIsoCode), StyleId = 8 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = " ", StyleId = 8 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.official-statistics", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.IsOfficialStatistic ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode), StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.exceptional", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.RlsExceptionalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode), StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.archived", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.RlsArchiveFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode), StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.analytical", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.RlsAnalyticalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode), StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.experimental", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = theMatrix.Release.RlsExperimentalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode), StyleId = 0 });
            matrix.Add(line);


            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = "", StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.language", lngIsoCode), StyleId = 8 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = " ", StyleId = 8 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.iso-code", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = spec.Language, StyleId = 0 });
            matrix.Add(line);

            line = new List<XlsxValue>();
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = Label.Get("xlsx.iso-name", lngIsoCode), StyleId = 1 });
            line.Add(new XlsxValue() { DataType = CellValues.String, Value = new Language_BSO().Read(spec.Language).LngIsoName, StyleId = 0 });
            matrix.Add(line);

            //We've created the 2D object to represent the contents, so now we can create a worksheet from that:
            xl.InsertDataWorksheet(matrix, Label.Get("About", lngIsoCode), OrientationValues.Landscape, true);

            //On the worksheet we've just created, add an image to the top left hand corner
            xl.AddImage(Configuration_BSO.GetCustomConfig(ConfigType.global, "url.logo"), Label.Get("About", lngIsoCode), 1, 1);

            //Create a second worksheet based on the Matrix contents, unless we've already prepared a data sheet (e.g. when we pivot)
            if (rowLists == null)
                xl.InsertDataWorksheet(theMatrix.GetMatrixSheet(null, false, 1), theMatrix.Code, OrientationValues.Landscape, true);
            else
                xl.InsertDataWorksheet(rowLists, theMatrix.Code, OrientationValues.Landscape, true);

            //cleanup
            xl.Close();

            //Test option...get a local version of the xlsx file
           // xl.SaveToFile(@"C:\nok\Schemas\" + theMatrix.Code + ".xlsx");

            //return the serialized version of the spreadsheet
            return xl.SerializeSpreadsheetFromByteArrayBase64();
        }


    }

}
