using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace XLsxHelper
{
    /*
     XLsxHelper is a set of minimum functions for creating very basic XLSX documents in C#.
     It's based on the idea that users who want to simply create worksheets based on 2D data matrices should not have to invest time 
     in getting to grips with OpenXML.
     XLsxHelper enables the following:
     - Create a new spreadsheet document
     - Add any number of worksheets
     - Orient the worksheet (Portrait, Landscape)
     - Create a 2D matrix as a list of lists. You will assemble the data as a List<List<XlsxValue>> and choose formats and styles for each cell.
     - Insert the 2D matrix into a worksheet.
     - Set formatting and cell width properties for any cell (or let these be calculated automatically)
     - Set style properties for any cell
     - Save the spreadsheet to a file
     - Generate a string representation of the spreadsheet in order to stream it to e.g. a HTTP client.
     */


    public class ExcelDocument
    {
        SpreadsheetDocument spreadsheetDocument;
        MemoryStream documentStream;

        WorkbookPart workbookPart;
        Sheets sheets;

        /// <summary>
        /// Initial creation of the Spreadsheet
        /// </summary>
        public void CreateSpreadsheet()
        {
            documentStream = new MemoryStream();
            spreadsheetDocument = SpreadsheetDocument.Create(documentStream, SpreadsheetDocumentType.Workbook);

            workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            if (spreadsheetDocument.WorkbookPart.WorkbookStylesPart == null)
            {
                WorkbookStylesPart stylesPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>();

                stylesPart.Stylesheet = GenerateStyleSheet();

            }

            sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
        }

        public string GetHyperlink(string link, string label = null)
        {
            if (label == null)
                return "HYPERLINK(\"" + link + "\")";
            else
                return "HYPERLINK(\"" + link + "\",\"" + label + "\")";

        }

        public List<List<XlsxValue>> GetBlankMatrix()
        {
            List<List<XlsxValue>> matrix = new List<List<XlsxValue>>();
            for (int i = 0; i < 10; i++)
            {
                List<XlsxValue> line = new List<XlsxValue>();
                int headerId = i == 0 ? 1 : 0;
                for (int j = 0; j < 1; j++)
                {
                    XlsxValue xval = new XlsxValue() { Value = "", DataType = CellValues.String, StyleId = 0 };

                    line.Add(xval);
                }

                matrix.Add(line);
            }
            return matrix;
        }
        /// <summary>
        /// Close the spreadsheet
        /// </summary>
        public void Close()
        {
            spreadsheetDocument.Close();
        }

        /// <summary>
        /// Saves the spreadsheet to a file. Requires the full path and filename
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveToFile(string fileNamePath)
        {
            documentStream.Seek(0, SeekOrigin.Begin);
            FileStream fs = new FileStream(fileNamePath, FileMode.CreateNew);
            documentStream.WriteTo(fs);
            fs.Close();
        }

        /// <summary>
        /// Serializes the entire spreadsheet to a Base64 encoded string
        /// This can be returned in a Http Response for downloading the Excel file
        /// </summary>
        /// <returns></returns>
        public string SerializeSpreadsheetFromByteArrayBase64()
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

        /// <summary>
        /// Takes a List<List<XlsxValue>> as a matrix and adds an Excel Worksheet for that data
        /// Style information will have been included in each XlsValue object
        /// You may also specify an orientation at this point
        /// </summary>
        /// <param name="rowLists"></param>
        /// <param name="sheetLabel"></param>
        /// <param name="oval"></param>
        public void InsertDataWorksheet(List<List<XlsxValue>> rowLists, string sheetLabel, OrientationValues oval = OrientationValues.Default, bool Autosize = true)
        {
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = (uint)workbookPart.WorksheetParts.Count(), Name = sheetLabel };

            Worksheet workSheet = new Worksheet();
            SheetData sheetData = new SheetData();

            UInt32Value rowcounter = 1;

            foreach (var list in rowLists)
            {
                // Add a row to the cell table.
                Row row;
                row = new Row() { RowIndex = rowcounter };
                sheetData.Append(row);
                Cell refCell = null;
                foreach (XlsxValue str in list)
                {

                    Cell addCell = new Cell();
                    row.InsertAfter(addCell, refCell);
                    if (str.FormulaText != null)
                    {
                        CellFormula cf = new CellFormula() { Space = SpaceProcessingModeValues.Preserve };
                        cf.Text = str.FormulaText;
                        addCell.Append(cf);
                    }
                    addCell.CellValue = new CellValue(str.Value);
                    //If the value was flagged as a potential number, then check if it parses as a number.If so, it will be a number in Xlsx
                    if (str.DataType == CellValues.Number)
                    {
                        if (!Double.TryParse(str.Value, out double result))
                            addCell.DataType = new EnumValue<CellValues>(CellValues.String);
                    }
                    else //otherwise we just allow it to be whatever type it was created as (default is CellValues.String)
                        addCell.DataType = str.DataType;

                    addCell.StyleIndex = (uint)str.StyleId;


                    refCell = addCell;
                }


                rowcounter++;
            }

            //Set column sizes, either automatically or with reference to each XlsxValue property
            if (Autosize)
                workSheet.Append(AutoSize(sheetData));
            else
                workSheet.Append(SetColumnSize(rowLists));


            workSheet.AppendChild(sheetData);
            worksheetPart.Worksheet = workSheet;
            sheets.Append(sheet);

            if (oval != OrientationValues.Default)
            {
                SetOrientation(worksheetPart, oval);
            }

        }

        private WorksheetPart GetWorksheetPartFromWorksheetName(string wsName)
        {
            var id = (spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().First(s => s.Name.Equals(wsName))).Id;
            if (id != null)
                return (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(id);
            else
                return null;
        }

        public bool AddImage(string url, string wsName, int xpos, int ypos)
        {
            WorksheetPart ws = GetWorksheetPartFromWorksheetName(wsName);
            if (ws == null) return false;

            try
            {

                byte[] imageData = null;

                using (var wc = new WebClient())
                    imageData = wc.DownloadData(url);

                Utilities.AddImage(ws, new MemoryStream(imageData), "logo", xpos, ypos);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Automatically sizes an excel sheet depending on the maximum width of a data cell
        /// Originally developed by Hath: https://stackoverflow.com/users/5186/hath
        /// Original posting: https://stackoverflow.com/questions/18268620/openxml-auto-size-column-width-in-excel
        /// </summary>
        /// <param name="sheetData"></param>
        /// <returns></returns>
        private Columns AutoSize(SheetData sheetData)
        {
            var maxColWidth = GetMaxCharacterWidth(sheetData);

            Columns columns = new Columns();
            //this is the width of my font - yours may be different
            double maxWidth = 7;
            foreach (var item in maxColWidth)
            {
                //width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;

                //pixels=Truncate(((256 * {width} + Truncate(128/{Maximum Digit Width}))/256)*{Maximum Digit Width})
                double pixels = Math.Truncate(((256 * width + Math.Truncate(128 / maxWidth)) / 256) * maxWidth);

                //character width=Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100
                double charWidth = Math.Truncate((pixels - 5) / maxWidth * 100 + 0.5) / 100;

                Column col = new Column() { BestFit = true, Min = (UInt32)(item.Key + 1), Max = (UInt32)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)width };
                columns.Append(col);
            }

            return columns;
        }

        /// <summary>
        /// This function will set the column size to the maximum value of XlsxValue.CellWidth for that column
        /// Note that XlsxValue.CellWidth will have a default value if it hasn't been set.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private Columns SetColumnSize(List<List<XlsxValue>> matrix)
        {
            var maxColWidth = GetMaxCharacterWidth(matrix);

            Columns columns = new Columns();
            //this is the width of my font - yours may be different
            double maxWidth = 7;
            foreach (var item in maxColWidth)
            {
                //width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;

                //pixels=Truncate(((256 * {width} + Truncate(128/{Maximum Digit Width}))/256)*{Maximum Digit Width})
                double pixels = Math.Truncate(((256 * width + Math.Truncate(128 / maxWidth)) / 256) * maxWidth);

                //character width=Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100
                double charWidth = Math.Truncate((pixels - 5) / maxWidth * 100 + 0.5) / 100;

                Column col = new Column() { BestFit = true, Min = (UInt32)(item.Key + 1), Max = (UInt32)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)width };
                columns.Append(col);
            }

            return columns;
        }

        /// <summary>
        /// Sets column widths to specific values
        /// If any cell contains a non-zero CellWidth, then the column for that cell will have its width set to the 
        /// maximum width of a cell in that column
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private Dictionary<int, int> GetMaxCharacterWidth(List<List<XlsxValue>> matrix)
        {
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();

            foreach (List<XlsxValue> line in matrix)
            {
                int i = 0;
                foreach (XlsxValue item in line)
                {
                    if (maxColWidth.ContainsKey(i))
                        maxColWidth[i] = item.CellWidth > maxColWidth[i] ? item.CellWidth : maxColWidth[i];
                    else
                        maxColWidth.Add(i, item.CellWidth);

                    i++;
                }
            }
            return maxColWidth;
        }

        /// <summary>
        /// Use with Autosize
        /// </summary>
        /// <param name="sheetData"></param>
        /// <returns></returns>
        private Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData)
        {
            //iterate over all cells getting a max char value for each column
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();
            var rows = sheetData.Elements<Row>();
            UInt32[] numberStyles = new UInt32[] { 5, 6, 7, 8 }; //styles that will add extra chars
            UInt32[] boldStyles = new UInt32[] { 1, 2, 3, 4, 6, 7, 8 }; //styles that will bold
            foreach (var r in rows)
            {
                var cells = r.Elements<Cell>().ToArray();

                //using cell index as my column
                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];
                    var cellValue = cell.CellValue == null ? string.Empty : cell.CellValue.InnerText;
                    var cellTextLength = cellValue.Length;

                    if (cell.StyleIndex != null && numberStyles.Contains(cell.StyleIndex))
                    {
                        int thousandCount = (int)Math.Truncate((double)cellTextLength / 4);

                        //add 3 for '.00' 
                        cellTextLength += (3 + thousandCount);
                    }

                    if (cell.StyleIndex != null && boldStyles.Contains(cell.StyleIndex))
                    {
                        //add an extra char for bold 
                        cellTextLength += 1;
                    }

                    if (maxColWidth.ContainsKey(i))
                    {
                        var current = maxColWidth[i];
                        if (cellTextLength > current)
                        {
                            maxColWidth[i] = cellTextLength + 2;
                        }
                    }
                    else
                    {
                        maxColWidth.Add(i, cellTextLength);
                    }
                }
            }

            return maxColWidth;
        }

        /// <summary>
        /// Create a stylesheet
        /// Unknown origin, possibly http://polymathprogrammer.com/ (Vincent Tan)
        /// </summary>
        /// <returns></returns>
        private Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(                                                               // Index 0 - The default font.
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 1 - The bold font.
                        new Bold(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 2 - The Italic font.
                        new Italic(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 3 - The Times Roman font. with 16 size
                        new FontSize() { Val = 16 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Times New Roman" }),
                    new Font(
                        new Bold(),                                                           // Index 4 - The Calibri font, bold with 13 size
                        new FontSize() { Val = 13 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                       // Index 4 - The Calibri font, bold with 13 size
                        new Bold(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "0645AD" } }, //Index 5 - Calibri blue, suitable for hyperlinks
                        new FontName() { Val = "Calibri" })
                ),
                new Fills(
                    new Fill(                                                           // Index 0 - The default fill.
                        new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(                                                           // Index 1 - The default fill of gray 125 (required)
                        new PatternFill() { PatternType = PatternValues.Gray125 }),
                    new Fill(                                                           // Index 2 - The yellow fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFFFFF00" } }
                        )
                        { PatternType = PatternValues.Solid })
                ),
                new Borders(
                    new Border(                                                         // Index 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                    new Border(                                                         // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                        new LeftBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new RightBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new TopBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new BottomBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new DiagonalBorder()),
                    new Border(                                                         // Index 2 - White (normally invisible) Border on all sides
                        new LeftBorder(
                            new Color() { Rgb = new HexBinaryValue() { Value = "FFFFFFFF" } }
                        )
                        { Style = BorderStyleValues.Thin },
                        new RightBorder(
                            new Color() { Rgb = new HexBinaryValue() { Value = "FFFFFFFF" } }
                        )
                        { Style = BorderStyleValues.Thin },
                        new TopBorder(
                            new Color() { Rgb = new HexBinaryValue() { Value = "FFFFFFFF" } }
                        )
                        { Style = BorderStyleValues.Thin },
                        new BottomBorder(
                            new Color() { Rgb = new HexBinaryValue() { Value = "FFFFFFFF" } }
                        )
                        { Style = BorderStyleValues.Thin },
                        new DiagonalBorder()),
                    new Border(                                                         // Index 3 - Default border except bottom - heavy blue border
                        new LeftBorder(

                        ),
                        new RightBorder(
                        ),
                        new TopBorder(
                        ),
                        new BottomBorder(
                            new Color() { Rgb = new HexBinaryValue() { Value = "A2B8E1" } }
                        )
                        { Style = BorderStyleValues.Thick },
                        new DiagonalBorder())
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },                          // Index 0 - The default cell style.  If a cell does not have a style index applied it will use this style combination instead
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 1 - Bold 
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 2 - Italic
                    new CellFormat() { FontId = 3, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 3 - Times Roman
                    new CellFormat() { FontId = 0, FillId = 2, BorderId = 0, ApplyFill = true },       // Index 4 - Yellow Fill
                    new CellFormat(                                                                   // Index 5 - Alignment
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                    )
                    { FontId = 0, FillId = 0, BorderId = 0, ApplyAlignment = true },
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true },      // Index 6 - Border
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 2, ApplyBorder = true },       //Index 7 - White (normally invisible) Border, standard fonts
                    new CellFormat() { FontId = 4, FillId = 0, BorderId = 3, ApplyBorder = true },       //Index 8 - White (normally invisible) Border, Calibri 13, Bold 
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 2, ApplyBorder = true },      //Index 9 - White (normally invisible) Border, Bold
                    new CellFormat() { FontId = 5, FillId = 0, BorderId = 0, ApplyFont = true } //Index 10 - hyperlink blue
                )
            );

        }



        /// <summary>
        /// Set orientation of the worksheet, i.e. Portrait, Landscape or Default
        /// </summary>
        /// <param name="worksheetPart"></param>
        /// <param name="oval"></param>
        public void SetOrientation(WorksheetPart worksheetPart, OrientationValues oval)
        {

            PageSetup pageSetup = new PageSetup() { Orientation = oval };

            worksheetPart.Worksheet.AppendChild(pageSetup);
            worksheetPart.Worksheet.Save();

            workbookPart.Workbook.Save();
        }

        /// <summary>
        /// Encode a byte array into a base64 string
        /// N.B. UFT8 in C# includes UTF16 too
        /// Taken from https://github.com/CSOIreland/Server-API-Library/blob/master/src/API.Library/Entities/Utility.cs
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string EncodeBase64FromByteArray(byte[] byteArray, string mimeType = null)
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

    /// <summary>
    /// Class to be used as a container for an XLSX data cell
    /// </summary>
    public class XlsxValue
    {
        /// <summary>
        /// The contents of the cell
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// The DataType of the cell (CellValues.*)
        /// </summary>
        public EnumValue<CellValues> DataType { get; set; }

        /// <summary>
        /// The StyleId. These correspond to the GenerateStyleSheet() index values
        /// </summary>
        public int StyleId { get; set; }
        /// <summary>
        /// Width of the cell. When calculating column widths, the application may set column width to the maximum column widths
        /// </summary>
        public int CellWidth { get; set; }

        public string FormulaText { get; set; }

        /// <summary>
        /// Constructor - set some default values
        /// </summary>
        public XlsxValue()
        {
            DataType = CellValues.String;
            CellWidth = XlConstants.DEFAULT_CELL_WIDTH;
        }

    }

    internal static class XlConstants
    {
        internal const int DEFAULT_CELL_WIDTH = 12;
    }
}
