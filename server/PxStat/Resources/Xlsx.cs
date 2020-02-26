using API;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PxStat.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PxStat.Resources
{
    internal class XlsxValue
    {
        internal string Value { get; set; }
        internal EnumValue<CellValues> DataType { get; set; }

        internal XlsxValue()
        {
            DataType = CellValues.String;
        }

    }
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
        internal string GetCsv(List<List<XlsxValue>> rowLists, string quote = "")
        {

            string lngIsoCode = Utility.GetUserAcceptLanguage();

            //This will be language dependent
            string sep = Label.Get("default.csv.separator");

            if (String.IsNullOrEmpty(lngIsoCode))
                lngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");

            StringBuilder sb = new StringBuilder();
            foreach (var row in rowLists)
            {
                int counter = 1;
                string line = "";
                foreach (XlsxValue column in row)
                {

                    //check if we need to reformat this by language
                    string word = column.Value;

                    //Doubles will vary by language
                    if (Double.TryParse(word, out double result))
                    {

                        word = result.ToString(CultureInfo.CreateSpecificCulture(lngIsoCode));
                    }


                    line = line + quote + word + quote + (counter < row.Count ? sep : "");
                    counter++;

                }
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        internal string GetXlsx(Matrix theMatrix, List<List<XlsxValue>> rowLists)
        {

            MemoryStream documentStream = new MemoryStream();
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(documentStream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());



            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = theMatrix.Code };
            sheets.Append(sheet);



            if (spreadsheetDocument.WorkbookPart.WorkbookStylesPart == null)
            {
                WorkbookStylesPart stylesPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>();

                stylesPart.Stylesheet = GenerateStyleSheet();


            }

            int rowcounter = 1;
            foreach (List<XlsxValue> rowList in rowLists)
            {

                worksheetPart = InsertDataRow(worksheetPart, rowList, rowcounter, spreadsheetDocument, rowcounter == 1);
                rowcounter++;
            }




            // Close the document.
            spreadsheetDocument.Close();

            documentStream.Seek(0, SeekOrigin.Begin);


            //Tester - not normally invoked
            //FileStream fs = new FileStream(@"C:\nok\Schemas\" + theMatrix.Code + ".xlsx", FileMode.CreateNew);
            //documentStream.WriteTo(fs);
            //fs.Close();

            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                documentStream.CopyTo(ms);
                bytes = ms.ToArray();

            }


            return Utility.EncodeBase64FromByteArray(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        internal WorksheetPart InsertDataRow(WorksheetPart worksheetPart, List<XlsxValue> rowList, int rownum, SpreadsheetDocument spreadsheetDocument, bool boldRow = false)
        {


            // Get the sheetData cell table.
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            // Add a row to the cell table.
            Row row;
            row = new Row();

            sheetData.Append(row);

            // In the new row, find the column location to insert a cell in A1.  
            Cell refCell = null;
            foreach (Cell cell in row.Elements<Cell>())
            {
                if (string.Compare(cell.CellReference.Value, "A" + rownum.ToString(), true) > 0)
                {
                    refCell = cell;
                    break;
                }
            }

            foreach (XlsxValue str in rowList)
            {
                Cell addCell = new Cell();
                row.InsertAfter(addCell, refCell);
                addCell.CellValue = new CellValue(str.Value);
                //If the value was flagged as a potential number, then check if it parses as a number. If so, it will be a number in Xlsx
                if (str.DataType == CellValues.Number)
                {
                    if (!Double.TryParse(str.Value, out double result))
                        addCell.DataType = new EnumValue<CellValues>(CellValues.String);
                }
                else //otherwise we just allow it to be whatever type it was created as (default is CellValues.String)
                    addCell.DataType = str.DataType;
                if (boldRow)
                    addCell.StyleIndex = Convert.ToUInt32(1);
                else
                    addCell.StyleIndex = Convert.ToUInt32(0);
                refCell = addCell;
            }



            return worksheetPart;
        }

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
                        new FontName() { Val = "Times New Roman" })
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
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }      // Index 6 - Border
                )
            );
        }

    }
}
