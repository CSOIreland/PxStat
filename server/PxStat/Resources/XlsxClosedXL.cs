using API;
using ClosedXML.Excel;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using static PxStat.Data.Matrix;

namespace PxStat.Resources
{
    internal class XlsxClosedXL
    {
        XLWorkbook workbook;
        internal XlsxClosedXL(MemoryStream xlstream)
        {
            workbook = new XLWorkbook(xlstream);

        }

        internal XlsxClosedXL()
        {
            workbook = new XLWorkbook();
        }

        internal MemoryStream InsertTableSheet(DataTable dt, string title)
        {
            var sheet = workbook.Worksheets.Add(title);
            var table = sheet.Cell(1, 1).InsertTable(dt, title, true);
            sheet.Columns().AdjustToContents();
            MemoryStream output = new MemoryStream();
            workbook.SaveAs(output);
            return output;
        }

        internal MemoryStream CreatAboutPage(Data.Matrix theMatrix, string title, string lngIsoCode, CultureInfo ci = null)
        {
            Specification theSpec = theMatrix.GetSpecFromLanguage(lngIsoCode);

            if (theSpec == null) theSpec = theMatrix.MainSpec;

            string noteString = "";
            if (theSpec.NotesAsString != null)
            {
                noteString = new BBCode().Transform(theSpec.NotesAsString, true);

                //This Regex must be compatible with the output of the BBCode Transform
                noteString = Regex.Replace(noteString, "<a\\shref=\"([^\"]*)\">([^<]*)<\\/a>", "$2 ($1)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }


            if (theMatrix.Release != null)
            {
                if (theMatrix.Release.RlsLiveFlag && theMatrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                {
                    string Href = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.api.restful") +
                        string.Format(Utility.GetCustomConfig("APP_RESTFUL_DATASET"), Utility.GetCustomConfig("APP_READ_DATASET_API"), theMatrix.Code, Constants.C_SYSTEM_XLSX_NAME, Constants.C_SYSTEM_XLSX_VERSION, theSpec.Language), Type = Utility.GetCustomConfig("APP_XLSX_MIMETYPE");
                }

            }

            var sheet = workbook.AddWorksheet(title);
            sheet.AddPicture(new MemoryStream(GetImage(Configuration_BSO.GetCustomConfig(ConfigType.global, "url.logo")))).MoveTo(sheet.Cell("A1"));

            sheet.Columns("B").Width = 100;
            int row = 8;
            int counter = 0;
            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.table", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);// XLColor.AirForceBlue;   //( "A2B8E1");
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.code");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Code;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.name");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theSpec.Title;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.last-updated");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).DataType = XLDataType.Text;
            sheet.Cell(row + counter, 2).SetValue<string>(Convert.ToString(theMatrix.Release.RlsLiveDatetimeFrom.ToString(ci ?? CultureInfo.InvariantCulture)));
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.note");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = noteString;
            counter++;

            if (theMatrix.Release != null)
            {
                if (theMatrix.Release.RlsLiveFlag && theMatrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                {
                    string Href = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.api.restful") +
                        string.Format(Utility.GetCustomConfig("APP_RESTFUL_DATASET"), Utility.GetCustomConfig("APP_READ_DATASET_API"), theMatrix.Code, Constants.C_SYSTEM_XLSX_NAME, Constants.C_SYSTEM_XLSX_VERSION, theSpec.Language), Type = Utility.GetCustomConfig("APP_XLSX_MIMETYPE");

                    sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.url");
                    sheet.Cell(row + counter, 1).Style.Font.Bold = true;
                    sheet.Cell(row + counter, 2).Value = Href;
                    sheet.Cell(row + counter, 2).Hyperlink = new XLHyperlink(Href);
                }
            }
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.product", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.code");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.PrcCode;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.name");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.PrcValue;
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.contacts", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.name");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.GrpContactName;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.email");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.GrpContactEmail;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.phone");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.GrpContactPhone;
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.copyright", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.code");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Copyright.CprCode;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.name");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Copyright.CprValue;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.url");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Copyright.CprUrl;
            sheet.Cell(row + counter, 2).Hyperlink = new XLHyperlink(theMatrix.Copyright.CprUrl);
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.properties", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.official-statistics");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.IsOfficialStatistic ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.exceptional");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.RlsExceptionalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.archived");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.RlsArchiveFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.analytical");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.RlsAnalyticalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.experimental");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theMatrix.Release.RlsExperimentalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.language", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.iso-code");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = theSpec.Language;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.iso-name");
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = new Language_BSO().Read(theSpec.Language).LngIsoName;


            sheet.Columns("A").Width = 20;
            MemoryStream output = new MemoryStream();
            workbook.SaveAs(output);
            return output;
        }

        private byte[] GetImage(string url)
        {

            try
            {

                byte[] imageData = null;

                using (var wc = new WebClient())
                    imageData = wc.DownloadData(url);

                return imageData;
            }
            catch
            {
                return null;
            }

        }

        internal MemoryStream InsertTableAndPivotSheet(DataTable dt, string pivotTableName, string pivotDimension, string tableTitle, string valueField, string unitField, Specification theSpec)
        {
            var sheet = workbook.Worksheets.Add(tableTitle);
            var table = sheet.Cell(1, 1).InsertTable(dt, tableTitle, true);
            sheet.Columns().AdjustToContents();



            //For the Value column, we need to set any cells with numbers to be numeric

            //First we find the values column
            var header = sheet.Rows(1, 1);

            string valueColumnLetter = "";
            foreach (IXLRow row in header)
            {
                foreach (var cell in row.CellsUsed())
                {
                    if (cell.Value.Equals(valueField))
                    {
                        valueColumnLetter = cell.WorksheetColumn().ColumnLetter();
                    }
                }
            }
            var valColumn = sheet.Column(valueColumnLetter);
            //Next we iterate through all used cells on the value column and change the DataType to number if the data is numeric
            if (valColumn != null)
            {
                foreach (IXLCell cell in valColumn.CellsUsed())
                {
                    if (DataAdaptor.IsNumeric(cell.Value.ToString()))
                    {
                        cell.DataType = XLDataType.Number;
                    }
                }
            }

            // Add a new sheet for our pivot table
            var ptSheet = workbook.Worksheets.Add(pivotTableName);

            // Create the pivot table, using the data from the "PastrySalesData" table
            var pt = ptSheet.PivotTables.Add(pivotTableName, ptSheet.Cell(1, 1), table.AsRange());

            // The rows in our pivot table will be the names of the pastries


            // The columns will be the months
            if (pivotDimension != null)
                pt.ColumnLabels.Add(pivotDimension);

            List<string> clsNames = new List<string>();
            foreach (var cls in theSpec.Classification)
            {
                clsNames.Add(cls.Value);
            }
            foreach (DataColumn col in dt.Columns)
            {
                //pivot dimension is treated differently...
                if (!col.ColumnName.Equals(pivotDimension) && !col.ColumnName.Equals(valueField) && !col.ColumnName.Equals(unitField))
                {
                    //Only value fields in the pivot, not codes!
                    if (col.ColumnName == Utility.GetCustomConfig("APP_CSV_STATISTIC") || col.ColumnName == theSpec.Frequency.Value || clsNames.Contains(col.ColumnName))
                        pt.RowLabels.Add(col.ColumnName);
                }
            }

            pt.Values.Add(valueField);

            pt.ShowGrandTotalsColumns = false;
            pt.ShowGrandTotalsRows = false;
            pt.SetShowGrandTotalsRows(false);
            pt.SetShowGrandTotalsColumns(false);

            ptSheet.Columns().AdjustToContents();
            MemoryStream output = new MemoryStream();
            workbook.SaveAs(output);
            return output;
        }



        internal void Dispose()

        {
            workbook.Dispose();
        }
    }
}
