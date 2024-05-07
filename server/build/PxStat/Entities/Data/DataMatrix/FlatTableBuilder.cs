using API;
using ClosedXML.Excel;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace PxStat.Data
{

    public class FlatTableBuilder
    {
        //Express the matrix as a data table
        internal DataTable GetMatrixDataTableCodesAndLabels(IDmatrix matrix, string lngIsoCode = null, bool indicateBlankSymbols = false, int headerStyle = 0, bool viewCodes = true, bool codesOnly = false)
        {
            DataTable dt = new DataTable();

            lngIsoCode = lngIsoCode == null || !matrix.Dspecs.ContainsKey(lngIsoCode) ? Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") : lngIsoCode;
            
            Dspec theSpec = matrix.Dspecs[lngIsoCode];

            foreach (var dimension in theSpec.Dimensions)
            {
                if (viewCodes)
                {
                    var code = dimension.Code;
                    dt.Columns.Add(code);


                }
                var label = dimension.Value;
                if (dimension.Code.ToUpper().Equals(dimension.Value.ToUpper()))
                {
                    label= dimension.Value + " " +Label.Get("default.csv.label");
                }

                dt.Columns.Add(label);

            }


            dt.Columns.Add(Label.Get("xlsx.unit", lngIsoCode));
            dt.Columns.Add(Label.Get("xlsx.value", lngIsoCode), typeof(double));




            IEnumerable<ValueElement> cells;
            int cellCounter = 0;

            cells = matrix.Cells.Select(c => (ValueElement)c);

            List<List<IDimensionVariable>> dlist = new List<List<IDimensionVariable>>();
            foreach (var m in theSpec.Dimensions)
                dlist.Add(m.Variables);

            var cjoin = CartesianProduct(dlist);



            if (cells.Count() > 0)

            {
                List<dynamic> Cells = matrix.Cells.ToList();

                Dictionary<int, dynamic> dCells = new Dictionary<int, dynamic>();

                int i = 0;
                foreach (var cell in Cells)
                {
                    dCells.Add(i, cell);
                    i++;
                }

                string confidential = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "px.confidential-value");
                foreach (var dataPoint in cjoin)
                {
                    int col = -1;
                    DataRow dr = dt.NewRow();
                    string unit = "";

                    foreach (var dim in dataPoint)
                    {
                        if (viewCodes) 
                            dr[++col] = dim.Code;
                        dr[++col] = dim.Value;
                        if (!String.IsNullOrEmpty(dim.Unit)) unit = dim.Unit;
                    }
                    dr[++col] = unit;

                    dynamic cell = dCells[cellCounter];

                    string emptyValue = indicateBlankSymbols ? confidential : "";

                    string val = cell.ToString();

                    if(Double.TryParse(val, out double result))
                    {
                        dr[++col] = val;
                    }
                    else 
                    {
                        dr[++col] = DBNull.Value;

                    }
                    



                    cellCounter++;

                    dt.Rows.Add(dr);
                }



            }

            return dt;
        }

        public DataTable GetMatrixDataTableCodesOnly(IDmatrix matrix, string lngIsoCode = null, bool indicateBlankSymbols = true, bool headerOnly = false)
        {
            DataTable dt = new DataTable();

            lngIsoCode = lngIsoCode == null ? Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") : lngIsoCode;

            Dspec theSpec = matrix.Dspecs[lngIsoCode];

            foreach (var vrb in theSpec.Dimensions)
            {

                dt.Columns.Add(vrb.Code);


            }


            dt.Columns.Add(Label.Get("xlsx.value", lngIsoCode));


            if (!headerOnly)
            {

                IEnumerable<ValueElement> cells;
                int cellCounter = 0;

                cells = matrix.Cells.Select(c => (ValueElement)c);

                List<List<IDimensionVariable>> dlist = new List<List<IDimensionVariable>>();
                foreach (var m in theSpec.Dimensions)
                    dlist.Add(m.Variables);

                var cjoin = CartesianProduct(dlist);



                if (cells.Count() > 0)

                {
                    List<dynamic> Cells = matrix.Cells.ToList();

                    Dictionary<int, dynamic> dCells = new Dictionary<int, dynamic>();

                    int i = 0;
                    foreach (var cell in Cells)
                    {
                        dCells.Add(i, cell);
                        i++;
                    }

                    string confidential = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "px.confidential-value");
                    foreach (var dataPoint in cjoin)
                    {
                        int col = -1;
                        DataRow dr = dt.NewRow();


                        foreach (var dim in dataPoint)
                        {
                            dr[++col] = dim.Code;
                        }


                        dynamic cell = dCells[cellCounter];

                        string emptyValue = indicateBlankSymbols ? confidential : "";

                        string val = cell.ToString();


                        dr[++col] = val == confidential ? DBNull.Value : cell.ToString();



                        cellCounter++;

                        dt.Rows.Add(dr);
                    }

                }

            }

            return dt;
        }

        public DataTable GetMatrixDataTableCodesAndSequences(IDmatrix matrix, string lngIsoCode = null, bool indicateBlankSymbols = true, bool headerOnly = false)
        {
            DataTable dt = new DataTable();

            lngIsoCode = lngIsoCode == null ? Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") : lngIsoCode;

            Dspec theSpec = matrix.Dspecs[lngIsoCode];

            foreach (var vrb in theSpec.Dimensions)
            {

                dt.Columns.Add(vrb.Code);
                dt.Columns.Add(vrb.Code + "_sequence",typeof(int));

            }


            dt.Columns.Add(Label.Get("xlsx.value", lngIsoCode));


            if (!headerOnly)
            {

                IEnumerable<ValueElement> cells;
                int cellCounter = 0;

                cells = matrix.Cells.Select(c => (ValueElement)c);

                List<List<IDimensionVariable>> dlist = new List<List<IDimensionVariable>>();
                foreach (var m in theSpec.Dimensions)
                    dlist.Add(m.Variables);

                var cjoin = CartesianProduct(dlist);



                if (cells.Count() > 0)

                {
                    List<dynamic> Cells = matrix.Cells.ToList();

                    Dictionary<int, dynamic> dCells = new Dictionary<int, dynamic>();

                    int i = 0;
                    foreach (var cell in Cells)
                    {
                        dCells.Add(i, cell);
                        i++;
                    }

                    string confidential = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "px.confidential-value");
                    foreach (var dataPoint in cjoin)
                    {
                        int col = -1;
                        DataRow dr = dt.NewRow();


                        foreach (var dim in dataPoint)
                        {
                            dr[++col] = dim.Code;
                            dr[++col] = dim.Sequence;
                        }


                        dynamic cell = dCells[cellCounter];

                        string emptyValue = indicateBlankSymbols ? confidential : "";

                        string val = cell.ToString();


                        dr[++col] = val == confidential ? DBNull.Value : cell.ToString();



                        cellCounter++;

                        dt.Rows.Add(dr);
                    }

                }

            }

            return dt;
        }

        internal string GetXlsx(IDmatrix theMatrix, string lngIsoCode, CultureInfo ci = null, string pivot = null, bool viewCodes = true)
        {
            Dspec spec;

            if (!theMatrix.Dspecs.ContainsKey(lngIsoCode)) 
            {
                lngIsoCode=Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            }
            spec = theMatrix.Dspecs[lngIsoCode];


            XlsxClosedXL xcl = new XlsxClosedXL();



            MemoryStream documentStream = xcl.CreateAboutPage(theMatrix, Label.Get("xlsx.about",lngIsoCode), lngIsoCode, ci);


            if (pivot != null)
            {
                StatDimension tdim = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault();
                //We must get the dimension name from the dimension code - the pivot variable contains only the code
                if (pivot.Equals(tdim.Code)) pivot = tdim.Value;
                foreach (var dim in spec.Dimensions)
                {
                    if (pivot.Equals(dim.Code))
                    {
                        if (dim.Code.ToUpper().Equals(dim.Value.ToUpper()))
                        {
                            pivot = dim.Value + " " + Label.Get("default.csv.label");
                        }
                        else
                        pivot = dim.Value;
                        break;
                    }
                }
            }

            xcl = new XlsxClosedXL(documentStream);



            documentStream = InsertTableAndPivotSheet(documentStream, GetMatrixDataTableCodesAndLabels(theMatrix, lngIsoCode, false, 1, viewCodes), Label.Get("xlsx.pivoted", lngIsoCode), pivot, Label.Get("xlsx.unpivoted", lngIsoCode), Label.Get("xlsx.value", lngIsoCode), Label.Get("xlsx.unit", lngIsoCode), spec);

            //Test option...get a local version of the xlsx file
            
            //SaveToFile(@"C:\nok\Schemas\" + theMatrix.Code + ".xlsx", documentStream);

            //return the serialized version of the spreadsheet
            return SerializeSpreadsheetFromByteArrayBase64(documentStream);

        }

        internal MemoryStream InsertTableAndPivotSheet(MemoryStream documentStream, DataTable dt, string pivotTableName, string pivotDimension, string tableTitle, string valueField, string unitField, Dspec theSpec)
        {
            var workbook = new XLWorkbook(documentStream);

            var sheet = workbook.Worksheets.Add(tableTitle);
            var table = sheet.Cell(1, 1).InsertTable(dt, tableTitle, true);
            sheet.Columns().AdjustToContents();



            //For the Value column, we need to set any cells with numbers to be numeric

            //First we find the values column
            var header = sheet.Rows(1, 1);



            // Add a new sheet for our pivot table
            var ptSheet = workbook.Worksheets.Add(pivotTableName);

            var pt = ptSheet.PivotTables.Add(pivotTableName, ptSheet.Cell(1, 1), table.AsRange());

            //List<string> columnNames = new ();
            //foreach(var col in dt.Columns)
            //{
            //    if (!col.ToString().Equals(pivotDimension) && !col.ToString().Equals(valueField) && !col.ToString().Equals(unitField) && ! pt.RowLabels.Contains(col.ToString()))
            //        pt.RowLabels.Add(col.ToString());
            //}
            if (pivotDimension != null)
            {
  
                pt.ColumnLabels.Add(pivotDimension);

            }
            

            List<string> clsNames = new List<string>();
            foreach (var cls in theSpec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION))
            {
                clsNames.Add(cls.Value);
            }


            foreach (DataColumn col in dt.Columns)
            {
                //pivot dimension is treated differently...
                if (!col.ColumnName.Equals(pivotDimension) && !col.ColumnName.Equals(valueField) && !col.ColumnName.Equals(unitField))
                {
                    //Only value fields in the pivot, not codes!

                    //if (GetDimensionNameFromColumnName(col.ColumnName) == theSpec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault().Value || col.ColumnName == theSpec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault().Value || clsNames.Contains(col.ColumnName))
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

        /// <summary>
        /// Translates from a column name to a dimension name where necessary
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private string GetDimensionNameFromColumnName(string columnName)
        {
            string ending = " " + Label.Get("default.csv.label");
            if (columnName.EndsWith(ending))
                return columnName.Substring(0, columnName.IndexOf(ending));
            return columnName;
        }


        internal IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct =
              new[] { Enumerable.Empty<T>() };
            IEnumerable<IEnumerable<T>> result = emptyProduct;
            foreach (IEnumerable<T> sequence in sequences)
            {
                result = from accseq in result from item in sequence select accseq.Concat(new[] { item });
            }
            return result;
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

        internal string GetXlsxObject(IDmatrix matrix, string lngIsoCode = null, CultureInfo ci = null, string pivot = null, bool viewCodes = true)
        {
            return GetXlsx(matrix, lngIsoCode, ci, pivot, viewCodes);

        }


        /// <summary>
        /// Get the data as datatable if we're pivoting the data
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="lngIsoCode"></param>
        /// <param name="pivot"></param>
        /// <param name="viewCodes"></param>
        /// <param name="indicateBlankSymbols"></param>
        /// <returns></returns>
        internal DataTable GetMatrixDataTablePivot(IDmatrix matrix, string lngIsoCode, string pivot, bool viewCodes = true, bool indicateBlankSymbols = false)
        {
            DataTable dt = new DataTable();
            var spec = matrix.Dspecs[lngIsoCode];

            List<StatDimension> noPivotDimensions = new List<StatDimension>();
            noPivotDimensions = (List<StatDimension>)spec.Dimensions.Where(x => x.Code != pivot).ToList();

            StatDimension pivotDimension = (StatDimension)(spec.Dimensions.Where(x => x.Code == pivot).FirstOrDefault());

            List<List<IDimensionVariable>> nonPivotVariables = new List<List<IDimensionVariable>>();


            foreach (var dim in noPivotDimensions)
            {
                if (dim.Code != pivot)
                    nonPivotVariables.Add(dim.Variables);
            }



            foreach (var dim in noPivotDimensions)
            {
                if (viewCodes)
                    dt.Columns.Add(dim.Code.Equals(dim.Value) ? dim.Code + Label.Get("default.csv.code", lngIsoCode) : dim.Code);
                dt.Columns.Add(dim.Value);
            }

            if (!pivotDimension.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))
            {
                dt.Columns.Add(Label.Get("xlsx.unit", lngIsoCode));
            }

            foreach (var vrb in pivotDimension.Variables)
            {
                if (pivotDimension.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))
                    dt.Columns.Add(vrb.Value + " (" + vrb.Unit + ')');
                else
                {
                    dt.Columns.Add(vrb.Value);
                }
            }


            IEnumerable<ValueElement> cells;
            int cellCounter = 0;

            cells = matrix.Cells.Select(c => (ValueElement)c);
            var pivotJoin = CartesianProduct(nonPivotVariables);

            //We can't use the default sorting of the cells. Instead we first sort the data by the dimensions that are not the pivot
            matrix.Cells = GetSortedCellsForPivot(spec, pivot, matrix.Cells.ToList(), noPivotDimensions);

            if (matrix.Cells.Count() > 0)
            {
                string confidential = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "px.confidential-value");
                foreach (var dataRow in pivotJoin)
                {
                    int col = -1;
                    DataRow dr = dt.NewRow();
                    string unit = "";
                    //The first columns are the usual variable data
                    foreach (var dim in dataRow)
                    {
                        if (viewCodes) dr[++col] = dim.Code;
                        dr[++col] = dim.Value;
                        if (!String.IsNullOrEmpty(dim.Unit)) unit = dim.Unit;
                    }
                    //dr[++col] = unit;

                    //Instead of the dimension, we insert a data value for each variable in the pivot dimension as a column of its own

                    //Calculate the column position of the values
                    int colOffset = 0;
                    if (viewCodes)
                        colOffset = (noPivotDimensions.Count * 2) - 1;
                    else colOffset = noPivotDimensions.Count;// - 1;

                    if (!pivotDimension.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_STATISTIC))
                    {
                        colOffset++;
                        dr[++col] = unit;
                    }

                    if (viewCodes) colOffset++;



                    //Add the unit for the data row

                    for (int i = 0; i < (pivotDimension.Variables.Count); i++)
                    {
                        dynamic cell = matrix.Cells.ToList()[cellCounter + i];
                        string emptyValue = indicateBlankSymbols ? confidential : "";

                        string val = cell.ToString();
                        if (!DataAdaptor.IsNumeric(val)) { val = confidential; }

                        dr[colOffset + i] = val == confidential ? DBNull.Value : cell.ToString();
                    }

                    cellCounter += pivotDimension.Variables.Count;

                    dt.Rows.Add(dr);
                }
            }



            return dt;
        }

        /// <summary>
        /// Sort the Cells for a pivot query
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="pivot"></param>
        /// <param name="cells"></param>
        /// <param name="noPivotDimensions"></param>
        /// <returns></returns>
        private IList<dynamic> GetSortedCellsForPivot(IDspec spec, string pivot, IList<dynamic> cells, List<StatDimension> noPivotDimensions)
        {
            //Create a table with the dimension sequences plus the values
            DataTable dt = new DataTable();
            foreach (var dim in spec.Dimensions)
            {
                dt.Columns.Add(dim.Code);
                dt.Columns.Add(dim.Code + "_dimSeq", typeof(Int32));
            }
            dt.Columns.Add("Value");

            List<List<IDimensionVariable>> allVariables = new List<List<IDimensionVariable>>();
            foreach (var dim in spec.Dimensions)
            {
                allVariables.Add(dim.Variables);
            }


            //Create a 2D version of the matrix
            var fullJoin = CartesianProduct(allVariables);
            int rowCount = 0;

            //Convert the 2D matrix to the new table
            foreach (var fj in fullJoin) // each row
            {
                DataRow dr = dt.NewRow();
                int colCount = 0;
                int dimCount = 0;
                foreach (var item in fj)//each dimension
                {
                    dr[colCount] = spec.Dimensions.ToList()[dimCount].Code;
                    dr[++colCount] = item.Sequence;
                    colCount++;
                    dimCount++;
                }
                dr["Value"] = cells[rowCount];
                dt.Rows.Add(dr);
                rowCount++;
            }

            //Sort by all dimension sequences except the pivot
            DataView dv = dt.DefaultView;
            string sortString = "";
            int dimCounter = 1;
            foreach (var dim in noPivotDimensions)
            {
                sortString = sortString + dim.Code + "_dimSeq" + (dimCounter < noPivotDimensions.Count ? "," : "");
                dimCounter++;
            }
            dv.Sort = sortString;
            dt = dv.ToTable();

            //Get the Values column as a List<dynamic>
            cells = dt.Rows.OfType<DataRow>()
    .Select(dr => dr.Field<string>("Value")).ToList<dynamic>();

            return cells;
        }

        public string GetCsv(DataTable dt, string quote = "", CultureInfo ci = null, bool showConfidential = false)
        {

            string lngIsoCode;// = Utility.GetUserAcceptLanguage();

            //This will be language dependent
            string sep = Label.Get("default.csv.separator");

            //if (String.IsNullOrEmpty(lngIsoCode))
                lngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            StringBuilder sb = new StringBuilder();

            string headerText = "";
            int headerCounter = 1;
            
            foreach (DataColumn col in dt.Columns)
            {

                headerText = headerText + quote + col.ColumnName + quote + (headerCounter < (dt.Columns.Count) ? sep : "");
                headerCounter++;
            }

            sb.AppendLine(headerText);

            string confidentialValue = "";
            if (showConfidential) confidentialValue = Configuration_BSO.GetStaticConfig("APP_PX_CONFIDENTIAL_VALUE");
            
            foreach (DataRow row in dt.Rows)
            {
                
                string line = "";
                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    //check if we need to reformat this by language
                    string word = row[i].Equals(DBNull.Value) ? confidentialValue : Convert.ToString( row[i]);

                    //Doubles will vary by language. However, this should only be applied to the last column, i.e. the VALUE
                    if (Double.TryParse(word, out double result) && i.Equals(dt.Columns.Count - 1))
                    {

                        word = result.ToString(ci != null ? ci : CultureInfo.CreateSpecificCulture(lngIsoCode));
                    }

                    line = line + quote + word + quote + (i < (dt.Columns.Count - 1) ? sep : "" ) ;


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

    }
}
